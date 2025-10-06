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

    private void UpdateDisplay()
    {
        if (_cardData == null) return;

        nameText.text          = _cardData.CardName;
        cardBackGround.sprite  = _cardData.CardBG;
        costText.text          = _cardData.Cost.ToString();
        artworkImage.sprite    = _cardData.CardArt;
        typeFrameImage.sprite =  _cardData.CardFrame;


        if (typeFrameImage && _dict != null && _dict.TryGetValue(_cardData.CardType, out var frame))
            typeFrameImage.sprite = frame;
    }
}
