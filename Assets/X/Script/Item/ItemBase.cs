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

    private InventoryManager inventoryManager;

    private void Awake()
    {
        inventoryManager = GameObject.Find("EventSystem").GetComponent<InventoryManager>();
        modifiers = item.modifiers;
        id = item.id;
        icon = item.icon;
        displayname = item.displayName;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("DD");
        if (other.gameObject.tag == "Player")
        {
            inventoryManager.Additem(id, displayname, icon , amount);
            Destroy(gameObject);
        }
    }
}
