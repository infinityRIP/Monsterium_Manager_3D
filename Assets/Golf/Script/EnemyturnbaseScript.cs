using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EnemyturnbaseScript : MonoBehaviour
{
    public TestEnemyScriptable enemy;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
 

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attack()
    {
       StartCoroutine(Attackscript());
    }
    public void Takedamage()
    {
        

    }
    IEnumerator Attackscript() 
    {
        //Player TakeDamage
        Debug.Log("Enemy Attack");
        yield return new WaitForSeconds(2f);


    }
   
}
