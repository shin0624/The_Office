using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossSelectButton : MonoBehaviour
{
    //SelectScene���� ��� ��ư�� Ŭ���ϸ� ������ ����� ������ ����ǰ�, MainScene���� �Ѿ�� ��ũ��Ʈ.

    [SerializeField] private Button maleBossButton;
    [SerializeField] private Button femaleBossButton;
    [SerializeField] private Button youngBossButton;

    public void Start()
    {
        maleBossButton.onClick.AddListener(() => OnSelectBoss("male_boss"));
        femaleBossButton.onClick.AddListener(() => OnSelectBoss("female_boss"));
        youngBossButton.onClick.AddListener(() => OnSelectBoss("young_boss"));
    }

    private void OnSelectBoss(string bossType)//������ ��翡 ���� JSON���� ��� Ÿ���� ������ KEY�� �Ҵ��ϴ� �޼���. ��ư Ŭ�� �̺�Ʈ�� ����Ѵ�.
    {
        AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ButtonClick);
        PlayerPrefs.SetString("SelectedBoss", bossType);//Character_Data.json�� characters �� male_boss, female_boss, young_boss���� ���´�.
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainScene");
    }
}
