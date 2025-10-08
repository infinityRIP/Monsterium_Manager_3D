using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
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

    private Dictionary<CardType, Sprite> _dict;

    void Awake()
    {
        BuildDict();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        BuildDict();
    }
#endif

    private void BuildDict()
    {
        _dict = new Dictionary<CardType, Sprite>();
        foreach (var m in typeSprites)
        {
            if (!_dict.ContainsKey(m.type) && m.sprite != null)
                _dict[m.type] = m.sprite;
        }
    }

    
    
    public void Setup(AllCardData data) => CardData = data;
 
    public void Setup(ICardView vm)
    {
    if (vm == null) return;
    if (nameText)         nameText.text = vm.Name;
    if (costText)         costText.text = vm.Cost.ToString();
    if (artworkImage)     artworkImage.sprite = vm.Artwork;
    if (cardBackGround)   cardBackGround.sprite = vm.Background;
    if (typeFrameImage)
    {
        // Prefer explicit frame if present, else use your type→sprite mapping
        typeFrameImage.sprite = vm.Frame;
        if (typeFrameImage.sprite == null && _dict != null &&    
            _dict.TryGetValue(vm.Type, out var mapped))
            typeFrameImage.sprite = mapped;                 
    }
    }

    private void UpdateDisplay()
    {
        if (_cardData == null) return;

        nameText.text = _cardData.CardName;
        cardBackGround.sprite = _cardData.CardBG;
        costText.text = _cardData.Cost.ToString();
        artworkImage.sprite = _cardData.CardArt;
        typeFrameImage.sprite = _cardData.CardFrame;


        if (typeFrameImage && _dict != null && _dict.TryGetValue(_cardData.CardType, out var frame))
            typeFrameImage.sprite = frame;

        #if UNITY_EDITOR
        Debug.Log($"[CardDisplay] UpdateDisplay: name={_cardData?.CardName ?? "NULL"}, " +
        $"BG={(bool)_cardData?.CardBG}, Art={(bool)_cardData?.CardArt}, Frame={(bool)_cardData?.CardFrame}");
        #endif

    }
}
