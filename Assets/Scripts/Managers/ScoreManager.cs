using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;

public enum EndingBranchType { Affection, Rank }// 엔딩 분기 타입 열거체 선언.

public class ScoreManager : MonoBehaviour
{
    //플레이어의 사회력점수와 상사 호감도를 관리하는 클래스. 게임의 핵심인 두 점수를 관리하고, 승진/엔딩 조건을 체크. 싱글톤으로 작성. 
    private static ScoreManager instance;
    public static ScoreManager Instance
    {
        get
        {
            // if (instance == null)
            // {
            //     GameObject managerObj = GameObject.Find("Managers");
            //     if (managerObj == null)
            //     {
            //         managerObj = new GameObject("Managers");
            //         DontDestroyOnLoad(managerObj);
            //     }

            //     instance = managerObj.GetComponent<ScoreManager>();
            //     if (instance == null)
            //     {
            //         instance = managerObj.AddComponent<ScoreManager>();
            //     }
            // }
            return instance;
        }
    }

    [Header("현재 게임 상태")]
    [SerializeField] private int affectionScore = 50;//현재 호감도
    [SerializeField] private int socialScore = 0;//현재 사회력 점수
    [SerializeField] private string currentRank = "인턴";// 현재 직급
    [SerializeField] private int currentDialogueId = 1;// 현재 대화 ID(진행 위치)

    private SaveData currentSaveData;//현재 게임의 저장 데이터
    private GameConfig gameConfig;//게임 설정 데이터
    private bool isInitialized = false;//초기화 완료 여부

    //이벤트 핸들러들. UI 업데이트 및 게임 상태 변화 알림을 위한 이벤트이며, 다른 스크립트들이 점수 변화를 실시간으로 감지할 수 있도록 옵저버 패턴으로 작성.
    public Action<int> OnAffectionChanged;//호감도 변경 이벤트
    public Action<int> OnSocialScoreChanged;//사회력 점수 변경 이벤트
    public Action<string> OnRankChanged;//직급 변경 이벤트
    public Action OnBadEnding;//베드 엔딩 이벤트
    public Action OnTrueEnding;//트루 엔딩 이벤트
    public Action<SaveData> OnGameDataLoaded;//게임 데이터 로드 이벤트
    public event Action<bool, EndingBranchType> OnEndingBranchChanged;// 엔딩 분기 액션
    public string rankTrue = "사장";
    public string rankEnableBranch = "차장";//Good/True엔딩 분기 가능 직급. 엔딩 분기 시점을 설정하지 않으면 게임 시작 직후 엔딩으로 분기할 수 있으므로, 이를 방지하기 위함
    private bool isEndingBranchEndabled = false;//Good/True엔딩 분기 플래그.

    //Getter 메서드들
    public int GetAffectionScore() => affectionScore;//호감도 반환
    public int GetSocialScore() => socialScore;//사회력 점수 반환
    public string GetCurrentRank() => currentRank;//현재 직급 반환
    public int GetCurrentDialogueId() => currentDialogueId;//현재 대화 ID 반환
    public SaveData GetCurrentSaveData() => currentSaveData;//현재 저장 데이터 반환

    //250813. affection, social 수치 변화 UI 시각화를 위한 이벤트 브로드캐스트 (ScoreManager는 싱글톤이라 MainScene을 벗어나면 스크립트 참조가 해제되어 NRE가 발생하므로, 수치변화 메서드가 작성된 ValueChangeEffect는 일반 참조할 수 없음.)
    public event Action<int, int> OnScoresChanged;// Action매개변수는 각각 affection, social이며, 점수 변경 시에만 ScoreManager가 이벤트를 발행하게 해서 MainScene 씬 활성화 시 ValueChangeEffect는 해당 이벤트를 구독, 비활성화 시 해제한다. -> 씬에 ui가 없으면 아무도 이벤트를 듣지 않으므로 NRE 발생X

    private void Awake()
    {
        // if (instance == null)
        // {
        //     instance = this;
        //     DontDestroyOnLoad(gameObject);//씬 전환 시에도 파괴되지 않도록 설정
        //     InitializeGame();//게임 초기화
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
        DontDestroyOnLoad(gameObject);
        InitializeGame();//게임 초기화
    }

