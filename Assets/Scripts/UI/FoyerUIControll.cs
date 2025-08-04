using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FoyerUIControll : MonoBehaviour
{

    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button collectionSceneButton;
    //[SerializeField] private GameObject gameSettingUI;

    void OnEnable()
    {
        HapticUX.Vibrate(100);//휴게실 ui 호출 시 진동 효과
        continueButton.onClick.AddListener(GameContinue);
        settingButton.onClick.AddListener(OpenSettingUI);
        collectionSceneButton.onClick.AddListener(OpenCollectionScene);
    }

    private void GameContinue()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private void OpenSettingUI()
    {

    }

    private void OpenCollectionScene()
    {
        SceneManager.LoadScene("CollectionScene");
    }


     void OnDisable()
     {
         continueButton.onClick.RemoveAllListeners();
         settingButton.onClick.RemoveAllListeners();
         collectionSceneButton.onClick.RemoveAllListeners();
     }


}
