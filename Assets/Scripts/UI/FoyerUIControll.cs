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
        HapticUX.Vibrate(100);//�ްԽ� ui ȣ�� �� ���� ȿ��
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
        //���� ��ó ��� : �ްԽ� �гο��� �÷��� ���� ��ư���� CollectionScene���� �̵��Ͽ��� ��.
        PlayerPrefs.SetString("CollectionEntrySource", "FromFoyerPanel");//250810. CollectionScene ������ 2��(EndingPanel-CollectionButton / MainScene-FoyerPanel) ���� ���� CollectionScene ���� �� �ڷΰ��� ��ư Ŭ�� �� �ٸ� ����� ����Ǿ�� �ϹǷ�, PlayerPrefs���� ���� ��ó�� ����ϰ�, �̸� �������� CollectionScene������ ���� �ൿ�� ����.
        PlayerPrefs.Save();//�÷��̾��������� ����.
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
