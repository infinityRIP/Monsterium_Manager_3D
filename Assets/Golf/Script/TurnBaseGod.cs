using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public class TurnBaseGod : MonoBehaviour
{
    private List<int> playerturn = new List<int>();
    EnemyturnbaseScript enemyturnbaseScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       enemyturnbaseScript = GetComponent<EnemyturnbaseScript>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
