using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class CardHoverLift : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Lift look")]
    [SerializeField] float liftPixels   = 90f;
    [SerializeField] float scaleOnHover = 1.12f;
    [SerializeField] float tiltDegrees  = 0f;
    [SerializeField] float dur          = 0.10f;

    RectTransform rect;
    Vector2    startAnchored;
    Vector3    startScale;
    Quaternion startRot;
    int        startSibling;
    bool       hovering;

    HandViewManager hand;   // to lock/unlock during hover
    CardDisplay     card;   // lock key

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        hand = GetComponentInParent<HandViewManager>();
        card = GetComponent<CardDisplay>();
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (hovering) return;
        hovering = true;

        // save original local pose (no parenting/canvas changes!)
        startAnchored = rect.anchoredPosition;
        startScale    = rect.localScale;
        startRot      = rect.localRotation;
        startSibling  = rect.GetSiblingIndex();

        // draw above neighbors inside the same parent
        rect.SetAsLastSibling();

        // stop layout from touching this card while hovered
        hand?.LockCard(card);

        // tween in *local/anchored* space so pointer stays stable
        rect.DOKill();
        rect.DOAnchorPosY(startAnchored.y + liftPixels, dur).SetUpdate(true);
        rect.DOScale(scaleOnHover, dur).SetUpdate(true);
        rect.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, tiltDegrees), dur).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!hovering) return;
        hovering = false;

        rect.DOKill();

        // return to exact original local pose
        rect.DOAnchorPos(startAnchored, dur).OnComplete(() =>
        {
            rect.SetSiblingIndex(startSibling);
        }).SetUpdate(true);

        rect.DOScale(startScale, dur).SetUpdate(true);
        rect.DOLocalRotateQuaternion(startRot, dur).SetUpdate(true);

        hand?.UnlockCard(card);
    }

    void OnDisable()
    {
        if (hovering) { hovering = false; hand?.UnlockCard(card); }
        rect.DOKill();
        // no need to forcibly restore pose here; HandView will re-layout when enabled
    }
}
