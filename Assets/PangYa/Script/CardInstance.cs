// Runtime, per-copy state for a card in hand/deck.
// You can extend this later (buffs, flags, etc.)
using UnityEngine;

public class CardInstance : ICardView
{
    public AllCardData Data { get; }
    public int CurrentCost { get; set; }
    public bool IsExhausted { get; set; }

    public CardInstance(AllCardData data)
    {
        Data = data;
        CurrentCost = data.Cost;
    }

    // ICardView projection
    public string Name       => Data.CardName;
    public int Cost          => CurrentCost;
    public CardType Type     => Data.CardType;
    public Sprite Artwork    => Data.CardArt;
    public Sprite Background => Data.CardBG;
    public Sprite Frame      => Data.CardFrame;
}
