using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestScript : MonoBehaviour
{
    string _lastDate;

    // Start is called before the first frame update
    void Start(){
        DateTime lastDate = DateTime.UtcNow;

        _lastDate = lastDate.AddSeconds(1).ToString("yyyy-MM-dd hh:mm:ss");
        Debug.Log(_lastDate);
    }

    // Update is called once per frame
    void FixedUpdate(){
        int diffSecond = TT.zGetDateDiffCurrToSeconds(ref _lastDate);
        Debug.Log(diffSecond);

        DateTime d = DateTime.Parse(_lastDate);
        _lastDate = d.ToString();
    }
}
