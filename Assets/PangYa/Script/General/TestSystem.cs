using System.Collections;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private HandViewManager hand;   // <- drag the component on CardViewCreator
    [SerializeField] private AllCardData testCardData;

    [Header("First turn draw")]
    [SerializeField] private int   cardsOnFirstTurn = 5;
    [SerializeField] private float firstTurnStagger = 0.12f;

    private bool drawnThisTurn;
    private bool firstTurn = true;

    private void Start()
    {
        StartCoroutine(DoFirstTurn());
    }

    private void Update()
    {
        // One draw per player turn (after the first turn is finished)
        if (TurnBaseGod.state == TurnBaseGodState.PlayerTurn)
        {
            if (!firstTurn && !drawnThisTurn)
            {
                StartCoroutine(hand.Spawn(testCardData)); // animate in, then CardContainer takes over
                drawnThisTurn = true;
                Debug.Log("Player drew a card!");
            }
        }
        else
        {
            // Reset when turn changes away from player
            drawnThisTurn = false;
        }
    }

    private IEnumerator DoFirstTurn()
    {
        // small delay so everything initializes
        yield return new WaitForSeconds(0.5f);

        // Draw N cards with a little stagger
        for (int i = 0; i < cardsOnFirstTurn; i++)
        {
            yield return hand.StartCoroutine(hand.Spawn(testCardData));
            if (firstTurnStagger > 0f) yield return new WaitForSeconds(firstTurnStagger);
        }

        firstTurn = false;
    }
}
