using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuitPanel : MonoBehaviour
{
    // 뒤로가기 버튼 클릭 시 나타나는 UI를 제어하는 스크립트.
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
            exText.text = "데이터 저장 중...";
        }
        bool saveSuccess = SaveAllGameData();// SaveLoadManager에 저장 작업 위임
        if (saveSuccess)
        {
            if (exText != null)
            {
                exText.text = "저장이 끝났습니다. 게임을 종료합니다.";
            }
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            if (exText != null)
            {
                exText.text = "저장 실패! 그래도 종료하시겠습니까?";
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

    private bool SaveAllGameData()// 모든 게임 데이터를 저장하는 메서드.SaveLoadManage.cs의 메서드를 사용
    {
        if (SaveLoadManager.Instance == null)
        {
            Debug.LogError("[QuitPanel] SaveLoadManager 인스턴스가 없습니다!");
            return false;
        }

        return SaveLoadManager.Instance.SaveAllGameDataOnQuit();// SaveLoadManager에 모든 저장 로직 위임
    }

    private IEnumerator ClosePanel()//패널을 닫고 GameQuitController 에 알리는 메서드.
    {
        NotifyQuitControllerPanelClosed();// GameQuitControll에 패널 제거 알림
        yield return null;
        Destroy(gameObject);// 게임 오브젝트 제거
    }

    private void NotifyQuitControllerPanelClosed()// GameQuitControll에 패널이 닫혔음을 알리는 메서드.
    {
        if (GameQuitControll.Instance != null)
        {
            GameQuitControll.Instance.ClearCurrentPanel(); // GameQuitControll의 currentQuitPanel을 null로 설정
        }
    }
    private void SetButtonsInteractable(bool interactable)// 버튼들의 상호작용 활성화/비활성화하는 메서드.
    {
        if (quitButton != null)
            quitButton.interactable = interactable;

        if (backButton != null)
            backButton.interactable = interactable;
    }

    private void QuitApplication()
    {
        Debug.Log("[QuitPanel] 애플리케이션 종료");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    
    void OnDestroy()
    {
        Debug.Log("[QuitPanel] QuitPanel 제거됨");
        NotifyQuitControllerPanelClosed();// 파괴될 때도 GameQuitControll에 알림
    }

}
