using UnityEngine;

/// <summary>
/// Runtime, per-copy state for a card in hand/deck.
/// Implements ICardView to provide display data to CardDisplay.
/// </summary>
public class CardInstance : ICardView
{
    // Reference to the immutable base data of the card.
    public AllCardData Data { get; }

    // Runtime mutable properties
    public int CurrentCost { get; set; }
    public bool IsExhausted { get; set; }

    /// <summary>
    /// Constructor for creating a new CardInstance from its base data.
    /// </summary>
    /// <param name="data">The AllCardData ScriptableObject for this card.</param>
    public CardInstance(AllCardData data)
    {
        Data = data;
        CurrentCost = data.Cost; // Initialize current cost from base data
        IsExhausted = false; // Initial state
    }

    // ICardView property projections: these properties simply return
    // the corresponding data from the immutable AllCardData.
    public string Name       => Data.CardName;
    public int Cost          => CurrentCost; // Use CurrentCost for runtime cost
    public CardType Type     => Data.CardType;
    public Sprite Artwork    => Data.CardArt;
    public Sprite Background => Data.CardBG;
    public Sprite Frame      => Data.CardFrame;

    // You can add more runtime logic here, e.g.,
    // public void PlayCard() { /* ... */ }
    // public void ApplyBuff(BuffData buff) { /* ... */ }
}
