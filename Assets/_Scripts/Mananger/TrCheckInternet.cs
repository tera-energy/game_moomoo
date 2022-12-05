using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrCheckInternet : MonoBehaviour
{
    static TrCheckInternet _instance;
    public static TrCheckInternet xInstance { get { return _instance; } }
    public GameObject _goWinDisConnection;

    public static bool zGetIsConnectNetwork()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public void zActiveConnectWindow(bool isOpen)
    {
        _goWinDisConnection.SetActive(isOpen);
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!zGetIsConnectNetwork())
        {
            if (!_goWinDisConnection.activeSelf)
                zActiveConnectWindow(true);
        }
        else{
            if(_goWinDisConnection.activeSelf)
                zActiveConnectWindow(false);
        }
    }
}
