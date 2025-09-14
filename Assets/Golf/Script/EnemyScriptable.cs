using UnityEngine;
[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/EnemyScriptableObject", order = 1)]
public class EnemyScriptable : ScriptableObject
{
    public GameObject enemyPrefab;
    [Header("Enemy Attributes")]
    public string enemyName;
    public int defense;
    public int defualtdefense;
    private int TotalHealth { get { return currentHealth; } set { currentHealth = Mathf.Clamp(value, 0, maxHealth);} }
    public int currentHealth;
    public int maxHealth;
    [SerializeField]int currentcost = 10;
    public int cost { get { return currentcost; } set { currentcost = Mathf.Clamp(value, 0, 10); } }

    void TakeDamage(int damage)
    {
        damage -= defense;
        TotalHealth -= damage;
        if (TotalHealth <= 0)
        {
            //die
        }
    }




}
