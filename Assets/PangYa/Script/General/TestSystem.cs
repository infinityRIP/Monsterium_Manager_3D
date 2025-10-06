using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandViewManager handView;
    [SerializeField] private Transform handParent; // set to HandViewManager.transform or Canva

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
        var cardView = CardViewCreator.Instance.CreateCardView(transform.position, Quaternion.identity, handParent);
        StartCoroutine(handView.AddCard(cardView));
        }
    }
}
