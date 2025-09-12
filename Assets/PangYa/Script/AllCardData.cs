using UnityEngine;
using System.Collections.Generic;

// Note: I've renamed the class to CardData, as its properties define a single card.
[CreateAssetMenu(fileName = "New Card", menuName = "Card Game/New Card")]
public class AllCardData : ScriptableObject
{
    // --- Private Backing Fields ---
    // These are hidden from other scripts but are visible in the Unity Inspector
    // thanks to the [SerializeField] attribute. This is where the data is actually stored.
    [Header("Card Information")]
    [SerializeField] private string cardName;
    [SerializeField] private int cost;
    [SerializeField, TextArea(3, 5)] private string description;
    
    [Header("Visuals")]
    [SerializeField] private Sprite cardArt;

    [Header("Card Logic")]
    [SerializeField] private CardType cardType;
    [SerializeField] private List<CardEffect> effects;

    // --- Public Properties (get/set) ---
    // Other scripts will use these to access the card's data. They simply
    // get or set the values of the private fields above.
    public string CardName { get { return cardName; } set { cardName = value; } }
    public int Cost { get { return cost; } set { cost = value; } }
    public string Description { get { return description; } set { description = value; } }
    public Sprite CardArt { get { return cardArt; } set { cardArt = value; } }
    public CardType CardType { get { return cardType; } set { cardType = value; } }
    public List<CardEffect> Effects { get { return effects; } } // Typically, you don't want other scripts to replace the whole list, so we only provide a 'get'.


    // This method would be called by the Player when the card is played.
    // It iterates through all the effects and executes them.
    public void Play(Character caster, Character target)
    {
        foreach (var effect in effects)
        {
            effect.Execute(caster, target);
        }
    }
}
