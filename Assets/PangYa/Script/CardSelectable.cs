using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardDisplay))]
public class CardSelectable : MonoBehaviour, IPointerClickHandler
{
    public static CardSelectable Current;     // track one selected

    [SerializeField] private Vector2 hoverOffset = new Vector2(0, 32); // how far above the card

    private CardDisplay display;
    private RectTransform rect;

    void Awake()
    {
        display = GetComponent<CardDisplay>();
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData e)
    {
        // left click (or any tap)
        if (e.button != PointerEventData.InputButton.Left) return;

        if (Current == this)
        {
            Deselect();
        }
        else
        {
            if (Current) Current.Deselect();       // clear previous
            Current = this;
            Select();
        }
    }

    private void Select()
    {
        // highlight (optional): slight scale pulse
        rect.localScale = Vector3.one * 1.03f;

        // show the big preview near this card
        if (CardViewHoverSystem.Instance && display.CardData != null)
            CardViewHoverSystem.Instance.ShowForCard(display.CardData, rect, hoverOffset);
    }

    public void Deselect()
    {
        rect.localScale = Vector3.one;
        if (CardViewHoverSystem.Instance) CardViewHoverSystem.Instance.Hide();
        if (Current == this) Current = null;
    }

    void OnDisable()
    {
        if (Current == this) Deselect(); // safety if the card disappears
    }

    void Update()
    {
        // quick ways to cancel
        if (Current == this && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
            Deselect();
    }
}
