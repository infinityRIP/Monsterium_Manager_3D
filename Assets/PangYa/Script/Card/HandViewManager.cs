using System.Collections;
using UnityEngine;
using DG.Tweening;

public class HandViewManager : MonoBehaviour
{
    [Header("Wires")]
    [SerializeField] private CardContainer container;   // Hand object with CardContainer
    [SerializeField] private CardDisplay   cardPrefab;  // Prefab that has CardDisplay
    [SerializeField] private Transform     deckOrigin;  // Where cards fly in from

    [Header("Entry Animation")]
    [SerializeField] private EntryStyle entryStyle = EntryStyle.Fly;
    [SerializeField] private float duration = 0.40f;
    [SerializeField] private Ease  ease     = Ease.OutCubic;
    [SerializeField] private float arcHeight = 0.6f;     // for Jump/Bezier
    [SerializeField, Range(3, 20)] private int bezierSamples = 8;

    /// <summary>Spawn 1 card: animate, then hand off to CardContainer.</summary>
    public IEnumerator Spawn(AllCardData data)
    {
        if (!ValidateWires(data)) yield break;

        // 1) Spawn at deck (or near hand if no deckOrigin)
        Vector3 startPos = deckOrigin ? deckOrigin.position
                                      : container.transform.position + Vector3.up * 0.2f;
        Quaternion startRot = deckOrigin ? deckOrigin.rotation : container.transform.rotation;

        CardDisplay view = Instantiate(cardPrefab, startPos, startRot);

        // Your CardDisplay exposes Setup(AllCardData) which updates the visuals. :contentReference[oaicite:2]{index=2}
        view.Setup(data);

        // Small pop
        view.transform.localScale = Vector3.one * 0.95f;

        // 2) Animate towards the hand (container center)
        Vector3 endPos = container.transform.position;
        yield return PlayEntry(view.transform, startPos, endPos);

        // 3) Parent under the container (keep world pose). CardContainer now controls layout.
        view.transform.SetParent(container.transform, true);
    }

    /// <summary>Spawn multiple cards with a small stagger.</summary>
    public IEnumerator SpawnMany(AllCardData[] cards, float stagger = 0.08f)
    {
        if (cards == null || cards.Length == 0) yield break;
        for (int i = 0; i < cards.Length; i++)
        {
            yield return Spawn(cards[i]);
            if (stagger > 0f && i < cards.Length - 1)
                yield return new WaitForSeconds(stagger);
        }
    }

    // ---------------- internals ----------------

    private IEnumerator PlayEntry(Transform t, Vector3 a, Vector3 c)
    {
        t.DOKill();
        switch (entryStyle)
        {
            case EntryStyle.Fly:
                yield return t.DOMove(c, duration).SetEase(ease).WaitForCompletion();
                break;

            case EntryStyle.Jump:
                yield return t.DOJump(c, arcHeight, 1, duration).SetEase(ease).WaitForCompletion();
                break;

            case EntryStyle.Bezier:
                Vector3 mid = Vector3.Lerp(a, c, 0.5f) + Vector3.up * arcHeight;
                Vector3[] path = BuildQuadratic(a, mid, c, bezierSamples);
                yield return t.DOPath(path, duration, PathType.CatmullRom)
                              .SetEase(ease)
                              .WaitForCompletion();
                break;
        }
        yield return t.DOScale(1f, 0.10f).SetEase(Ease.OutSine).WaitForCompletion();
    }

    private Vector3[] BuildQuadratic(Vector3 a, Vector3 b, Vector3 c, int samples)
    {
        samples = Mathf.Max(3, samples);
        var pts = new Vector3[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = i / (samples - 1f);
            pts[i] = (1 - t) * (1 - t) * a + 2 * (1 - t) * t * b + t * t * c;
        }
        return pts;
    }

    private bool ValidateWires(AllCardData data)
    {
        if (!container)   { Debug.LogWarning("[HandViewManager] Missing CardContainer."); return false; }
        if (!cardPrefab)  { Debug.LogWarning("[HandViewManager] Missing CardDisplay prefab."); return false; }
        if (!data)        { Debug.LogWarning("[HandViewManager] CardData is null."); return false; }
        return true;
    }

    public enum EntryStyle { Fly, Jump, Bezier }
}
