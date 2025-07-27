using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuitPanel : MonoBehaviour
{
    // �ڷΰ��� ��ư Ŭ�� �� ��Ÿ���� UI�� �����ϴ� ��ũ��Ʈ.
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI exText;

    void OnEnable()
    {
        SetUpButtons();
    }
    void OnDisable()
    {
        RemoveButtons();
    }

    private void SetUpButtons()
    {
        quitButton.onClick.AddListener(() => OnQuitButtonClicked());
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void RemoveButtons()
    {
        quitButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
    }

    private IEnumerator OnQuitButtonClicked()
    {
        if (exText != null)
        {
            exText.text = "������ ���� ��...";
        }
        bool saveSuccess = SaveAllGameData();// SaveLoadManager�� ���� �۾� ����
        if (saveSuccess)
        {
            if (exText != null)
            {
                exText.text = "������ �������ϴ�. ������ �����մϴ�.";
            }
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            if (exText != null)
            {
                exText.text = "���� ����! �׷��� �����Ͻðڽ��ϱ�?";
            }

            yield return new WaitForSeconds(1.5f);
        }
        QuitApplication();
    }

    private void OnBackButtonClicked()
    {
        SetButtonsInteractable(false);
        StartCoroutine(ClosePanel());
    }

    private bool SaveAllGameData()// ��� ���� �����͸� �����ϴ� �޼���.SaveLoadManage.cs�� �޼��带 ���
    {
        if (SaveLoadManager.Instance == null)
        {
            Debug.LogError("[QuitPanel] SaveLoadManager �ν��Ͻ��� �����ϴ�!");
            return false;
        }

        return SaveLoadManager.Instance.SaveAllGameDataOnQuit();// SaveLoadManager�� ��� ���� ���� ����
    }

    private IEnumerator ClosePanel()//�г��� �ݰ� GameQuitController �� �˸��� �޼���.
    {
        NotifyQuitControllerPanelClosed();// GameQuitControll�� �г� ���� �˸�
        yield return null;
        Destroy(gameObject);// ���� ������Ʈ ����
    }

    private void NotifyQuitControllerPanelClosed()// GameQuitControll�� �г��� �������� �˸��� �޼���.
    {
        if (GameQuitControll.Instance != null)
        {
            GameQuitControll.Instance.ClearCurrentPanel(); // GameQuitControll�� currentQuitPanel�� null�� ����
        }
    }
    private void SetButtonsInteractable(bool interactable)// ��ư���� ��ȣ�ۿ� Ȱ��ȭ/��Ȱ��ȭ�ϴ� �޼���.
    {
        if (quitButton != null)
            quitButton.interactable = interactable;

        if (backButton != null)
            backButton.interactable = interactable;
    }

    private void QuitApplication()
    {
        Debug.Log("[QuitPanel] ���ø����̼� ����");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    
    void OnDestroy()
    {
        Debug.Log("[QuitPanel] QuitPanel ���ŵ�");
        NotifyQuitControllerPanelClosed();// �ı��� ���� GameQuitControll�� �˸�
    }

}
