using System.Collections.Generic;
using UnityEngine;

public enum StatsType
{
    MaxHealth,
    Attack,
    Defense,
    Persuasion
}

public enum ItemType
{
    Weapon,
    Equipment,
    Item,
}

[System.Serializable]
public struct ItemStatSpec
{
    public StatsType stat;             // dropdown enum ใน Inspector
    public StatModifierType mode;      // Flat / PercentAdd / PercentMult
    public float value;                // 10  หรือ 0.10f สำหรับ +10%
    public int orderOverride;     // ปล่อย -1 เพื่อใช้ค่า default ของ mode
}

[CreateAssetMenu(menuName = "Game/Item", fileName = "Item")]
public class ItemSO : ScriptableObject
{
    public Sprite icon;
    public string id;
    public string displayName;
    public int amount;
    public int maxNumberOfItem;
    [TextArea(3, 12)]public string description;


    [Header("Type")]
    public ItemType itemtype;
    public List<ItemStatSpec> modifiers = new();


    public void Use()
    {
        if (itemtype == ItemType.Item)
        {

        }
    }
    public void Equip()
    {
        switch (itemtype)
        {
            case ItemType.Weapon:



                break;
            case ItemType.Equipment:

                break;
        }
    }

    void Checkmodifiers()
    {
        for (int i = 0; i < modifiers.Count; i++)
        {

        }
    }

}
