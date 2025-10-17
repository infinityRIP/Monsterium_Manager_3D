// Assets/Scripts/Cards/ScriptableCard.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Scriptable Card", fileName = "Card_")]
public class Deckdata : ScriptableObject
{
    [Header("Reference")]
    [SerializeField] private AllCardData data;   // your existing SO
    public AllCardData Data => data;

    [Header("Deck Settings")]
    [Min(1)] [SerializeField] private int quantity = 1;   // how many copies in the deck
    public int Quantity => quantity;

    [Tooltip("If >= 0, overrides Data.Cost specifically for this deck entry.")]
    [SerializeField] private int costOverride = -1;

    public int EffectiveCost => costOverride >= 0 ? costOverride : (data != null ? data.Cost : 0);

    public string DisplayName => data != null ? data.CardName : "(null)";
}
