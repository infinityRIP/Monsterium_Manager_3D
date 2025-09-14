
using System;
using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
public enum TurnBaseGodState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Win,
    GameOver
}
public class TurnBaseGod : MonoBehaviour
{
    public EnemyScriptable enemy;
    [Header("Spawnpos")]
    public Transform enemySpawnPos;
    public Transform playerSpawnPos;
    [Header("State")]
    TurnBaseGodState state;
    void Start()
    {
        enemy.cost = 3;
       state = TurnBaseGodState.Start;
       StartCoroutine(SetupBattle());
    }
    IEnumerator SetupBattle()
    {
        GameObject enemyincombat = Instantiate(enemy.enemyPrefab, enemySpawnPos);
        yield return new WaitForSeconds(1f);
        state = TurnBaseGodState.PlayerTurn;
        StartCoroutine(PlayerTurn());
    }
    IEnumerator PlayerTurn()
    {
        Debug.Log("Player Turn");
        yield return null;  

    }
    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);
        enemy.defense = enemy.defualtdefense;
        enemy.cost += 1;
        Debug.Log("Enemy Turn");
        yield return new WaitForSeconds(1f);
        StartCoroutine(Enemyattack());
        

    }
    public void Endturn()
    {
       if (state != TurnBaseGodState.PlayerTurn)
       {
            return;

       }

       state = TurnBaseGodState.EnemyTurn;
       StartCoroutine(EnemyTurn());

    }
    IEnumerator Enemyattack()
    {
        while (state == TurnBaseGodState.EnemyTurn)
        {
            int randomaction = UnityEngine.Random.Range(0, 10);
            if (enemy.cost > 0)
            {
                if (randomaction <= 1)
                {
                    Debug.Log("Endturn");
                    state = TurnBaseGodState.PlayerTurn;
                    StartCoroutine(PlayerTurn());

                }
                else if (randomaction <= 3 && randomaction > 1 && enemy.cost >= 1)
                {
                    Debug.Log("Defend");
                    enemy.defense += 2;
                    //run defend code


                }
                else if (randomaction > 3 && enemy.cost >= 2)
                {
                    Debug.Log("Enemy Attack");
                    enemy.cost -= 2;

                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
                state = TurnBaseGodState.PlayerTurn;
                StartCoroutine(PlayerTurn());
            }
            yield return new WaitForSeconds(5f);
        }
       
            
        
        
    }



}
