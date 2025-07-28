using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HapticUX
{
    //진동 등 UX 관련 기능을 정의한 스크립트.

// #if UNITY_ANDROID && !UNITY_EDITOR
//     public static AndroidJavaClass androidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");//유니티가 안드로이드에서 실행될 때의 메인 액티비티 클래스. c#에서 java 클래스에 직접 접근하는 브릿지 역할
//     public static AndroidJavaObject androidCurrentActivity = androidPlayer.GetStatic<AndroidJavaObject>("currentActivity");// 현재 안드로이드 앱에서 실행 중인 Activity 객체 획득. 모든 안드로이드 시스템 서비스 접근의 시작점.
//     public static AndroidJavaObject androidVibrator = androidCurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");//안드로이드의 시스템 진동 서비스에 접근. Android SDK의 API를 호출.
// #endif

    public static void Vibrate(long miliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    //androidVibrator.Call("Vibrate", miliseconds);
        Handheld.Vibrate();
#endif
    }

}
