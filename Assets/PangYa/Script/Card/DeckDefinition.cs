// Assets/Scripts/Cards/DeckDefinition.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Deck Definition", fileName = "Deck_")]
public class DeckDefinition : ScriptableObject
{
    [SerializeField] private List<Deckdata> cards = new();
    public IReadOnlyList<Deckdata> Cards => cards;
}
