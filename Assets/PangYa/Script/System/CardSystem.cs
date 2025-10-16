using System.Collections;
using UnityEngine;

/// <summary>
/// Domain layer for card drawing.
/// - Listens to DrawcardGA and spawns via HandViewManager.
/// - Centralizes rules (25-card cap now; easy to extend with costs/shuffle/AI later).
/// - Keeps the view (HandViewManager) decoupled from rules.
/// </summary>
public class CardSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandViewManager hand;           // drag your HandViewManager here
    [SerializeField] private AllCardData     defaultCard;    // used when GA doesn't provide specific data

    [Header("Rules")]
    [SerializeField] private int maxHand = 25;

    // We track count on the domain side so rules are centralized.
    // If your HandViewManager exposes a count, you can read that instead.
    [SerializeField] private int currentHandCount = 0;

    private void OnEnable()
    {
        // Performer for DrawcardGA
        ActionSystem.AttachPerformer<DrawcardGA>(PerformDraw);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawcardGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    /// <summary>
    /// Draw performer: respects the hand limit, spawns cards via the view.
    /// </summary>
    private IEnumerator PerformDraw(DrawcardGA ga)
    {
        if (hand == null || defaultCard == null)
        {
            Debug.LogError("[CardSystem] Missing refs.");
            yield break;
        }

        int capacityLeft = Mathf.Max(0, maxHand - currentHandCount);

        // If GA carries explicit cards, prefer them
        if (ga.Cards != null && ga.Cards.Count > 0)
        {
            int toTake = Mathf.Min(capacityLeft, ga.Cards.Count);
            for (int i = 0; i < toTake; i++)
            {
                yield return hand.StartCoroutine(hand.Spawn(ga.Cards[i]));
                currentHandCount++;
            }

            int skipped = ga.Cards.Count - toTake;
            if (skipped > 0)
                Debug.LogWarning($"[CardSystem] Hand full ({maxHand}). Skipped {skipped} specific card(s).");
            yield break;
        }

        // Fallback: draw N of defaultCard
        int toDraw = Mathf.Clamp(ga.Amount, 0, capacityLeft);
        for (int i = 0; i < toDraw; i++)
        {
            yield return hand.StartCoroutine(hand.Spawn(defaultCard));
            currentHandCount++;
        }

        int notDrawn = ga.Amount - toDraw;
        if (notDrawn > 0)
            Debug.LogWarning($"[CardSystem] Hand full ({maxHand}). Skipped {notDrawn} draw(s).");
    }

    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        DrawcardGA drawcardGA = new(1);
        ActionSystem.Instance.AddReaction(drawcardGA);
    }


    // --- Public hooks for other systems (discard, play, mulligan, etc.) ---

    /// <summary>Call when a card leaves the hand (played/discarded).</summary>
    public void OnCardRemovedFromHand(int count = 1)
    {
        currentHandCount = Mathf.Max(0, currentHandCount - Mathf.Max(0, count));
    }

    /// <summary>Force-set if you sync with an external view count.</summary>
    public void SetHandCount(int value) => currentHandCount = Mathf.Clamp(value, 0, maxHand);

    public int  CurrentHandCount => currentHandCount;
    public int  MaxHand          => maxHand;
    public void SetMaxHand(int v) => maxHand = Mathf.Max(1, v);
}
