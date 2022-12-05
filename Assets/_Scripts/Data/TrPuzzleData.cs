using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrPuzzleData : MonoBehaviour
{

    static TrPuzzleData _instance;
    static public TrPuzzleData xInstance { get { return _instance; } }

    void Awake(){
        if (_instance == null){
            _instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void yResetDomainCodes()
    {
        _instance = null;
    }
}
