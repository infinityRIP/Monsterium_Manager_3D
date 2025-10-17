using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DeckRuntime : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private DeckDefinition startingDeck;

    [Header("Debug (readonly)")]
    [SerializeField] private int drawCount;
    [SerializeField] private int discardCount;

    [Serializable]
    public class CardInstance
    {
        public Deckdata Entry;
        public AllCardData Data => Entry != null ? Entry.Data : null;
        public int Cost => Entry != null ? Entry.EffectiveCost : 0;
        public override string ToString() => Data != null ? Data.CardName : "(null)";
    }

    private readonly List<CardInstance> _drawPile = new();
    private readonly List<CardInstance> _discardPile = new();
    private System.Random _rng;

    void Awake() => ResetFrom(startingDeck);

    public void ResetFrom(DeckDefinition deckDef, int? seed = null)
    {
        _drawPile.Clear();
        _discardPile.Clear();

        if (deckDef != null)
        {
            foreach (var e in deckDef.Cards)
            {
                if (e == null || e.Data == null) continue;
                int copies = Mathf.Max(1, e.Quantity);
                for (int i = 0; i < copies; i++)
                    _drawPile.Add(new CardInstance { Entry = e });
            }
        }

        _rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        Shuffle(_drawPile, _rng);
        SyncDebug();
    }

    public int DrawPileCount => _drawPile.Count;
    public int DiscardPileCount => _discardPile.Count;
    public bool IsCompletelyEmpty => _drawPile.Count == 0 && _discardPile.Count == 0;

    /// <summary>Rebuild from the serialized startingDeck.</summary>
    public void RebuildFromStartingDeck()
    {
        if (startingDeck == null)
        {
            Debug.LogWarning("[DeckRuntime] No startingDeck assigned to rebuild from.");
            return;
        }
        ResetFrom(startingDeck);
    }

    /// <summary>Draw one card. If draw & discard empty, rebuild from startingDeck once.</summary>
    public CardInstance DrawOne()
    {
        // If draw pile empty but we have discard, recycle.
        if (_drawPile.Count == 0 && _discardPile.Count > 0)
        {
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            Shuffle(_drawPile, _rng);
        }

        // If everything empty, rebuild from definition.
        if (_drawPile.Count == 0 && _discardPile.Count == 0)
        {
            if (startingDeck != null)
            {
                RebuildFromStartingDeck();
            }
            else
            {
                // Still empty and nowhere to rebuild from.
                return null;
            }
        }

        if (_drawPile.Count == 0) return null;

        int last = _drawPile.Count - 1;
        var c = _drawPile[last];
        _drawPile.RemoveAt(last);
        SyncDebug();
        return c;
    }

    /// <summary>Draw up to 'count'. If deck runs dry completely, rebuild once and continue.</summary>
    public List<CardInstance> DrawOrRebuild(int count)
    {
        var result = new List<CardInstance>(count);
        for (int i = 0; i < count; i++)
        {
            var one = DrawOne();
            if (one == null)
            {
                // If DrawOne returned null, try rebuilding one more time (in case startingDeck was empty before)
                if (IsCompletelyEmpty && startingDeck != null)
                {
                    RebuildFromStartingDeck();
                    one = DrawOne();
                }
                if (one == null) break;
            }
            result.Add(one);
        }
        return result;
    }

    public void Discard(CardInstance c)
    {
        if (c == null) return;
        _discardPile.Add(c);
        SyncDebug();
    }

    public void DiscardMany(IEnumerable<CardInstance> cards)
    {
        if (cards == null) return;
        _discardPile.AddRange(cards);
        SyncDebug();
    }

    public void ShuffleDrawPile()
    {
        Shuffle(_drawPile, _rng);
        SyncDebug();
    }

    private static void Shuffle<T>(IList<T> list, System.Random rng)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void SyncDebug()
    {
        drawCount = _drawPile.Count;
        discardCount = _discardPile.Count;
    }
}
