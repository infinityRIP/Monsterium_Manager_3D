using UnityEngine;
using DG.Tweening;
using Game.Patterns; // ← this must match your Singleton<T> namespace

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardDisplay cardPrefab;
    [SerializeField] private Transform defaultParent; // assign HandRoot in Inspector

    public CardDisplay CreateCardView(Vector3 position, Quaternion rotation, Transform parentOverride = null)
    {
        var parent = parentOverride != null ? parentOverride : defaultParent;
        var card = parent ? Instantiate(cardPrefab, parent) : Instantiate(cardPrefab);

        if (parent)
        {
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.identity;
        }
        else
        {
            card.transform.SetPositionAndRotation(position, rotation);
        }

        card.transform.localScale = Vector3.zero;
        card.transform.DOScale(Vector3.one, 0.15f);
        return card;
    }
}
