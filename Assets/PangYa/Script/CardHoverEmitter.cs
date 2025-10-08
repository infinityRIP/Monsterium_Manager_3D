// File: CardHoverEmitter.cs
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardDisplay))]
public class CardHoverEmitter : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    CardDisplay display;

    void Awake() => display = GetComponent<CardDisplay>();

    public void OnPointerEnter(PointerEventData e)
        => CardViewHoverSystem.Instance.Show(display.CardData, e.position);

    public void OnPointerMove(PointerEventData e)
        => CardViewHoverSystem.Instance.Move(e.position);

    public void OnPointerExit(PointerEventData e)
        => CardViewHoverSystem.Instance.Hide();

    void OnDisable()
    {
        if (CardViewHoverSystem.Instance) CardViewHoverSystem.Instance.Hide();
    }
}
