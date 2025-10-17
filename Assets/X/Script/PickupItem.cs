using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemSO def;

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("Hit!!");
        var inv = other.GetComponentInParent<Inventory>();
        if (inv == null)
        {
            Debug.LogWarning("No inventory!!");
            return;
        }


        inv.Equip(def);
        Destroy(gameObject); // เก็บแล้วหาย
    }
}
