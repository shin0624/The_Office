using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoToFoyer : MonoBehaviour
{
    //휴게실 버튼-UI 연동 스크립트
    [SerializeField] private Button foyerButton;
    [SerializeField] private GameObject foyerUI;

    void Awake()
    {
        foyerButton.onClick.AddListener(OnFoyerButtonClicked);
    }

    private void OnFoyerButtonClicked()
    {
        
        if(foyerUI.activeSelf==false)
        {
            AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ButtonClick);

            AdMobManager.Instance?.ShowIfReady();//250817. 휴게실 버튼 클릭 시 전면 광고 출력

            foyerUI.SetActive(true);

        }
    }

     void OnDisable()
     {
        foyerButton.onClick.RemoveAllListeners();
     }
}
