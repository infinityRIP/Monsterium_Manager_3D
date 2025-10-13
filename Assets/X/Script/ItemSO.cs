using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum StatsType
{
    MaxHealth,
    Attack,
    Defense,
    Persuasion
}

[CreateAssetMenu(menuName = "Game/Item", fileName = "Item")]
public class ItemSO : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public StatsType type;


}
