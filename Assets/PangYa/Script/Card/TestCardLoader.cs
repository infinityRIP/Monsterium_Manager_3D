using UnityEngine;

public class TestCardLoader : MonoBehaviour
{
    public AllCardData initialCard;          
    private CardDisplay display;

    void Awake()
    { display = GetComponent<CardDisplay>(); }
    void Start()
    { if (display && initialCard) display.CardData = initialCard; }


    public void SetCard(AllCardData data) { if (display) display.CardData = data; }
}
