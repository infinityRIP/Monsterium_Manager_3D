using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public List<ItemStatSpec> modifiers;
    public string id;
    public string displayName;
    public Sprite icon;
    public int amount;
    public bool isFull;

    [SerializeField]
    private TMP_Text amountText;

    [SerializeField]
    private Image itemImg;

    public void Additem(string id, string name, Sprite icon, int amount)
    {
        this.id = id;
        this.displayName = name;
        this.icon = icon;
        this.amount = amount;
        isFull = true;

        amountText.text = amount.ToString();
    }
}
