using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoToFoyer : MonoBehaviour
{
    //�ްԽ� ��ư-UI ���� ��ũ��Ʈ
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

            foyerUI.SetActive(true);

        }
    }

     void OnDisable()
     {
        foyerButton.onClick.RemoveAllListeners();
     }
}
