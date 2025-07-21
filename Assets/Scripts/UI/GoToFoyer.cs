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
        
        if(foyerUI.activeSelf==false && Time.timeScale != 0.0f)
        {
            foyerUI.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    void OnDestroy()
    {
        foyerUI.SetActive(false);
        Time.timeScale = 1.0f;
        foyerButton.onClick.RemoveAllListeners();
    }
}
