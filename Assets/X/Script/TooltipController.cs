using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipController : MonoBehaviour
{
    public static TooltipController I;

    [SerializeField] RectTransform panel;    // พื้นหลัง
    [SerializeField] TMP_Text text;          // ตัวอักษร
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject Des;
    [SerializeField] Vector2 offset = new(16, -16);

    CanvasGroup cg;
    Coroutine revealCo;

    void Awake()
    {
        I = this;
        if (!canvas) canvas = GetComponent<Canvas>();

        cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;          // ห้ามบังเมาส์
        cg.alpha = 0f;                      // active ตลอดแต่โปร่งใส

        // ปักซ้ายบน: ย้ายแล้วไม่เด้ง
        panel.anchorMin = panel.anchorMax = new Vector2(0, 1);
        panel.pivot = new Vector2(0, 1);

        // กัน text บัง raycast
        if (text) text.raycastTarget = false;
        var img = panel.GetComponent<Image>(); if (img) img.raycastTarget = false;
    }

    public void PrimeAtMouse(string content)
    {
        if (string.IsNullOrEmpty(content)) return;

        // 1) เซ็ตข้อความ + คำนวนขนาดล่วงหน้า
        text.text = content;
        text.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
        Des.SetActive(true);

        // 2) ย้ายไปตำแหน่งเมาส์ "ก่อนโชว์"
        Reposition(Input.mousePosition);
        panel.SetAsLastSibling();

        // 3) เผยในเฟรมถัดไป (ให้ layout pass เสร็จ)
        cg.alpha = 0f;
        if (revealCo != null) StopCoroutine(revealCo);
        revealCo = StartCoroutine(RevealNextFrame());
    }

    IEnumerator RevealNextFrame()
    {
        yield return new WaitForEndOfFrame(); // ให้ UI คำนวนครบ 1 รอบ
        cg.alpha = 1f;
        revealCo = null;
    }

    public void Reposition(Vector2 screenPos)
    {
        // snap พิกัดเล็กน้อยกัน sub-pixel jitter
        screenPos = new Vector2(Mathf.Round(screenPos.x), Mathf.Round(screenPos.y));

        var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform, screenPos, cam, out var lp);
        panel.anchoredPosition = lp + offset;
    }

    public void Hide()
    {
        if (revealCo != null) { StopCoroutine(revealCo); revealCo = null; }
        cg.alpha = 0f; // ไม่ SetActive เพื่อเลี่ยง rebuild
    }
}

