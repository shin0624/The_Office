using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private Button startButton;
    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ButtonClick);//��ư Ŭ�� ����
        SceneManager.LoadScene("SelectScene");
    }

}
