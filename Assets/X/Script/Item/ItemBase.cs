using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public ItemSO item;
    [SerializeField]
    private List<ItemStatSpec> modifiers;
    [SerializeField]
    private string id;
    [SerializeField]
    private string displayname;
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private int amount;
    [SerializeField]
    private string description;
    [SerializeField]
    private int maxNumberOfItem;

    private InventoryManager inventoryManager;

    private void Awake()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        modifiers = item.modifiers;
        id = item.id;
        icon = item.icon;
        displayname = item.displayName;
        description = item.description;
        maxNumberOfItem = item.maxNumberOfItem;
        amount = item.amount; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inventoryManager.Additem(id, displayname, icon , amount, description, modifiers, maxNumberOfItem);
            Destroy(gameObject);
        }
    }
}
