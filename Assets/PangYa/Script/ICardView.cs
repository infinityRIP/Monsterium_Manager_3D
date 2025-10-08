// Shows what a card "looks like" to the UI (read-only).
using UnityEngine;

public interface ICardView
{
    string Name { get; }
    int Cost { get; }
    CardType Type { get; }
    Sprite Artwork { get; }
    Sprite Background { get; }
    Sprite Frame { get; }
}
