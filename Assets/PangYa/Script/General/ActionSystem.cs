// ActionSystem.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Patterns;   // your CardSingleton / PersistentSingleton

/// <summary>
/// Runs GameActions through PRE → PERFORM → POST phases with reaction chaining,
/// per-type performers, and pre/post subscribers.
/// </summary>
public class ActionSystem : PersistentSingleton<ActionSystem>
{
    // Pointer to the reaction list of the *current* phase (Pre / Perform / Post).
    private List<GameAction> reactions = null;

    public bool IsPerforming { get; private set; } = false;

    // Subscribers per action type (PRE/POST)
    private static readonly Dictionary<Type, List<Action<GameAction>>> preSubs  = new();
    private static readonly Dictionary<Type, List<Action<GameAction>>> postSubs = new();

    // Performer coroutine per action type
    private static readonly Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

    // ---------- Public API --------------------------------------------------

    /// <summary>
    /// Enqueue a follow-up action for the *current* phase (called from performers/subscribers).
    /// </summary>
    /// 
  
    public void AddReaction(GameAction followUp)
    {
        if (reactions == null)
        {
            Debug.LogWarning("AddReaction was called outside of an action phase.");
            return;
        }
        reactions.Add(followUp);
    }

    /// <summary>
    /// Register the coroutine that performs a concrete action type.
    /// </summary>
    public static void AttachPerformer<J>(Func<J, IEnumerator> performer) where J : GameAction
    {
        if (performer == null) throw new ArgumentNullException(nameof(performer));
        performers[typeof(J)] = ga => performer((J)ga);
    }

    /// <summary>
    /// Optionally remove a performer registration.
    /// </summary>
    public static void DetachPerformer<J>() where J : GameAction
    {
        performers.Remove(typeof(J));
    }

    /// <summary>
    /// Subscribe a callback to PRE or POST for a given action type.
    /// </summary>
    public static void SubscribeReaction<J>(Action<J> callback, ReactionTiming timing) where J : GameAction
    {
        if (callback == null) throw new ArgumentNullException(nameof(callback));
        var dict = timing == ReactionTiming.PRE ? preSubs : postSubs;
        var key  = typeof(J);

        if (!dict.TryGetValue(key, out var list))
        {
            list = new List<Action<GameAction>>();
            dict[key] = list;
        }

        // Store a type-safe wrapper
        list.Add(ga => callback((J)ga));
    }

    /// <summary>
    /// Convenience: start a root action from anywhere (if an instance exists).
    /// </summary>
    public static void Do(GameAction root, Action onFinished = null)
    {
        if (!Instance) return;
        Instance.Perform(root, onFinished);
    }

    /// <summary>
    /// Start the 3-phase flow for a root action (non-coroutine entry like in the video).
    /// </summary>
    public void Perform(GameAction action, Action onFinished = null)
    {
        if (action == null) return;
        if (IsPerforming) return; // video pattern: block re-entry

        IsPerforming = true;
        StartCoroutine(Flow(action, () =>
        {
            IsPerforming = false;
            onFinished?.Invoke();
        }));
    }

    // ---------- Flow --------------------------------------------------------

    private IEnumerator Flow(GameAction action, Action onFlowFinished = null)
    {
        // ===== PRE =====
        reactions = action.PreReactions;
        InvokeSubscribers(preSubs, action);
        yield return DrainReactions();

        // === PERFORM ===
        reactions = action.PerformReactions;
        if (performers.TryGetValue(action.GetType(), out var perf))
            yield return perf(action);
        else
            Debug.LogWarning($"No performer registered for {action.GetType().Name}. Skipping PERFORM phase.");
        yield return DrainReactions();

        // ==== POST ====
        reactions = action.PostReactions;
        InvokeSubscribers(postSubs, action);
        yield return DrainReactions();

        reactions = null; // clear pointer for safety
        onFlowFinished?.Invoke();
    }

    /// <summary>
    /// Drain the current phase’s reaction queue; new items appended during
    /// execution are also processed (index-based loop).
    /// </summary>
    private IEnumerator DrainReactions()
    {
        int i = 0;
        while (i < reactions.Count)
        {
            var next = reactions[i++];
            yield return Flow(next);
        }
    }

    private static void InvokeSubscribers(Dictionary<Type, List<Action<GameAction>>> dict, GameAction action)
    {
        if (!dict.TryGetValue(action.GetType(), out var list)) return;

        // Snapshot count so newly-added subs during iteration don’t execute immediately
        for (int i = 0, n = list.Count; i < n; i++)
        {
            try { list[i]?.Invoke(action); }
            catch (Exception ex) { Debug.LogException(ex); }
        }
    }
}

/// <summary>Phase identifiers for reaction subscriptions.</summary>
public enum ReactionTiming
{
    PRE,
    POST
}
