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
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public Image artworkImage;
    // Add a reference for a banner or frame to change its color
    public Image typeBannerImage; 

    [Header("Type Colors")]
    // Set these colors in the Inspector to define your theme
    public Color attackColor = new Color(0.8f, 0.2f, 0.2f); // A nice red
    public Color skillColor = new Color(0.2f, 0.5f, 0.8f); // A nice blue
    public Color defenseColor = new Color(0.2f, 0.8f, 0.5f); // A nice green
    public Color counterColor = new Color(0.9f, 0.7f, 0.1f); // A nice gold/yellow
    public Color ultimateColor = new Color(0.6f, 0.1f, 0.9f); // A nice purple


    // This private method updates the UI elements using the properties from the CardData script.
    private void UpdateDisplay()
    {
        if (_cardData == null) return;

        // Use the public properties (CardName, Description, Cost, etc.)
        nameText.text = _cardData.CardName;
        descriptionText.text = _cardData.Description;
        costText.text = _cardData.Cost.ToString();
        artworkImage.sprite = _cardData.CardArt;

        // --- Updated code to handle the new CardType enum ---
        if (typeBannerImage != null)
        {
            switch (_cardData.CardType)
            {
                case CardType.Attack:
                    typeBannerImage.color = attackColor;
                    break;
                case CardType.Skill:
                    typeBannerImage.color = skillColor;
                    break;
                case CardType.Defense:
                    typeBannerImage.color = defenseColor;
                    break;
                case CardType.Counter:
                    typeBannerImage.color = counterColor;
                    break;
                case CardType.Ultimate:
                    typeBannerImage.color = ultimateColor;
                    break;
                default:
                    // A default color in case a type is not set
                    typeBannerImage.color = Color.gray;
                    break;
            }
        }
    }
}
