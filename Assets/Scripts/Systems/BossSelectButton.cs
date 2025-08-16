using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossSelectButton : MonoBehaviour
{
    //SelectScene에서 상사 버튼을 클릭하면 선택한 상사의 정보가 저장되고, MainScene으로 넘어가는 스크립트.

    [SerializeField] private Button maleBossButton;
    [SerializeField] private Button femaleBossButton;
    [SerializeField] private Button youngBossButton;

    public void Start()
    {
        maleBossButton.onClick.AddListener(() => OnSelectBoss("male_boss"));
        femaleBossButton.onClick.AddListener(() => OnSelectBoss("female_boss"));
        youngBossButton.onClick.AddListener(() => OnSelectBoss("young_boss"));
    }

    private void OnSelectBoss(string bossType)//선택한 상사에 따라 JSON에서 상사 타입을 구별할 KEY를 할당하는 메서드. 버튼 클릭 이벤트에 등록한다.
    {
        AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ButtonClick);
        PlayerPrefs.SetString("SelectedBoss", bossType);//Character_Data.json의 characters 내 male_boss, female_boss, young_boss값이 들어온다.
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainScene");
    }
}
