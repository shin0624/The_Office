using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using static DataStructures;
using System;
public class SaveLoadManager : MonoBehaviour
{
    //저장/로드 관리 스크립트. 게임 데이터의 저장, 로드, json 파일 관리를 담당하는 싱글톤 매니저로, 파일 시스템과의 모든 상호작용을 처리.

    private static SaveLoadManager instance;//싱글톤 인스턴스
    public static SaveLoadManager Instance//싱글톤 인스턴스 접근용 프로퍼티
    {
        get
        {
            if (instance == null)
            {
                GameObject managerObj = GameObject.Find("Managers");
                if (managerObj == null)
                {
                    managerObj = new GameObject("Managers");//Managers 오브젝트가 없으면 생성
                    DontDestroyOnLoad(managerObj);//씬 전환 시에도 파괴되지 않도록 설정
                }
                instance = managerObj.GetComponent<SaveLoadManager>();//Managers 오브젝트에서 SaveLoadManager 컴포넌트를 찾음
                if (instance == null)
                {
                    instance = managerObj.AddComponent<SaveLoadManager>();//컴포넌트가 없으면 추가
                }
            }
            return instance;//싱글톤 인스턴스 반환
        }
    }

    private string saveFilePath;//저장 파일 경로
    private string configFilePath;//설정 파일 경로
    private GameConfig gameConfig;//로드된 게임 설정 데이터

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);//씬 전환 시에도 파괴되지 않도록 설정
            InitializePaths();//저장 경로 초기화
            LoadGameConfig();//게임 설정 로드
        }
        else if (instance != this)
        {
            Destroy(gameObject);//중복 인스턴스 제거
        }
    }

    private void InitializePaths()//저장 및 설정 파일의 전체 경로를 설정하는 메서드. 플랫폼 별로 올바른 경로를 생성한다.
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "Save_Data.json");//영구 저장 경로. persistentDataPath는 플랫폼에 따라 다르게 설정됨. 
        configFilePath = Path.Combine(Application.streamingAssetsPath, "Game_Config.json");//스트리밍 에셋 폴더에서 설정 파일 경로. streamingAssetsPath는 빌드 시 읽기 전용으로 사용됨.
        Debug.Log($"[SaveLoadManager] 저장 경로: {saveFilePath}");
        Debug.Log($"[SaveLoadManager] 설정 파일 경로: {configFilePath}");
    }

    private void LoadGameConfig()//게임 설정 파일을 로드하는 메서드.
    {
        try
        {
            if (File.Exists(configFilePath))//설정 파일이 경로에 존재하면
            {
                string json = File.ReadAllText(configFilePath, Encoding.UTF8);//파일 내용을 읽어옴.
                gameConfig = JsonUtility.FromJson<GameConfig>(json);//JSON 문자열을 GameConfig 객체로 변환
                Debug.Log("[SaveLoadManager] Game_Config.json 로드 완료");
            }
            else
            {
                Debug.LogError("[SaveLoadManager] Game_Config.json 파일이 존재하지 않습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] Game_Config.json 로드 중 오류 발생: {e.Message}");//파일 읽기 오류 처리
            CreateDefaultGameConfig();//기본 게임 설정을 생성
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

    public bool HasSaveData()//저장 데이터 존재 여부를 확인하는 메서드.
    {
        return File.Exists(saveFilePath);//저장 파일이 존재하면 true 반환
    }

    public SaveData LoadSaveData()//Sabe_Data.json파일을 읽어서 SaveData 객체로 반환하는 메서드. 파일 손상 시 기본 데이터를 생성하도록 예외처리를 하고, utf8인코딩으로 한글 깨짐을 방지.
    {
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

}