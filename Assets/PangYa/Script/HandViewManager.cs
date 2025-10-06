using System.Collections;              // ← add this
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;


public class HandViewManager : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardDisplay> _cardD = new();

public IEnumerator AddCard(CardDisplay card)
{
    card.transform.SetParent(transform, true);
    _cardD.Add(card);
    yield return UpdateCardPositions(0.15f);
}

    private IEnumerator UpdateCardPositions(float duration)
    {
        if (_cardD.Count == 0) yield break;
        float cardSpacing = 1f / 10f;
        float firstCardPos = 0.5f - (_cardD.Count - 1) * cardSpacing / 2f;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < _cardD.Count; i++)
        {
            float p = firstCardPos + i * cardSpacing;
            Vector3 splinePos = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
            _cardD[i].transform.DOMove(splinePos + transform.position + 0.01f * Vector3.back, duration);
            _cardD[i].transform.DORotate(rotation.eulerAngles, duration);
        }

        yield return new WaitForSeconds(duration);

    }
}    
