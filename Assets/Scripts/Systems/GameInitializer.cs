using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;

public class GameInitializer : MonoBehaviour
{
    //게임 시작 시 필요한 모든 매니저들을 초기화하고 설정을 적용하는 스크립트.
    [Header("초기화 설정")]
    [SerializeField] private bool showDebugLogs = true; //디버그 로그 표시 여부

    private GameObject managersObject; //Managers 오브젝트 참조

    void Start()
    {
        InitializeGame();//게임 초기화 메서드 호출
    }

    private void InitializeGame()
    {
        if (showDebugLogs)
            Debug.Log("[GameInitializer] 게임 초기화 시작");

        CreateOrFindManagers();//Managers 오브젝트 생성 또는 찾기

        //각 매니저 강제 생성(접근 시 자동 생성됨.)
        var saveLoadManager = SaveLoadManager.Instance; //저장/로드 매니저 인스턴스 가져오기
        var scoreManager = ScoreManager.Instance; //점수 매니저 인스턴스 가져오기
        if (showDebugLogs)
        {
            Debug.Log("[GameInitializer] SaveLoadManager 초기화 완료");
            Debug.Log("[GameInitializer] ScoreManager 초기화 완료");
        }
        InitializeUI();
        StartCoroutine(InitializeAudioAfterDelay());//오디오 매니저 초기화는 딜레이 후 실행

        if (showDebugLogs)
            Debug.Log("[GameInitializer] 게임 초기화 완료");
    }

    private void CreateOrFindManagers()//Managers 오브젝트 생성 또는 찾기 메서드.
    {
        managersObject = GameObject.Find("Managers");
        if (managersObject == null)
        {
            managersObject = new GameObject("Managers");
            DontDestroyOnLoad(managersObject);
            
            if (showDebugLogs)
                Debug.Log("[GameInitializer] Managers 오브젝트 생성");
        }
        else
        {
            if (showDebugLogs)
                Debug.Log("[GameInitializer] 기존 Managers 오브젝트 발견");
        }
    }

    private void InitializeUI()//UI 매니저 초기화 메서드
    {
        
        var uiManager = FindAnyObjectByType<UIManager>();// UI 매니저가 있다면 초기화
        if (uiManager != null)
        {
            if (showDebugLogs)
                Debug.Log("[GameInitializer] UI 매니저 초기화");
        }
    }

    private IEnumerator InitializeAudioAfterDelay()//오디오 매니저 초기화 메서드
    {
        yield return new WaitForSeconds(1f);// ScoreManager가 완전히 초기화될 때까지 대기
        
        var saveData = ScoreManager.Instance.GetCurrentSaveData();
        if (saveData != null)
        {
            AudioListener.volume = saveData.game_settings.sound_volume;
            if (showDebugLogs)
                Debug.Log($"[GameInitializer] 오디오 볼륨 설정: {saveData.game_settings.sound_volume}");
        }
    }

}
