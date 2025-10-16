using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TestSystem_GA : MonoBehaviour
{
    [Header("Scripted cards for the FIRST turn (optional)")]
    [SerializeField] private List<AllCardData> firstTurnCards = new();   // if empty, falls back to count

    [Header("Fallback count for first turn (used if firstTurnCards empty)")]
    [SerializeField] private int   cardsOnFirstTurn = 5;
    [SerializeField] private float firstTurnStagger = 0.12f;

    [Header("Per-turn scripted deck (optional)")]
    [SerializeField] private List<AllCardData> perTurnDeck = new();
    private int perTurnIndex = 0;

    private bool drawnThisTurn;
    private bool firstTurn = true;

    private void Start() => StartCoroutine(DoFirstTurn());

    private void Update()
    {
        if (TurnBaseGod.state == TurnBaseGodState.PlayerTurn)
        {
            if (!firstTurn && !drawnThisTurn)
            {
                if (perTurnDeck != null && perTurnDeck.Count > 0)
                {
                    var card = perTurnDeck[perTurnIndex % perTurnDeck.Count];
                    ActionSystem.Instance.Perform(new DrawcardGA(card)); // params ctor
                    perTurnIndex++;
                }
                else
                {
                    ActionSystem.Instance.Perform(new DrawcardGA(1)); // default card
                }

                drawnThisTurn = true;
                Debug.Log("[TestSystem_GA] Per-turn draw requested.");
            }
        }
        else
        {
            drawnThisTurn = false;
        }
    }

    private IEnumerator DoFirstTurn()
    {
        yield return new WaitForSeconds(0.5f);

        if (firstTurnCards != null && firstTurnCards.Count > 0)
        {
            // Single GA with the whole first-hand list (best with ActionSystem queue or single action)
            ActionSystem.Instance.Perform(new DrawcardGA(firstTurnCards));
            if (firstTurnStagger > 0f)
                yield return new WaitForSeconds(firstTurnStagger * firstTurnCards.Count);
        }
        else
        {
            // Fallback: N default draws
            // (Consider queuing in ActionSystem or waiting for isPerforming to avoid drops)
            for (int i = 0; i < cardsOnFirstTurn; i++)
            {
                ActionSystem.Instance.Perform(new DrawcardGA(1));
                if (firstTurnStagger > 0f) yield return new WaitForSeconds(firstTurnStagger);
            }
        }

        firstTurn = false;
    }
}
