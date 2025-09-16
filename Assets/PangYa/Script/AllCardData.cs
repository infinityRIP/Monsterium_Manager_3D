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
    [SerializeField] private Sprite cardBG;

    [Header("Visuals")]
    [SerializeField] private Sprite cardArt;
    [SerializeField] private Sprite cardFrame;   // <-- missing semicolon fixed

    [Header("Card Logic")]
    [SerializeField] private CardType cardType;
    [SerializeField] private List<CardEffect> effects = new List<CardEffect>(); // init so it's never null

    // Properties
    public string CardName { get => cardName; set => cardName = value; }
    public int Cost { get => cost; set => cost = value; }
    public Sprite CardBG { get => cardBG; set => cardBG = value; }         // <-- set backing field, not property
    public Sprite CardArt { get => cardArt; set => cardArt = value; }
    public Sprite CardFrame { get => cardFrame; set => cardFrame = value; } // <-- same fix
    public CardType CardType { get => cardType; set => cardType = value; }
    public IReadOnlyList<CardEffect> Effects => effects; // expose read-only view

    // This method would be called by the Player when the card is played.
    // It iterates through all the effects and executes them.
   public void Play(Character caster, Character target)
    {
        if (effects == null) return;
        foreach (var effect in effects)
        {
            if (effect != null)
                effect.Execute(caster, target);
        }
    }
}
