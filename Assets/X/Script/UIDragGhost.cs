using UnityEngine;
using UnityEngine.UI;

public class UIDragGhost : MonoBehaviour
{
    public static UIDragGhost I;             // singleton แบบตรงไปตรงมา
    [SerializeField] Canvas canvas;           // Canvas ที่ตัวนี้อยู่ (Screen Space)
    [SerializeField] Image ghostImage;        // Image ที่โชว์ไอคอน
    RectTransform rt;

    void Awake()
    {
        I = this;
        rt = (RectTransform)ghostImage.transform;
        ghostImage.raycastTarget = false;     // ห้ามบล็อค raycast ตอนลาก
        Hide();
    }

    public void Show(Sprite sprite)
    {
        ghostImage.sprite = sprite;
        ghostImage.enabled = sprite != null;
        gameObject.SetActive(true);
    }

    public void Move(Vector2 screenPos)
    {
        var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform, screenPos, cam, out var lp);
        rt.anchoredPosition = lp;
    }

    public void Hide()
    {
        ghostImage.enabled = false;
        gameObject.SetActive(false);
    }
}
