// File: CardViewHoverSystem.cs
using UnityEngine;
using DG.Tweening;

// If you prefer your Singleton<J>, change MonoBehaviour to Game.Patterns.Singleton<CardViewHoverSystem>
public class CardViewHoverSystem : MonoBehaviour
{
    public static CardViewHoverSystem Instance { get; private set; }

    [Header("Assign a disabled CardDisplay under Canvas")]
    [SerializeField] private CardDisplay hoverView;

    [Header("Style")]
    [SerializeField] private bool followCursor = true;
    [SerializeField] private float scaleOnShow = 1.35f;
    [SerializeField] private float popDuration = 0.08f;
    [SerializeField] private Vector2 cursorOffset = new Vector2(36, -36);

    [Header("Fixed anchor (if followCursor = false)")]
    [SerializeField] private RectTransform fixedAnchor;  // e.g., center above hand

    RectTransform hoverRect;
    Canvas rootCanvas;
    CanvasGroup cg;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        hoverRect = hoverView.GetComponent<RectTransform>();
        rootCanvas = hoverView.GetComponentInParent<Canvas>();

        // Make the preview non-blocking so it never steals the pointer
        cg = hoverView.GetComponent<CanvasGroup>();
        if (!cg) cg = hoverView.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        foreach (var g in hoverView.GetComponentsInChildren<UnityEngine.UI.Graphic>(true))
            g.raycastTarget = false;

        hoverView.gameObject.SetActive(false);
    }

    // Accept either your asset or any view-model that implements ICardView
    public void Show(AllCardData data, Vector3 screenPos)
    {
        hoverView.Setup(data);
        ShowInternal(screenPos);
    }

    public void Show(ICardView vm, Vector3 screenPos)
    {
        hoverView.Setup(vm);
        ShowInternal(screenPos);
    }

    public void Move(Vector3 screenPos)
    {
        if (hoverView.gameObject.activeSelf && followCursor)
            SetPosition(screenPos);
    }

    public void Hide()
    {
        if (!hoverView.gameObject.activeSelf) return;
        hoverRect.DOKill();
        hoverView.gameObject.SetActive(false);
    }

    // ---------- internals ----------
    void ShowInternal(Vector3 screenPos)
    {
        hoverRect.DOKill();
        hoverRect.localScale = Vector3.one * 0.96f; // slight squash
        hoverView.gameObject.SetActive(true);
        hoverRect.SetAsLastSibling();               // ensure on top
        SetPosition(screenPos);
        hoverRect.DOScale(scaleOnShow, popDuration);
    }

    void SetPosition(Vector3 screenPos)
    {
        Vector2 target;

        if (followCursor)
        {
            if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                target = (Vector2)screenPos + cursorOffset;
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)rootCanvas.transform, screenPos, rootCanvas.worldCamera, out target);
                target += cursorOffset;
            }
        }
        else
        {
            // snap to a fixed anchor rect (e.g., center above hand)
            target = WorldToCanvasPoint(fixedAnchor ? fixedAnchor.position : rootCanvas.transform.position);
        }

        // keep inside canvas
        target = ClampToCanvas(target, (RectTransform)rootCanvas.transform, hoverRect);
        // top-left pivot feels nice next to cursor; adjust as you like
        hoverRect.pivot = new Vector2(0f, 1f);
        hoverRect.anchoredPosition = target;
    }

    Vector2 WorldToCanvasPoint(Vector3 world)
    {
        Vector2 pt;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rootCanvas.transform, RectTransformUtility.WorldToScreenPoint(rootCanvas.worldCamera, world),
            rootCanvas.worldCamera, out pt);
        return pt;
    }

    static Vector2 ClampToCanvas(Vector2 pos, RectTransform canvasRect, RectTransform element)
    {
        var size = canvasRect.rect.size;
        var half = element.rect.size * element.localScale * 0.5f;

        float x = Mathf.Clamp(pos.x, 0 + half.x, size.x - half.x);
        float y = Mathf.Clamp(pos.y, 0 + half.y, size.y - half.y);
        return new Vector2(x, y);

    }
    

    // Show the preview anchored near a specific card RectTransform (top-center)
    public void ShowForCard(AllCardData data, RectTransform source, Vector2 offset)
    {
    hoverView.Setup(data);         // or Setup(ICardView) if you use the interface
    hoverView.gameObject.SetActive(true);
    hoverRect.SetAsLastSibling();

    // screen position at the top-center of the card
    Vector3 topCenterWorld = source.TransformPoint(new Vector3(source.rect.center.x, source.rect.yMax));
    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(rootCanvas.worldCamera, topCenterWorld);
    // reuse existing positioner (will clamp to canvas)
    followCursor = false;          // lock position; not following mouse
    SetPosition(screenPos + offset);

    // tiny pop
    hoverRect.localScale = Vector3.one * 0.96f;
    hoverRect.DOScale(scaleOnShow, popDuration);
    }

}
