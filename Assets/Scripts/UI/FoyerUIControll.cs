using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoyerUIControll : MonoBehaviour
{

    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingButton;
    //[SerializeField] private GameObject gameSettingUI;

    void OnEnable()
    {
        HapticUX.Vibrate(100);//�ްԽ� ui ȣ�� �� ���� ȿ��
        continueButton.onClick.AddListener(GameContinue);
        settingButton.onClick.AddListener(OpenSettingUI);
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
        continueButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
    }


}
