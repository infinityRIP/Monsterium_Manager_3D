using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    // The base card data that this display is currently showing.
    // Setting this property will automatically update the display.
    private AllCardData _cardData;
    public AllCardData CardData
    {
        get => _cardData;
        set { _cardData = value; UpdateDisplay(); }
    }

    [Header("UI Refs")]
    public TextMeshProUGUI nameText;
    public Image cardBackGround;
    public TextMeshProUGUI costText;
    public Image artworkImage;
    public Image typeFrameImage;

    [Header("Type → Sprite Mapping")]
    public List<CardTypeSprite> typeSprites = new();

    // A dictionary for quick lookup of sprites based on CardType.
    private Dictionary<CardType, Sprite> _dict;

    void Awake()
    {
        BuildDict();
    }

    // Called in the editor when the script is loaded or a value is changed.
    // Ensures the dictionary is always up-to-date in the editor.
#if UNITY_EDITOR
    void OnValidate()
    {
        BuildDict();
        // Also update the display in editor for immediate feedback
        UpdateDisplay();
    }
#endif

    // Builds the dictionary from the serialised list of CardTypeSprite.
    private void BuildDict()
    {
        _dict = new Dictionary<CardType, Sprite>();
        foreach (var m in typeSprites)
        {
            if (!_dict.ContainsKey(m.type) && m.sprite != null)
                _dict[m.type] = m.sprite;
        }
    }

    /// <summary>
    /// Sets up the card display using raw AllCardData.
    /// This is useful for previewing cards directly from their ScriptableObject.
    /// </summary>
    /// <param name="data">The AllCardData to display.</param>
    public void Setup(AllCardData data) => CardData = data;
 
    /// <summary>
    /// Sets up the card display using an object that implements ICardView.
    /// This is the primary method for displaying runtime card instances.
    /// </summary>
    /// <param name="vm">The ICardView object (e.g., CardInstance) to display.</param>
    public void Setup(ICardView vm)
    {
        if (vm == null)
        {
            Debug.LogWarning("CardDisplay.Setup(ICardView vm) called with null ICardView.");
            return;
        }

        // Update UI elements if they are assigned in the Inspector
        if (nameText) nameText.text = vm.Name;
        if (costText) costText.text = vm.Cost.ToString();
        if (artworkImage) artworkImage.sprite = vm.Artwork;
        if (cardBackGround) cardBackGround.sprite = vm.Background;
        
        // Handle the type frame image:
        // 1. Prefer an explicit frame sprite if provided by the ICardView.
        // 2. If no explicit frame, try to find a mapped sprite based on CardType.
        if (typeFrameImage)
        {
            typeFrameImage.sprite = vm.Frame; // Attempt to use explicit frame from ICardView
            if (typeFrameImage.sprite == null && _dict != null &&    
                _dict.TryGetValue(vm.Type, out var mapped))
            {
                typeFrameImage.sprite = mapped; // Fallback to mapped sprite by type
            }                 
        }

        #if UNITY_EDITOR
        Debug.Log($"[CardDisplay] Setup(ICardView): name={vm.Name}, cost={vm.Cost}, type={vm.Type}");
        #endif
    }

    /// <summary>
    /// Internal method to update the display based on the current _cardData.
    /// This is primarily used when setting the CardData property directly.
    /// </summary>
    private void UpdateDisplay()
    {
        if (_cardData == null)
        {
            // Optionally clear display if no card data is set.
            if (nameText) nameText.text = "EMPTY";
            if (costText) costText.text = "0";
            if (artworkImage) artworkImage.sprite = null;
            if (cardBackGround) cardBackGround.sprite = null;
            if (typeFrameImage) typeFrameImage.sprite = null;
            return;
        }

        nameText.text = _cardData.CardName;
        cardBackGround.sprite = _cardData.CardBG;
        costText.text = _cardData.Cost.ToString();
        artworkImage.sprite = _cardData.CardArt;
        
        // Prioritize the frame from _cardData if it exists.
        // Otherwise, use the type-mapped sprite.
        if (typeFrameImage)
        {
            typeFrameImage.sprite = _cardData.CardFrame;
            if (typeFrameImage.sprite == null && _dict != null && _dict.TryGetValue(_cardData.CardType, out var frame))
            {
                typeFrameImage.sprite = frame;
            }
        }

        #if UNITY_EDITOR
        Debug.Log($"[CardDisplay] UpdateDisplay: name={_cardData?.CardName ?? "NULL"}, " +
        $"BG={(bool)_cardData?.CardBG}, Art={(bool)_cardData?.CardArt}, Frame={(bool)_cardData?.CardFrame}, Type={_cardData?.CardType}");
        #endif
    }
}
