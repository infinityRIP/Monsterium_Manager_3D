using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;
    public bool isPerforming { get; private set; } = false;

    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

    public event Action OnPerformFinished = null;

    public void Perform(GameAction action)
    {
        if (isPerforming) return;
        isPerforming = true;
        StartCoroutine(Flow(action));
    }

    public void AddReaction(GameAction gameAction)
    {
        reactions.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        yield return PerformPerformer(action);

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        OnFlowFinished?.Invoke();
        isPerforming = false;
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
        else
        {
            Debug.LogError($"No performer found for action type: {type}");
        }
    }

    private void PerformSubscribers(GameAction gameAction, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = gameAction.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(gameAction);
            }
        }
    }

    private IEnumerator PerformReactions()
    {
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction);
        }
        reactions.Clear();
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type))
        {
            Debug.LogWarning($"Performer already attached for action type: {type}. Overwriting.");
            performers[type] = (action) => performer((T)action);
        }
        else
        {
            performers.Add(type, (action) => performer((T)action));
        }
    }

    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type))
        {
            performers.Remove(type);
        }
    }

    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Type type = typeof(T);
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;

        if (!subs.ContainsKey(type))
        {
            subs.Add(type, new List<Action<GameAction>>());
        }
        subs[type].Add((gameAction) => reaction((T)gameAction));
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Type type = typeof(T);
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;

        if (subs.ContainsKey(type))
        {
            subs[type].Remove((gameAction) => reaction((T)gameAction));
        }
    }
}