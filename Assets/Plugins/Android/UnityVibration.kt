package com.example.unityvibration

import android.app.Activity
import android.os.Vibrator
import com.unity3d.player.UnityPlayer

class UnityVibration {
    fun Vibration(len : Long){
        val vibrator = UnityPlayer.currentActivity.getSystemService(Activity.VIBRATOR_SERVICE) as Vibrator

        vibrator.vibrate(len)
    }
}