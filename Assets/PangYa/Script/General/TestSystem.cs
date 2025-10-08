using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandViewManager handView;
    [SerializeField] private Transform handParent;      // HandView or leave null
    [SerializeField] private AllCardData testCardData;  // <-- assign in Inspector

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var view = CardViewCreator.Instance.CreateCardView(transform.position, Quaternion.identity, handParent);
            view.CardData = testCardData;  // <- THIS IS THE IMPORTANT LINE
            StartCoroutine(handView.AddCard(view));
        }
    }
}
