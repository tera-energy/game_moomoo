using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSManager : MonoBehaviour
{
    public static double _firstLat; //최초 위도
    public static double _firstLong; //최초 경도
    public static double _currLat; //현재 위도
    public static double _currLong; //현재 경도

    private static WaitForSeconds _second;
    private static LocationInfo _location;

    static GPSManager _instance;
    public static GPSManager xInstance { get { return _instance; } }

    private static bool _gpsStart = false;

    private void Awake(){
        _second = TT.WaitForSeconds(1f);
    }
    IEnumerator Start(){
        _location = Input.location.lastData;
        _currLat = _location.latitude * 1.0d;
        _currLong = _location.longitude * 1.0d;


        if (!Input.location.isEnabledByUser)
        {
            yield break;
        }
        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return _second;
            maxWait -= 1;
        }
        if (maxWait < 1)
        {
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            yield break;
        }
        else
        {
            //gps접근허가 최초 위치
            _location = Input.location.lastData;
            _firstLat = _location.latitude * 1.0d;
            _firstLong = _location.longitude * 1.0d;
            _gpsStart = true;
            while (_gpsStart)
            {
                _location = Input.location.lastData;
                _currLat = _location.latitude * 1.0d;
                _currLong = _location.longitude * 1.0d;
                yield return _second;
            }
            

        }
    }  
    public static void StopGPS()
    {
        if (Input.location.isEnabledByUser)
        {
            _gpsStart = false;
            Input.location.Stop();
        }
    }
}
