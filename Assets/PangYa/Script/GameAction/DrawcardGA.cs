using System.Collections.Generic;

public sealed class DrawcardGA : GameAction
{
    public int Amount { get; }                                  // fallback count
    public IReadOnlyList<AllCardData> Cards { get; }            // optional explicit cards

    // Existing usage: draw N default cards
    public DrawcardGA(int amount)
    {
        Amount = amount < 0 ? 0 : amount;
        Cards  = null;
    }

    // New usage: draw these exact cards (List)
    public DrawcardGA(IEnumerable<AllCardData> cards)
    {
        var list = new List<AllCardData>();
        if (cards != null) list.AddRange(cards);
        Cards  = list;
        Amount = list.Count; // convenience
    }

    // New usage: draw these exact cards (params for single or many)
    public DrawcardGA(params AllCardData[] cards) : this((IEnumerable<AllCardData>)cards) { }
}
