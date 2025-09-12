using UnityEngine;
using UnityEngine.UI;
using TMPro; // Make sure you have TextMeshPro imported

public class CardDisplay : MonoBehaviour
{
    // This private field holds the actual data.
    private AllCardData _cardData;

    // This is the public property. Other scripts will set this,
    // and the 'set' block will automatically update the UI.
    public AllCardData CardData
    {
        get { return _cardData; }
        set
        {
            _cardData = value;
            UpdateDisplay(); // Update visuals whenever new data is assigned.
        }
    }

    // --- UI Element References ---
    // Drag these in the Unity Inspector for your Card Prefab
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public Image artworkImage;

    public CardType cardType;

    // This private method updates the UI elements using the properties from the CardData script.
    private void UpdateDisplay()
    {
        if (_cardData == null) return;

        // Use the public properties (CardName, Description, Cost, etc.)
        nameText.text = _cardData.CardName;
        descriptionText.text = _cardData.Description;
        costText.text = _cardData.Cost.ToString();
        artworkImage.sprite = _cardData.CardArt;
    }
}