    private void Start()
    {
        CheckEndingBranchValidity();//엔딩 브랜치 가능 여부 체크   
    }

    private void InitializeGame()//게임 초기화 메서드. 게임 설정과 저장 데이터를 로드하고, 초기 상태를 설정한다.
    {
        StartCoroutine(InitializeAfterFrame());// SaveLoadManger가 초기화된 후에 실행되도록 코루틴으로 처리.
    }

    private IEnumerator InitializeAfterFrame()//한 프레임 대기 후 게임 초기화를 수행하는 코루틴. SaveLoadManager가 먼저 초기화되도록 보장.
    {
        yield return new WaitForEndOfFrame();// 한 프레임 대기

        while (SaveLoadManager.Instance == null)
        {
            yield return null;// SaveLoadManager가 초기화될 때까지 대기
        }

        while (!SaveLoadManager.Instance.IsConfigLoaded())//250728 의존성 수정 : SaveLoadManager의 gameConfig 로드 완료까지 대기.
        {
            yield return new WaitForSeconds(0.1f);
        }

        gameConfig = SaveLoadManager.Instance.GetGameConfig();//게임 설정 로드
        LoadOrCreateGameData();//게임 시작 시 저장데이터 확인 및 초기화. 저장 데이터 존재 여부에 따라 분기 처리한다.
        isInitialized = true;//초기화 완료 플래그 설정
    }

    private void LoadOrCreateGameData()//저장데이터가 존재하면 로드하고, 없으면 기본값으로 초기화 하는 메서드.
    {
        if (SaveLoadManager.Instance.HasSaveData())//저장 데이터가 존재하면
        {
            Debug.Log("[ScoreManager] 기존 저장 데이터 발견, 로드 중...");
            LoadGameData();//저장 데이터 로드
        }
        else
        {
            Debug.Log("[ScoreManager] 저장 데이터 없음, 새 게임 시작");
            CreateNewGame();
        }
    }

    private void LoadGameData()//저장된 게임 데이터를 로드하는 메서드.
    {
        currentSaveData = SaveLoadManager.Instance.LoadSaveData();//저장 데이터 로드. 이후 저장된 데이터를 현재 상태에 덮어씌운다.
        affectionScore = currentSaveData.player_data.affection_level;//호감도 초기화
        socialScore = currentSaveData.player_data.social_score;//사회력 점수 초기화
        currentRank = currentSaveData.player_data.current_rank;//현재 직급 초기화
        currentDialogueId = currentSaveData.player_data.current_dialogue_id;//현재 대화 ID

        string calculatedRank = SaveLoadManager.Instance.GetRankNameByScore(socialScore);//랭크 검증 및 업데이트.
        if (currentRank != calculatedRank)//현재 직급과 검증을 시도한 직급이 다르면
        {
            currentRank = calculatedRank;//현재 직급을 검증된 직급으로 업데이트
            currentSaveData.player_data.current_rank = currentRank;//저장 데이터에도 반영        
        }
        //UI 업데이트 이벤트 호출
        OnAffectionChanged?.Invoke(affectionScore);//호감도 변경 이벤트 호출
        OnSocialScoreChanged?.Invoke(socialScore);//사회력 점수 변경 이벤트 호출
        OnRankChanged?.Invoke(currentRank);//직급 변경 이벤트 호출
        OnGameDataLoaded?.Invoke(currentSaveData);//게임 데이터 로드 이벤트 호출
        Debug.Log($"[ScoreManager] 데이터 로드 완료 - 호감도: {affectionScore}, 사회력: {socialScore}, 직급: {currentRank}");
    }

