using System.Collections;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandViewManager handView;
    [SerializeField] private Transform handParent;
    [SerializeField] private AllCardData testCardData;

    private bool drawnThisTurn = false;
    private bool firstturn;

    void Start()
    {
        firstturn = true;
        StartCoroutine(FirstTurn());
    }

    void Update()
    {
        // Only allow drawing when it's the player's turn and not already drawn
        if (TurnBaseGod.state == TurnBaseGodState.PlayerTurn && !drawnThisTurn && !firstturn)
        {
             DrawCard();
             drawnThisTurn = true;
            
        }

        // Reset draw flag when it's not the player's turn anymore
        if (TurnBaseGod.state != TurnBaseGodState.PlayerTurn)
            drawnThisTurn = false;
    }

    private IEnumerator FirstTurn()
    {
        yield return new WaitForSeconds(1f);

        // Draw 5 cards at start
        for (int i = 0; i < 5; i++)
        {
            var view = CardViewCreator.Instance.CreateCardView(transform.position, Quaternion.identity, handParent);
            view.CardData = testCardData;
            yield return handView.StartCoroutine(handView.AddCard(view));
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);
        DrawCard(); // Optional extra draw after setup
        firstturn = false;
    }

    private void DrawCard()
    {
        var view = CardViewCreator.Instance.CreateCardView(transform.position, Quaternion.identity, handParent);
        view.CardData = testCardData;
        StartCoroutine(handView.AddCard(view));
        drawnThisTurn = true;
        Debug.Log("Player drew a card!");
    }
}
