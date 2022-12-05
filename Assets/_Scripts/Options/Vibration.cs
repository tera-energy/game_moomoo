using System.Collections;
using UnityEngine;

#if PLATFORM_IOS
using UnityCoreHaptics;
#endif

public static class Vibration
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
    public static void Vibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate");
#endif
#if PLATFORM_IOS
        Vibrate(1000);
#endif
    }

    public static void Vibrate(float sec, int intensity=1, int sharpness=1)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        long ms = (long)(sec * 1000);
        AndroidVibrator.Call("vibrate", ms);
#endif
#if PLATFORM_IOS
        UnityCoreHapticsProxy.PlayContinuousHaptics(intensity, sharpness, sec);
#endif
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", pattern, repeat);
#else
        Handheld.Vibrate();
#endif
    }

    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("cancel");
#endif
    }

}