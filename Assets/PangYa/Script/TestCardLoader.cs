using UnityEngine;

public class TestCardLoader : MonoBehaviour
{
    public AllCardData initialCard;           // assign a ScriptableObject
    private CardDisplay display;

    void Awake()  { display = GetComponent<CardDisplay>(); }
    void Start()  { if (display && initialCard) display.CardData = initialCard; }

    // For changing cards via code:
    public void SetCard(AllCardData data) { if (display) display.CardData = data; }
}
