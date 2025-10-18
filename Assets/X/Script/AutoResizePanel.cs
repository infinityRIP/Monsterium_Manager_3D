using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoResizePanel : MonoBehaviour
{
    [SerializeField] RectTransform panel;     // RectTransform ของพื้นหลัง
    [SerializeField] TMP_Text text;           // TextMeshProUGUI
    [SerializeField] float maxWidth = 420f;

    [Header("Padding")]
    [SerializeField] float padLeft = 24;
    [SerializeField] float padRight = 24;
    [SerializeField] float padTop = 16;
    [SerializeField] float padBottom = 16;

    RectTransform TextRT => (RectTransform)text.transform;

    void Awake()
    {
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Truncate;
        text.alignment = TextAlignmentOptions.TopLeft;

        // บังคับยึดมุมซ้ายบน (แก้ปัญหาลอย)
        TextRT.pivot = new Vector2(0, 1);
        TextRT.anchorMin = new Vector2(0, 1);
        TextRT.anchorMax = new Vector2(0, 1);
    }

    public void SetText(string s)
    {
        text.text = s;
        Recalc();
    }

    public void Refresh() => Recalc();

    void Recalc()
    {
        float contentMaxW = Mathf.Max(0, maxWidth - (padLeft + padRight));

        // จำกัดความกว้างของข้อความก่อน
        TextRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentMaxW);

        // ขอขนาดที่ต้องการภายใต้เพดานความกว้าง
        Vector2 pref = text.GetPreferredValues(text.text, contentMaxW, 0);

        float w = Mathf.Min(pref.x, contentMaxW);
        float h = pref.y;

        // ขนาดจริงของ Text
        TextRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        TextRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);

        // เอาข้อความเข้าไปในกรอบตาม padding (ซ้ายบน)
        TextRT.anchoredPosition = new Vector2(padLeft, -padTop);

        // ขนาด Panel = ขนาดข้อความ + padding
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w + padLeft + padRight);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h + padTop + padBottom);

        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
    }
}
