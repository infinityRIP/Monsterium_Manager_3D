using UnityEngine;

// This is the base class for all card effects. 
// It corresponds to the "ICardEffect" interface in your diagram.
// Specific effects like "DamageEffect" or "DefendEffect" would inherit from this.
public abstract class CardEffect : ScriptableObject
{
    // The core function of any effect.
    // 'caster' is the one playing the card (e.g., the Player).
    // 'target' is the character the card is played on.
    public abstract void Execute(Character caster, Character target);
}