    public void CreateNewGame()//새 게임을 시작할 때 호출되는 메서드. 기본값으로 초기화하고 저장한다.
    {
        currentSaveData = SaveLoadManager.Instance.CreateDefaultSaveData();// 기본 저장 데이터 생성
        affectionScore = currentSaveData.player_data.affection_level;//기본 호감도
        socialScore = currentSaveData.player_data.social_score;//기본 사회력 점수
        currentRank = SaveLoadManager.Instance.GetRankNameByScore(socialScore);//기본 사회력 점수에 따른 직급 계산
        currentDialogueId = 1;//기본 대화 ID

        currentSaveData.player_data.current_rank = currentRank;//저장 데이터에 현재 직급 반영
        currentSaveData.player_data.current_dialogue_id = currentDialogueId;//저장 데이터에 현재 대화 ID 반영

        //UI 요소 업데이트 이벤트 호출 
        OnAffectionChanged?.Invoke(affectionScore);//호감도 변경 이벤트 호출
        OnSocialScoreChanged?.Invoke(socialScore);//사회력 점수 변경 이벤트 호출
        OnRankChanged?.Invoke(currentRank);//직급 변경 이벤트 호출
        OnGameDataLoaded?.Invoke(currentSaveData);//게임 데이터 로드 이벤트 호출

        SaveGame();//초기화된 데이터를 자동 저장
        Debug.Log($"[ScoreManager] 새 게임 시작 - 호감도: {affectionScore}, 사회력: {socialScore}, 직급: {currentRank}");
    }

    public void UpdateScores(int affectionChange, int socialChange)//호감도와 사회력 점수 동시 업데이트를 수행하는 메서드. 자동 저장 설정을 확인 후 점수가 바뀔 때 마다 자동 저장한다.
    {
        AddAffection(affectionChange);//호감도 업데이트
        AddSocialScore(socialChange);//사회력 점수 업데이트

        OnScoresChanged?.Invoke(affectionChange, socialChange);//250813. 점수 변화 시에만 구독하는 점수 증감치 시각화 이벤트.

        if (currentSaveData.game_settings.auto_save_enabled)//자동 저장이 활성화되어 있으면
        {
            SaveGame();//자동 저장 활성화 시 점수 변경 후 자동 저장
        }
    }

