using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Player Player;                        // ลากอ้างอิงใน Inspector
    private readonly List<ItemRuntime> equipped = new();

    private void Start()
    {
        Player = GetComponent<Player>();
    }
    public ItemRuntime Equip(ItemSO asset)
    {
        var rt = new ItemRuntime(asset);         // <— ไม่มี CS1729 แล้ว
        rt.Equip(Player);
        equipped.Add(rt);
        return rt;
    }

    public void Unequip(ItemRuntime rt)
    {
        if (equipped.Remove(rt))
            rt.Unequip(Player);
    }
}
