using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null; // current reaction bucket (pre or post)
    public bool isPerforming { get; private set; } = false;

    private static readonly Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static readonly Dictionary<Type, List<Action<GameAction>>> postSubs = new();

    // Keep wrapper indexes so Unsubscribe can remove exactly what Subscribe added
    private static readonly Dictionary<Type, Dictionary<Delegate, Action<GameAction>>> preIndex = new();
    private static readonly Dictionary<Type, Dictionary<Delegate, Action<GameAction>>> postIndex = new();

    private static readonly Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

    public event Action OnPerformFinished = null;

    public void Perform(GameAction action)
    {
        if (isPerforming) return;
        isPerforming = true;
        StartCoroutine(Flow(action, () =>
        {
            isPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    public void AddReaction(GameAction gameAction)
    {
        if (reactions == null)
        {
            Debug.LogWarning("AddReaction() called while no reaction list is active. Ignored.");
            return;
        }
        reactions.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        // PRE
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions(reactions);

        // MAIN
        yield return PerformPerformer(action);

        // POST
        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions(reactions);

        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        var type = action.GetType();
        if (performers.TryGetValue(type, out var perf))
        {
            yield return perf(action);
        }
        else
        {
            Debug.LogError($"No performer found for action type: {type}");
            yield break;
        }
    }

    private static void PerformSubscribers(GameAction gameAction, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        var type = gameAction.GetType();
        if (!subs.TryGetValue(type, out var list) || list.Count == 0) return;

        // Snapshot to avoid issues if reactions subscribe/unsubscribe during iteration
        var snapshot = list.ToArray();
        for (int i = 0; i < snapshot.Length; i++)
        {
            try { snapshot[i](gameAction); }
            catch (Exception e) { Debug.LogException(e); }
        }
    }

    private static IEnumerator PerformReactions(List<GameAction> list)
    {
        if (list == null || list.Count == 0) yield break;

        // Use index-based loop so newly added reactions (via AddReaction) are included
        for (int i = 0; i < list.Count; i++)
        {
            var r = list[i];
            // Re-enter Flow for nested reactions
            yield return Instance.Flow(r);
        }

        // Clear the exact list we just processed
        list.Clear();
    }

    // ---- Performer API ----
    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        var type = typeof(T);
        performers[type] = (action) => performer((T)action);
    }

    public static void DetachPerformer<T>() where T : GameAction
    {
        var type = typeof(T);
        performers.Remove(type);
    }

    // ---- Reaction API ----
    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        var type = typeof(T);
        var subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        var index = timing == ReactionTiming.PRE ? preIndex : postIndex;

        if (!subs.TryGetValue(type, out var list))
        {
            list = new List<Action<GameAction>>();
            subs[type] = list;
        }
        if (!index.TryGetValue(type, out var map))
        {
            map = new Dictionary<Delegate, Action<GameAction>>();
            index[type] = map;
        }

        // Create and store a stable wrapper so we can remove it later
        Action<GameAction> wrapper = (ga) => reaction((T)ga);
        map[reaction] = wrapper;
        list.Add(wrapper);
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        var type = typeof(T);
        var subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        var index = timing == ReactionTiming.PRE ? preIndex : postIndex;

        if (!index.TryGetValue(type, out var map)) return;
        if (!map.TryGetValue(reaction, out var wrapper)) return;

        if (subs.TryGetValue(type, out var list))
        {
            list.Remove(wrapper);
        }
        map.Remove(reaction);
    }
}
