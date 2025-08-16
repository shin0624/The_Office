using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnums : MonoBehaviour
{
    // �� ����� Ÿ���� �����ϴ� ����ü Ŭ����
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
