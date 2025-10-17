using System.Collections.Generic;
using UnityEngine;


public enum StatsType
{
    MaxHealth,
    Attack,
    Defense,
    Persuasion
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
    public string id;
    public string displayName;
    public Sprite icon;
    public StatsType stat;            
    public StatModifierType mode;

    [Header("Stat Modifiers")]
    public List<ItemStatSpec> modifiers = new();
}
