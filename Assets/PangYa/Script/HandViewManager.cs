using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class HandViewManager : MonoBehaviour

{
    // at top-level inside the class:
    private readonly HashSet<CardDisplay> _locked = new();
    public void LockCard(CardDisplay c)   { if (c) _locked.Add(c); }
    public void UnlockCard(CardDisplay c) { if (c) _locked.Remove(c); }


    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardDisplay> _cards = new();

    public IEnumerator AddCard(CardDisplay card)
    {
        // keep world-position but share space with hand & spline
        card.transform.SetParent(transform, true);
        _cards.Add(card);
        yield return UpdateCardPositions(0.15f);
    }
    
    

    private IEnumerator UpdateCardPositions(float duration)
    {
        if (_cards.Count == 0) yield break;

        float cardSpacing = 1f / 10f; // 10 slots across 0..1
        float firstCardPos = 0.5f - (_cards.Count - 1) * cardSpacing / 2f;

        Spline spline = splineContainer.Spline;
        

        for (int i = 0; i < _cards.Count; i++)
        {
    if (_locked.Contains(_cards[i])) continue;  // <-- skip hovered

        float p = Mathf.Clamp01(firstCardPos + i * cardSpacing);
        Vector3 splinePos = spline.EvaluatePosition(p);
        Vector3 forward   = spline.EvaluateTangent(p);
        Vector3 up        = spline.EvaluateUpVector(p);
        Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);

        var t = _cards[i].transform;
        t.DOKill();
        t.DOMove(splinePos + transform.position + 0.01f * Vector3.back, duration);
        t.DORotate(rotation.eulerAngles, duration);
        }



        
        

        yield return new WaitForSeconds(duration);
    }
}
