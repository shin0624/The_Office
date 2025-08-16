using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnums : MonoBehaviour
{
    // 각 오디오 타입을 정의하는 열거체 클래스
    public enum BGMType
    {
        StartScene,
        SelectScene,
        MainScene,
        CollectionScene
    }

    public enum SFXType
    {
        ButtonClick,
        PanelOpen,
        PanelClose,
        PanelSlide,
        ScoreUp,
        ScoreDown
    }
}
