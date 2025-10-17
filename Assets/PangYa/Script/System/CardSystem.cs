using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralizes drawing rules and talks to the view.
/// </summary>
public class CardSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandViewManager hand;
    [SerializeField] private DeckRuntime     deck;

    [Header("Rules")]
    [SerializeField] private int maxHand     = 25;
    [SerializeField] private int openingHand = 5;   // draw 5 at start

    [SerializeField] private int currentHandCount = 0;

    private void Awake()
    {
        if (deck == null) deck = GetComponent<DeckRuntime>();
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawcardGA>(PerformDraw);

        // "New turn" → draw 1 at the end of EnemyTurn (so it appears at start of player's turn).
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);

        // Opening hand
        StartCoroutine(DealOpeningHand());
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawcardGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    private IEnumerator DealOpeningHand()
    {
        if (hand == null || deck == null) yield break;
        yield return StartCoroutine(DrawFromDeckToHand(openingHand));
    }

    /// <summary>Helper used by both opening hand and GA path.</summary>
    private IEnumerator DrawFromDeckToHand(int amount)
    {
        if (amount <= 0 || hand == null || deck == null) yield break;

        int capacityLeft = Mathf.Max(0, maxHand - currentHandCount);
        int want = Mathf.Min(amount, capacityLeft);
        if (want <= 0) yield break;

        List<DeckRuntime.CardInstance> batch = deck.DrawOrRebuild(want);

        foreach (var inst in batch)
        {
            if (inst?.Data == null) continue;
            yield return hand.StartCoroutine(hand.Spawn(inst.Data));
            currentHandCount++;
        }
    }

    /// <summary>
    /// Performer for DrawcardGA. Uses DeckRuntime as the source of truth and rebuilds automatically.
    /// </summary>
    private IEnumerator PerformDraw(DrawcardGA ga)
    {
        if (hand == null || deck == null) yield break;

        // If GA brought specific cards, spawn those (still respect hand cap)
        if (ga.Cards != null && ga.Cards.Count > 0)
        {
            int capacityLeft = Mathf.Max(0, maxHand - currentHandCount);
            int toTake = Mathf.Min(capacityLeft, ga.Cards.Count);
            for (int i = 0; i < toTake; i++)
            {
                var data = ga.Cards[i];
                if (data == null) continue;
                yield return hand.StartCoroutine(hand.Spawn(data));
                currentHandCount++;
            }
            yield break;
        }

        // Otherwise draw from DeckRuntime (auto rebuild if needed)
        yield return StartCoroutine(DrawFromDeckToHand(Mathf.Max(0, ga.Amount)));
    }

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        // Queue a draw of 1 at the end of enemy turn → appears as start-of-player-turn draw
        ActionSystem.Instance.AddReaction(new DrawcardGA(1));
    }

    // --- Public hooks for other systems (discard/play can call this) ---
    public void OnCardRemovedFromHand(int count = 1)
    {
        currentHandCount = Mathf.Max(0, currentHandCount - Mathf.Max(0, count));
    }

    public int  CurrentHandCount => currentHandCount;
    public int  MaxHand          => maxHand;
    public void SetMaxHand(int v) => maxHand = Mathf.Max(1, v);
}
