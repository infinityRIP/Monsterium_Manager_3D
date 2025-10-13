using UnityEngine;
using DG.Tweening;
 // your Singleton<T>

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardDisplay cardPrefab;
    [SerializeField] private Transform defaultParent; // HandRoot under Canvas

    public CardDisplay CreateCardView(Vector3 position, Quaternion rotation, Transform parentOverride = null)
    {
        var p = parentOverride ? parentOverride : defaultParent;
        var view = p ? Instantiate(cardPrefab, p) : Instantiate(cardPrefab);

        if (p)
        {
            view.transform.localPosition = Vector3.one;
            view.transform.localRotation = Quaternion.identity;
        }
        else
        {
            view.transform.SetPositionAndRotation(position, rotation);
        }

        view.transform.localScale = Vector3.one;
        view.transform.DOScale(Vector3.one, 0.15f);
        return view;
    }

    // Convenience overloads:
    public CardDisplay CreateCardView(AllCardData data, Transform parentOverride = null)
    {
        var v = CreateCardView(Vector3.zero, Quaternion.identity, parentOverride);
        v.Setup(data);
        return v;
    }

    public CardDisplay CreateCardView(ICardView vm, Transform parentOverride = null)
    {
        var v = CreateCardView(Vector3.zero, Quaternion.identity, parentOverride);
        v.Setup(vm);
        return v;
    }
}
