using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MoveObject : MonoBehaviour
{
    void Start()
    {
        DateTime d2 = new DateTime(2020, 04, 10, 4, 2, 20);
        string a = DateTime.Now.ToString();
        DateTime d1 = DateTime.Parse(a);
        Debug.Log(a);

        TimeSpan diff = d1 - d2;

        double dif = diff.TotalSeconds;

        Debug.Log(dif);
    }
}

