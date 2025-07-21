using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoyerUIControll : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingButton;
    //[SerializeField] private GameObject gameSettingUI;

    void OnEnable()
    {
        quitButton.onClick.AddListener(GameQuit);
        continueButton.onClick.AddListener(GameContinue);
        settingButton.onClick.AddListener(OpenSettingUI);
    }

    private void GameQuit()
    {
        //종료 전 데이터 모두 저장 로직 추가
        Application.Quit();
    }

    private void GameContinue()
    {
        if (Time.timeScale != 1.0f && gameObject.activeSelf)
        {
            Time.timeScale = 1.0f; 
            gameObject.SetActive(false);
        }
    }

    private void OpenSettingUI()
    {

    }

    void OnDisable()
    {
        quitButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
    }


}
