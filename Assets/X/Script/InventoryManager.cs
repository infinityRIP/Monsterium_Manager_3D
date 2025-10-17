using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool isActiveted = false;
    public KeyCode inventory = KeyCode.Tab;
    void Start()
    {
        InventoryMenu.SetActive(false);
    }

    void Update()
    {
            if (Input.GetKeyDown(inventory) && isActiveted)
        {
            InventoryMenu.SetActive(false);
            isActiveted = false;
        }

        else if (Input.GetKeyDown(inventory) && !isActiveted)
        {
            InventoryMenu.SetActive(true);
            isActiveted = true;
        }
    }
    public void Additem(string id, string name, Sprite icon,int amount)
    {
        Debug.Log("id = " + id + "name = " + name);
    }
}
