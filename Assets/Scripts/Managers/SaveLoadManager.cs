using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using static DataStructures;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class SaveLoadManager : MonoBehaviour
{
    //저장/로드 관리 스크립트. 게임 데이터의 저장, 로드, json 파일 관리를 담당하는 싱글톤 매니저로, 파일 시스템과의 모든 상호작용을 처리.

    private static SaveLoadManager instance;//싱글톤 인스턴스
    public static SaveLoadManager Instance//싱글톤 인스턴스 접근용 프로퍼티
    {
        get
        {
            // if (instance == null)
            // {
            //     GameObject managerObj = GameObject.Find("Managers");
            //     if (managerObj == null)
            //     {
            //         managerObj = new GameObject("Managers");//Managers 오브젝트가 없으면 생성
            //         DontDestroyOnLoad(managerObj);//씬 전환 시에도 파괴되지 않도록 설정
            //     }
            //     instance = managerObj.GetComponent<SaveLoadManager>();//Managers 오브젝트에서 SaveLoadManager 컴포넌트를 찾음
            //     if (instance == null)
            //     {
            //         instance = managerObj.AddComponent<SaveLoadManager>();//컴포넌트가 없으면 추가
            //     }
            // }
            return instance;//싱글톤 인스턴스 반환
        }
    }

    private string saveFilePath;//저장 파일 경로
    private string configFilePath;//설정 파일 경로
    private GameConfig gameConfig;//로드된 게임 설정 데이터
    private bool configLoaded = false;

    public string beforeSceneName = "";

    // 250813. 긴급 저장 메서드 호출 구간 정의. Application.quitting, OnApplicationPause(true), OnApplicationFocus(false)일 때 긴급저장을 수행하도록 하고, 
    // EmergencyCheckManager.cs에서 StartScene 진입 시 LastQuitMethod == EmergencyQuit를 읽어서 이전 세션이 비정상 종료일 경우 비정상 종료 히스토리를 표시하도록 설정.
    private bool emergencySaved = false;//긴급저장 플래그

    private void Awake()
    {
        // if (instance == null)
        // {
        //     instance = this;
        //     DontDestroyOnLoad(gameObject);//씬 전환 시에도 파괴되지 않도록 설정
        //     InitializePaths();//저장 경로 초기화

        // }
        // else if (instance != this)
        // {
        //     Destroy(gameObject);//중복 인스턴스 제거
        // }

        if (instance != null && instance != this)
        {
            Destroy(gameObject);//중복 방지
            return;
        }
        instance = this;
        InitializePaths();//저장 경로 초기화
        DontDestroyOnLoad(gameObject);
        Application.quitting += OnAppQuitting;//긴급저장 이벤트 할당
       
    }

    void Start()
    {
        DotweenAnimations.DotweenInit();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        StartCoroutine(LoadGameConfigFix());
    }

    private void OnActiveSceneChanged(Scene scene1, Scene scene2)
    {
        beforeSceneName = scene1.name;
        Debug.Log($"[SaveLoadManager] 현재 씬 이름 : {beforeSceneName}");
    }

    private void InitializePaths()//저장 및 설정 파일의 전체 경로를 설정하는 메서드. 플랫폼 별로 올바른 경로를 생성한다.
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "Save_Data.json");//영구 저장 경로. persistentDataPath는 플랫폼에 따라 다르게 설정됨. 

        Debug.Log($"[SaveLoadManager] 저장 경로: {saveFilePath}");

    }

    private IEnumerator LoadGameConfigFix()//게임 설정 파일을 로드하는 메서드. UnityWebRequest 방식으로 수정
    {
        configFilePath = Path.Combine(Application.streamingAssetsPath, "Game_Config.json");//스트리밍 에셋 폴더에서 설정 파일 경로. streamingAssetsPath는 빌드 시 읽기 전용으로 사용됨.
        Debug.Log($"[SaveLoadManager] 설정 파일 경로: {configFilePath}");

        using (UnityWebRequest request = UnityWebRequest.Get(configFilePath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                gameConfig = JsonUtility.FromJson<GameConfig>(json);
                Debug.Log("[SaveLoadManager] Game_Config.json 로드 완료");
            }
            else
            {
                Debug.LogError("[SaveLoadManager] Game_Config.json 파일이 존재하지 않습니다.");
                CreateDefaultGameConfig();
            }
            configLoaded = true;
        }
    }

    private void CreateDefaultGameConfig()// 기본 게임 설정을 생성하는 메서드. 설정 파일이 없을 때 호출된다.
    {
        gameConfig = new GameConfig();// 새 GameConfig 객체 생성
        gameConfig.rank_system = new RankSystem();//직급 시스템 초기화
        gameConfig.rank_system.ranks = new List<RankInfo>()//직급 정보 리스트 초기화
        {
            new RankInfo { id = 1, name = "인턴", required_score = 0 },
            new RankInfo { id = 2, name = "사원", required_score = 30 },
            new RankInfo { id = 3, name = "주임", required_score = 40 },
            new RankInfo { id = 4, name = "대리", required_score = 50 },
            new RankInfo { id = 5, name = "과장", required_score = 60 },
            new RankInfo { id = 6, name = "차장", required_score = 70 },
            new RankInfo { id = 7, name = "부장", required_score = 80 },
            new RankInfo { id = 8, name = "이사", required_score = 100 },
            new RankInfo { id = 9, name = "상무", required_score = 150 },
            new RankInfo { id = 10, name = "전무", required_score = 190 },
            new RankInfo { id = 11, name = "부사장", required_score = 250 },
            new RankInfo { id = 12, name = "사장", required_score = 300 }
        };
        gameConfig.affection_thresholds = new AffectionThresholds();//호감도 임계값 초기화
    }

    public bool IsConfigLoaded()//게임 설정 데이터가 로드되었는지 여부를 확인하는 메서드.
    {
        return configLoaded;
    }

    public bool HasSaveData()//저장 데이터 존재 여부를 확인하는 메서드.
    {
        return File.Exists(saveFilePath);//저장 파일이 존재하면 true 반환
    }

    public SaveData LoadSaveData()//Sabe_Data.json파일을 읽어서 SaveData 객체로 반환하는 메서드. 파일 손상 시 기본 데이터를 생성하도록 예외처리를 하고, utf8인코딩으로 한글 깨짐을 방지.
    {
        //250728 : 이 메서드에서, 사용되는 saveFilePath는 persistentDataPath --> persistentDataPath는 앱 전용 외부 저장소로, APK 내부에 압축되어 저장되는 StreamingAssets와 다르게 외부에 일반 파일로 저장되기에 File.ReadAllText() 사용 가능.
        try
        {
            if (HasSaveData())//저장 데이터가 존재하면
            {
                string json = File.ReadAllText(saveFilePath, Encoding.UTF8);//파일 내용을 읽어온다.
                SaveData data = JsonUtility.FromJson<SaveData>(json);//JSON 문자열을 SaveData 객체로 변환
                Debug.Log("[SaveLoadManager] Save_Data.json 로드 완료");
                return data;//변환된 SaveData 객체 반환
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] Save_Data.json 로드 중 오류 발생: {e.Message}");//파일 읽기 오류 처리
        }
        return CreateDefaultSaveData();//저장 데이터가 없거나 오류 발생 시 기본 데이터를 생성하여 반환
    }

    public SaveData CreateDefaultSaveData()// 기본 저장 데이터를 생성하는 메서드. Save_Data.json이 없거나 손상된 경우 호출.
    {
        Debug.Log("[SaveLoadManager] 기본 저장 데이터 생성");
        SaveData defaultData = new SaveData();//새 SaveData 객체 생성
        defaultData.player_data.affection_level = 50;//기본 호감도 50
        defaultData.player_data.social_score = 0;//기본 사회력 점수 0
        defaultData.timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");//현재 시각을 저장
        return defaultData;//생성된 기본 SaveData 객체 반환
    }

    public void SaveGameData(SaveData data)// SaveData 객체를 json 형태로 저장하는 메서드.
    {
        try
        {
            data.timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");//저장 시각 업데이트
            string json = JsonUtility.ToJson(data, true);//SaveData 객체를 JSON 문자열로 변환
            File.WriteAllText(saveFilePath, json, Encoding.UTF8);//파일에 JSON 문자열을 UTF-8 인코딩으로 저장
            Debug.Log("[SaveLoadManager] Save_Data.json 저장 완료");//저장 완료 로그 출력
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] Save_Data.json 저장 중 오류 발생: {e.Message}");//파일 쓰기 오류 처리
        }
    }

    public GameConfig GetGameConfig()//게임 설정을 반환하는 메서드.
    {
        return gameConfig;//로드된 GameConfig 객체 반환
    }

    public string GetRankNameByScore(int socialScore)//사회력 점수에 따른 직급 명을 계산하는 메서드. Game_Config.json의 rank_system.ranks를 참조한다.
    {
        if (gameConfig?.rank_system?.ranks == null) return "인턴";//직급 정보가 없으면 기본 "인턴" 반환

        string rankName = "인턴";
        foreach (var rank in gameConfig.rank_system.ranks)//직급 정보 리스트를 순회
        {
            if (socialScore >= rank.required_score)//사회력 점수가 해당 직급의 요구 점수 이상이면
                rankName = rank.name;//직급 명을 업데이트
        }
        return rankName;//최종 직급 명 반환
    }

    public int GetRankIdByScore(int socialScore)//사회력 점수에 따른 직급 ID를 계산하는 메서드. Game_Config.json의 rank_system.ranks를 참조한다.
    {
        if (gameConfig?.rank_system?.ranks == null) return 1;//직급 정보가 없으면 기본 ID 1(인턴) 반환.
        int rankId = 1;//기본 직급 id
        foreach (var rank in gameConfig.rank_system.ranks)// 직급 정보 리스트를 순회.
        {
            if (socialScore >= rank.required_score)//사회력 점수가 해당 직급의 요구 점수 이상이면
            {
                rankId = rank.id;//직급 ID를 업데이트
            }
        }
        return rankId;//최종 직급 ID 반환
    }

    public void DeleteSaveData()//저장 데이터를 삭제하는 메서드.
    {
        try
        {
            if (HasSaveData())
            {
                File.Delete(saveFilePath);// 저장 파일 삭제.
                Debug.Log("[SaveLoadManager] Save_Data.json 삭제 완료");//삭제 완료 로그 출력
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] Save_Data.json 삭제 중 오류 발생: {e.Message}");//파일 삭제 오류 처리
        }
    }

    //--------------250727 추가 : 뒤로가기를 통한 게임 정상 종료 시 수행되는 데이터 저장 메서드
    public bool SaveAllGameDataOnQuit()//뒤로가기를 통한 정상 종료에서 호출되는 게임 데이터 및 시점 정보 저장 메서드.QuitPanel 스크립트에서 호출된다.
    {
        try
        {
            Debug.Log("[SaveLoadManager] 게임 종료 시 전체 데이터 저장 시작");

            // 1. ScoreManager 검증
            if (ScoreManager.Instance == null)
            {
                Debug.LogError("[SaveLoadManager] ScoreManager 인스턴스가 없습니다!");
                return false;
            }

            // 2. 현재 저장 데이터 획득
            var currentSaveData = ScoreManager.Instance.GetCurrentSaveData();
            if (currentSaveData == null)
            {
                Debug.LogError("[SaveLoadManager] 현재 저장 데이터가 없습니다!");
                return false;
            }

            // 3. 게임 종료 시점 추가 정보 업데이트
            UpdateGameStateOnQuit(currentSaveData);

            // 4. 메인 게임 데이터 저장
            SaveGameData(currentSaveData);

            // 5. 플레이 통계 저장
            SavePlayStatistics();

            Debug.Log("[SaveLoadManager] 게임 종료 시 데이터 저장 완료");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveLoadManager] 게임 종료 시 데이터 저장 실패: {e.Message}");
            return false;
        }
    }

    private void UpdateGameStateOnQuit(SaveData saveData)//게임 종료 시점의 추가 정보를 저장 데이터에 업데이트하는 메서드.
    {
        saveData.timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        if (saveData.game_settings != null)// 게임 설정에 종료 관련 정보 추가
        {
            PlayerPrefs.SetString("LastQuitMethod", "NormalQuit");
            PlayerPrefs.SetString("LastQuitTime", saveData.timestamp);
        }
        if (saveData.player_data != null)// ScoreManager 상태와 저장 데이터 동기화 확인
        {
            saveData.player_data.affection_level = ScoreManager.Instance.GetAffectionScore();
            saveData.player_data.social_score = ScoreManager.Instance.GetSocialScore();
            saveData.player_data.current_rank = ScoreManager.Instance.GetCurrentRank();
            saveData.player_data.current_dialogue_id = ScoreManager.Instance.GetCurrentDialogueId();
        }
    }

    private void SavePlayStatistics()//PlayerPrefs 전용 플레이 통계 정보 저장 메서드
    {
        PlayerPrefs.SetString("LastPlayTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));// 현재 시간 저장

        float currentSessionTime = Time.time;
        float totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f) + currentSessionTime;// 총 플레이 시간 저장
        PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);

        int quitCount = PlayerPrefs.GetInt("GameQuitCount", 0) + 1; // 게임 종료 횟수 저장
        PlayerPrefs.SetInt("GameQuitCount", quitCount);

        PlayerPrefs.Save();

        Debug.Log($"[SaveLoadManager] 플레이 통계 저장 - " + $"총 플레이 시간: {totalPlayTime:F1}초, 종료 횟수: {quitCount}");
    }

    //------------긴급 저장 관련 메서드

    private void EmergencySave()// 게임 강제 종료 또는 비정상 종료 시 응급 저장을 수행하는 메서드.
    {
        try// 긴급 저장은 I/O실패 가능성이 있으므로, try-catch를 사용해야 함.
        {
            Debug.Log("[SaveLoadManager] 응급 저장 실행");
            if (ScoreManager.Instance?.GetCurrentSaveData() != null)
            {
                var saveData = ScoreManager.Instance.GetCurrentSaveData();
                saveData.timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                SaveGameData(saveData);
                PlayerPrefs.SetString("LastQuitMethod", "EmergencyQuit");
                PlayerPrefs.Save();
                Debug.Log("[SaveLoadManager] 응급 저장 완료");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] 응급 저장 실패: {e.Message}");
        }
    }

    private void SafeEmergencySave()//EmergencySave()를 호출하고 긴급저장 플래그를 true로 변경하는 메서드.
    {
        if (emergencySaved) return;//이미 true일 경우 리턴.
        emergencySaved = true;
        EmergencySave();
    }

    void OnApplicationPause(bool pause)//홈 버튼, 전화 수신, 앱 전환 등 앱이 백그라운드로 내려갈 때 pauseStatus==true가 되며 호출되는 메서드.
    {
        if (pause) SafeEmergencySave();//pause == true이면 앱이 백그라운드로 내려갔다는 뜻.
    }

    void OnApplicationFocus(bool hasFocus)//다른 앱이 위로 올라오는 등 현재 앱이 포커스를 잃을 때 호출되는 메서드.
    {
        if (!hasFocus) SafeEmergencySave();//hasFocus == false이면 현재 앱이 포커스를 잃었다는 뜻.
    }

    private void OnAppQuitting()//앱이 종료되기 직전 호출되는 메서드.
    {
        SafeEmergencySave();
    }

    public bool WasLastQuitEmergency()
    {
        return PlayerPrefs.GetString("LastQuitMethod", string.Empty) == "EmergencyQuit";//이전 종료 방법이 긴급종료였다면 true 반환.
    }

    public void ClearLastQuitFlag()
    {
        PlayerPrefs.DeleteKey("LastQuitMethod");
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        Application.quitting -= OnAppQuitting;
    }
}