    public void AddAffection(int value)//호감도를 증가시키는 메서드.
    {
        int oldScore = affectionScore;//이전 호감도 저장
        affectionScore = Mathf.Clamp(affectionScore + value, 0, 100);//호감도 범위(0~100) 내에서 증가
        if (oldScore != affectionScore)//호감도 변경 시
        {
            if (oldScore > affectionScore)// 호감도 감소 시 진동 기능 호출
            {
                HapticUX.Vibrate(500);
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ScoreDown);//점수 다운 효과음 재생 
            }
            else
            {
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ScoreUp);//점수 업 효과음 재생
            }
            currentSaveData.player_data.affection_level = affectionScore;//저장 데이터에 반영.
            OnAffectionChanged?.Invoke(affectionScore);//호감도 변경 이벤트 호출
            CheckEndingBranchValidity();//엔딩 분기 플래그 체크
            CheckEndingConditions();//호감도에 따른 엔딩 조건 체크
        }
    }

    public void AddSocialScore(int value)//사회력 점수를 증가시키는 메서드.
    {
        int oldScore = socialScore;//이전 사회력 점수 저장
        socialScore = Mathf.Clamp(socialScore + value, 0, 300);//사회력 점수 범위(0~300) 내에서 증가
        if (oldScore != socialScore)//사회력 점수 변경 시
        {
            if (oldScore > socialScore)//사회력 점수 감소 시 진동 기능 호출
            {
                HapticUX.Vibrate(500);
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ScoreDown);//점수 다운 효과음 재생 
            }
            else
            {
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ScoreUp);//점수 업 효과음 재생
            }
            currentSaveData.player_data.social_score = socialScore;//저장 데이터에 반영.
            OnSocialScoreChanged?.Invoke(socialScore);//사회력 점수 변경 이벤트 호출
            CheckRankUp();//직급 업데이트 체크
            CheckEndingBranchValidity();// 엔딩 분기 플래그 체크
        }
    }

    private void CheckEndingConditions()//호감도에 따른 엔딩 조건을 체크하는 메서드. Game_Config.json의 임계값 기준으로 엔딩 조건을 체크한다. 엔딩 처리 로직 작성 중(250731)
    {
        if (gameConfig?.affection_thresholds == null) return;//호감도 임계값 정보가 없으면 반환.
        // if (affectionScore <= gameConfig.affection_thresholds.bad_ending)//호감도가 베드엔딩 임계값이면
        // {
        //     Debug.Log("[ScoreManager] 베드 엔딩");
        //     OnBadEnding?.Invoke();//베드엔딩 이벤트 호출
        //     //추가적인 엔딩 처리 로직 작성.
        // }
        // else if (affectionScore < gameConfig.affection_thresholds.true_ending && affectionScore >= gameConfig.affection_thresholds.low_threshold)//호감도 100 이하 && 호감도 30 이상 && 플레이어직급==사장 이면 True엔딩.
        // {
        //     Debug.Log("[ScoreManager] 트루 엔딩");
        //     OnTrueEnding?.Invoke();//트루엔딩 이벤트 호출
        //     //추가적인 엔딩 처리 로직 작성.
        // }
        
        if(!isEndingBranchEndabled)//분기 유효화 전에는 무조건 엔딩 체크 안함 --> 엔딩 플래그 설정 이후 부터 분기 가능.
            return;

        bool isTrueBranch = affectionScore >= gameConfig.affection_thresholds.low_threshold && affectionScore <= gameConfig.affection_thresholds.true_ending;//호감도 30 이상 100 이하 시 True 조건(1) 만족
        OnEndingBranchChanged?.Invoke(isTrueBranch, EndingBranchType.Affection);// isTrueBranch의 t/f값과 호감도 플래그를 전달.
    }

    private void CheckRankUp()//사회력 점수에 따른 직급 업데이트를 체크하는 메서드.
    {
        string newRank = SaveLoadManager.Instance.GetRankNameByScore(socialScore);//사회력 점수에 따른 직급 계산
        if (newRank != currentRank)//새로운 직급이 현재 직급과 다르면
        {
            string oldRank = currentRank;//이전 직급 저장
            currentRank = newRank;//현재 직급 업데이트
            currentSaveData.player_data.current_rank = currentRank;//저장 데이터에 반영

            OnRankChanged?.Invoke(currentRank);//직급 변경 이벤트 호출
            Debug.Log($"[ScoreManager] 직급 변경: {oldRank} -> {currentRank}");//직급 변경 로그 출력
        }

        if (!isEndingBranchEndabled)//분기 유효화 전에는 무조건 엔딩 체크 안함. --> 엔딩 플래그 설정 이후 부터 분기 가능
            return;
        bool isTrueBranch = currentRank == rankTrue;//현재 직급이 사장이 되면 True 조건(2) 만족
        OnEndingBranchChanged?.Invoke(isTrueBranch, EndingBranchType.Rank);//t/f값과 직급 플래그 전달.
    }

    private void SaveGame()//게임 데이터를 저장하는 메서드.
    {
        if (currentSaveData != null)
        {
            SaveLoadManager.Instance.SaveGameData(currentSaveData);//저장 데이터 저장
        }
    }

    public void SetCurrentDialogue(int dialogueId)//현재 대화 id를 설정하는 메서드. 대화 진행 상태를 저장.
    {
        currentDialogueId = dialogueId;//현재 대화 id 업데이트
        currentSaveData.player_data.current_dialogue_id = dialogueId;//저장 데이터에 반영
    }

    public void AddCompletedDialogue(int dialogueId)//완료된 대화 id를 추가하는 메서드. 대화 완료 상태를 저장.
    {
        if (!currentSaveData.player_data.completed_dialogues.Contains(dialogueId))//이미 완료된 대화가 아니면
        {
            currentSaveData.player_data.completed_dialogues.Add(dialogueId);//완료된 대화 목록에 추가
        }
    }

    public string GetAffectionLevel()//호감도 레벨을 반환하는 메서드. 호감도에 따라 레벨을 문자열로 반환한다.
    {
        if (gameConfig?.affection_thresholds == null) return "보통";//호감도 임계값 정보가 없으면 기본 "보통" 반환
        if (affectionScore < gameConfig.affection_thresholds.low_threshold) return "낮음";//낮은 호감도
        else if (affectionScore < gameConfig.affection_thresholds.true_ending) return "보통";//보통 호감도
        else return "높음";//높은 호감도
    }

    private void CheckEndingBranchValidity()//엔딩분기 플래그의 t/f를 결정하는 메서드.
    {
        string playerRank = GetCurrentRank();
        isEndingBranchEndabled = playerRank == rankEnableBranch;// 플레이어가 차장이 될 때 엔딩 분기 유효 플래그 = true가 될 것.
        Debug.Log($"엔딩 분기 플래그  = {isEndingBranchEndabled}");
    }

    public bool IsEndingBranchEnabled()
    {
        return isEndingBranchEndabled;
    }

}

