using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CardTypeBG
{
    public CardType type;
    public Sprite background;
}

[CreateAssetMenu(menuName = "Cards/Appearance/CardType BG Library", fileName = "CardTypeBG_Library")]
public class CardTypeBackgroundLibrary : ScriptableObject
{
    [SerializeField] private List<CardTypeBG> entries = new();
    [SerializeField] private Sprite fallback;

    private Dictionary<CardType, Sprite> _map;

    private void OnEnable() => Rebuild();
#if UNITY_EDITOR
    private void OnValidate() => Rebuild();
#endif

    private void Rebuild()
    {
        _map = new Dictionary<CardType, Sprite>();
        foreach (var e in entries)
        {
            if (e.background != null) _map[e.type] = e.background;
        }
    }

    public Sprite GetBG(CardType type)
    {
        return _map != null && _map.TryGetValue(type, out var s) ? s : fallback;
    }
}
