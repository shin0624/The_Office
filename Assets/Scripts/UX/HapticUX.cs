using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HapticUX
{
    //���� �� UX ���� ����� ������ ��ũ��Ʈ.

// #if UNITY_ANDROID && !UNITY_EDITOR
//     public static AndroidJavaClass androidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");//����Ƽ�� �ȵ���̵忡�� ����� ���� ���� ��Ƽ��Ƽ Ŭ����. c#���� java Ŭ������ ���� �����ϴ� �긴�� ����
//     public static AndroidJavaObject androidCurrentActivity = androidPlayer.GetStatic<AndroidJavaObject>("currentActivity");// ���� �ȵ���̵� �ۿ��� ���� ���� Activity ��ü ȹ��. ��� �ȵ���̵� �ý��� ���� ������ ������.
//     public static AndroidJavaObject androidVibrator = androidCurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");//�ȵ���̵��� �ý��� ���� ���񽺿� ����. Android SDK�� API�� ȣ��.
// #endif

    public static void Vibrate(long miliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    //androidVibrator.Call("Vibrate", miliseconds);
        Handheld.Vibrate();
#endif
    }

}
