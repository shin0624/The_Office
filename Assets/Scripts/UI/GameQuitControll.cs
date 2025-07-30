using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameQuitControll : MonoBehaviour
{
    //안드로이드의 뒤로가기 버튼 클릭 시 이벤트 제어를 수행하는 클래스. 뒤로가기는 어떤 Scene에서도 호출될 수 있으므로, static으로 선언.

    [SerializeField] private GameObject quitPanelPrefab;//게임 종료 패널 프리팹 
    [SerializeField] private Transform quitPanelParent;
    private float backButtonDoubleClickTime = 0.5f;//뒤로가기 버튼 더블클릭 인식 시간
    private float lastBackButtonClickedTime = 0.0f;
    private GameObject currentQuitPanel;
    private static GameQuitControll instance;
    public static GameQuitControll Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managerobj = GameObject.Find("Managers");
                if (managerobj == null)
                {
                    managerobj = new GameObject("Managers");//Managers 오브젝트가 없으면 생성
                    DontDestroyOnLoad(managerobj);//씬 전환 시에도 파괴되지 않도록 설정
                }
                instance = managerobj.GetComponent<GameQuitControll>();//Managers 오브젝트에서 GameQuitControll 컴포넌트를 찾음
                if (instance == null)
                {
                    instance = managerobj.AddComponent<GameQuitControll>();//컴포넌트가 없으면 추가
                }
            }
            return instance;
        }
    }

    void Update()
    {
        // if (Application.platform == RuntimePlatform.Android)
        // {
        //     if (Input.GetKeyDown(KeyCode.Escape))
        //     {
        //         HandleAndroidBackButton();
        //     }
        // }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleAndroidBackButton();
            }
        
    }

    private void HandleAndroidBackButton()//뒤로가기 버튼 클릭 횟수에 따라 종료 UI 이벤트를 다르게 수행하는 메서드. 
    {
        float currentTime = Time.time;
        float timeSinceLastClick = currentTime - lastBackButtonClickedTime;//마지막 뒤로가기 버튼 클릭 후 부터 소요된 시간
        if (timeSinceLastClick <= backButtonDoubleClickTime)//0.5 초 이내에 뒤로가기 두번 클릭 시
        {
            if (currentQuitPanel != null)//뒤로가기 더블클릭 감지 -> 기존 패널 제거 및 게임 재개
            {
                Destroy(currentQuitPanel);
                currentQuitPanel = null;
            }
        }
        else//첫 번째 뒤로가기 클릭 시
        {
            if (currentQuitPanel == null)
            {
                currentQuitPanel = Instantiate(quitPanelPrefab, quitPanelParent);
            }
        }
        lastBackButtonClickedTime = currentTime;
    }

    public void ClearCurrentPanel()// QuitPanel에서 호출되어 currentQuitPanel을 null로 설정하는 메서드.
    {
        Debug.Log("[GameQuitControll] QuitPanel 참조 제거");
        currentQuitPanel = null;
    }

    public bool IsQuitPanelActive()// 현재 QuitPanel이 활성화되어 있는지 확인하는 메서드.
    {
        return currentQuitPanel != null;
    }
}
