using System.Collections.Generic;
using UnityEngine;

public class EnemyturnbaseScript : MonoBehaviour
{
    public ScriptableObject enemy;


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
    IEnumerator Attackscript() 
    {
        //Player TakeDamage
        Debug.Log("Enemy Attack");
        yield return new WaitForSeconds(2f);


    }
}
