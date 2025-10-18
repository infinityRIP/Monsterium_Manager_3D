using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool isActiveted = false;
    public KeyCode inventory = KeyCode.Tab;

    public InventorySlot[] slot;

    void Start()
    {
        InventoryMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(inventory) && isActiveted)
        {
            Cursor.lockState = CursorLockMode.Locked;
            InventoryMenu.SetActive(false);
            isActiveted = false;
            closeDescription();
        }

        else if (Input.GetKeyDown(inventory) && !isActiveted)
        {
            Cursor.lockState = CursorLockMode.None;
            InventoryMenu.SetActive(true);
            isActiveted = true;
        }
    }
    public void Additem(string id, string name, Sprite icon,int amount, string description ,List<ItemStatSpec> modifiers, int maxNumberOfItem )
    {
        for (int i = 0; i < slot.Length; i++)
        {
            if (slot[i].isFull == false && slot[i].displayName == name || slot[i].amount == 0)
            {
                slot[i].Additem(id, name, icon, amount, description, modifiers, maxNumberOfItem);
                return;
            }
        }
    }

    public void DeselectedAllSlots()
    {
        for (int i = 0; i < slot.Length; i++)
        {
            slot[i].selectedShader.SetActive(false);
            slot[i].thisItemSelected = false;
        }
    }

    public void closeDescription()
    {
        for (int i = 0; i < slot.Length; i++)
        {
            slot[i].descriptionUI.SetActive(false);
        }
    }
}
