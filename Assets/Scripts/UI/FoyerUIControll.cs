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
    [SerializeField] private GameObject gameSettingUI;
    [SerializeField] private Button BackCloseButton;

    void OnEnable()
    {
        gameSettingUI.SetActive(false);
        HapticUX.Vibrate(100);//휴게실 ui 호출 시 진동 효과
        continueButton.onClick.AddListener(GameContinue);
        settingButton.onClick.AddListener(OpenSettingUI);
        collectionSceneButton.onClick.AddListener(OpenCollectionScene);
    }

    private void GameContinue()
    {
        if (gameObject.activeSelf)
        {
            AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ButtonClick);
            gameObject.SetActive(false);
        }
    }

    private void OpenSettingUI()
    {
        AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ButtonClick);
        BackCloseButton.onClick.AddListener(CloseSettingUI);
        if (!gameSettingUI.activeSelf)
        {
            gameSettingUI.SetActive(true);
        }
    }

    private void OpenCollectionScene()
    {
        AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ButtonClick);
        //진입 출처 기록 : 휴게실 패널에서 컬렉션 보기 버튼으로 CollectionScene으로 이동하였을 때.
        PlayerPrefs.SetString("CollectionEntrySource", "FromFoyerPanel");//250810. CollectionScene 진입점 2개(EndingPanel-CollectionButton / MainScene-FoyerPanel) 별로 각각 CollectionScene 진입 후 뒤로가기 버튼 클릭 시 다른 결과가 도출되어야 하므로, PlayerPrefs에서 진입 출처를 기록하고, 이를 바탕으로 CollectionScene에서의 다음 행동을 연결.
        PlayerPrefs.Save();//플레이어프렙스에 저장.
        SceneManager.LoadScene("CollectionScene");
    }

    private void CloseSettingUI()
    {
        if (!gameSettingUI.activeSelf) return;
        AudioManager.Instance.PlaySFX(AudioEnums.SFXType.PanelClose);
        gameSettingUI.SetActive(false);
    }


    void OnDisable()
    {
        continueButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
        collectionSceneButton.onClick.RemoveAllListeners();
        BackCloseButton.onClick.RemoveAllListeners();
     }


}
