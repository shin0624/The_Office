using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;

public class LoadingUI : MonoBehaviour
{
    //GameInitializer, SaveLoadManager가 StartScene에서 초기화 작업 + 저장 데이터 체크 작업을 수행함과 동시에 로딩 화면을 출력하는 기능을 수행하는 스크립트. 로딩이 끝나면 로딩패널이 비활성화되고, StartPanel이 보여짐. StartPanel은 상시 활성화상태(로딩패널에 가려서 안보이니까)
    [SerializeField] private Image progressCircle;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingText;//"로딩 중.."텍스트
    [SerializeField] private string[] loadingMessages = {
        "게임 데이터 로딩 중...",
        "저장 파일 확인 중...",
        "게임 매니저 초기화 중...",
        "준비 완료!"};
    private float rotationSpeed = 360.0f;//progressCircle 초당 회전 각도
    private float minLoadingTime = 2.0f;//최소 로딩 시간(너무 빨리 끝나지 않도록)
    private bool isLoading = false;
    private float loadingStartTime;
    private int currentMessageIndex = 0;
    public bool IsLoading => isLoading;//외부에서 로딩 상태를 확인할 수 있는 프로퍼티. 
    private static LoadingUI instance;
    public static LoadingUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<LoadingUI>();
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
    }

    void Start()
    {
        StartLoadingProgress();
    }

    void Update()
    {
        if (isLoading && progressCircle != null)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;//진행시간에 회전속도를 곱하면 progressCircle의 회전량을 구할 수 있음.
            progressCircle.transform.Rotate(0.0f, 0.0f, -rotationAmount);//progressCircle을 -z축으로 회전시켜 로딩 중임을 가시화.
        }
        if (isLoading && loadingText != null)
        {
            AnimateLoadingText();//로딩 텍스트 애니메이션 재생(점 깜빡임)
        }
    }

    private async void StartLoadingProgress()
    {
        isLoading = true;
        loadingStartTime = Time.time;

        Debug.Log("[LoadingUI] 로딩 시작");
        await PerformLoadingTasks();//로딩 작업들을 비동기로 병렬 실행
        Debug.Log("[LoadingUI] 로딩 완료");
        await EnsureMinimumLoadingTime();//최소 로딩 시간(2초) 보장
        CompleteLoading();//로딩 완료
    }

    private async Task PerformLoadingTasks()//실제 로딩 작업을 수행하는 비동기 메서드. 
    {
        try//추후 구글sdk, dotween 등 초기화 로직 추가
        {
            UpdateLoadingMessage(0);//"게임 데이터 로딩중.." - SaveLoadManager 초기화 대기
            await WaitForSaveLoadManagerInitialization();

            UpdateLoadingMessage(1);//"저장 파일 확인 중.." - 저장 데이터 확인
            await CheckSaveDataAsync();

            UpdateLoadingMessage(3);//"게임 매니저 초기화 중.." - 각 Manager들 초기화 대기
            await WaitForGameManagersInitialization();

            UpdateLoadingMessage(4);//"준비 완료" - 완료
            await Task.Delay(500);//0.5초 대기 후 시작
        }
        catch (Exception e)
        {
            Debug.LogError($"[LoadingUI] 로딩 중 오류 발생 : {e.Message}");
        }
    }

    private async Task WaitForSaveLoadManagerInitialization()//SaveLoadManager 초기화 대기 메서드
    {
        await Task.Run(async () =>
        {
            while (SaveLoadManager.Instance == null)//인스턴스 생성 시 까지 대기
            {
                await Task.Delay(50);//50ms마다 체크
            }
            await Task.Delay(500);//추가 초기화 시간
        });
    }
    private async Task CheckSaveDataAsync()//저장 데이터 확인 메서드
    {
        await Task.Run(async () =>
        {
            bool hasSaveData = SaveLoadManager.Instance?.HasSaveData() ?? false;//저장 데이터 존재 여부 확인.
            if (hasSaveData)
            {
                Debug.Log("[LoadingUI] 기존 저장 데이터 발견");
            }
            else
            {
                Debug.Log("[LoadingUI] 새 게임 준비");
            }
            await Task.Delay(400);
        });
    }
    private async Task WaitForGameManagersInitialization()// 게임매니저 초기화 대기 메서드.
    {
        await Task.Run(async () =>
        {
            while (ScoreManager.Instance == null)
            {
                await Task.Delay(50);
            }
            await Task.Delay(600);//GameInitializer 작업 완료 대기
        });
    }

    private async Task EnsureMinimumLoadingTime()//최소 로딩시간 보장 메서드
    {
        float elapsedTime = Time.time - loadingStartTime;//현재 시간 - 로딩 시작 시간으로 소요 시간을 계산
        if (elapsedTime < minLoadingTime)//소요시간이 최소 로딩시간보다 작을 때
        {
            float remaningTime = minLoadingTime - elapsedTime;//최소로딩시간 - 대기시간으로 딜레이할 시간을 구한다.
            await Task.Delay((int)(remaningTime * 1000));
        }
    }

    private void UpdateLoadingMessage(int messageIndex)//로딩 메세지 업데이트 메서드
    {
        if (loadingText != null && messageIndex < loadingMessages.Length)
        {
            loadingText.text = loadingMessages[messageIndex];
        }
    }

    private void AnimateLoadingText()//로딩 텍스트 애니메이션 메서드.
    {
        if (loadingText == null) return;

        float time = Time.time * 2.0f;//애니메이션 속도
        int dotCount = Mathf.FloorToInt(time) % 4;//0~3개의 점 출력

        string baseText = loadingMessages[Mathf.Min(currentMessageIndex, loadingMessages.Length - 1)];//현재 로딩메시지 번지와 로딩메시지 배열 길이를 비교하여 더 작은 쪽을 베이스텍스트로 가져옴
        if (baseText.EndsWith("..."))
        {
            baseText = baseText.Substring(0, baseText.Length - 1);//0부터 ...까지 자른다.

        }
        loadingText.text = baseText = new string('.', dotCount);//카운트만큼 '.'을 늘린다. 
    }

    private void CompleteLoading()//로딩 완료 처리 메서드
    {
        isLoading = false;
        if (loadingPanel != null)//로딩패널 비활성화
        {
            StartCoroutine(FadeOutLaodingPanel());
        }
    }

    private IEnumerator FadeOutLaodingPanel()//로딩패널 페이드아웃 효과 메서드
    {
        CanvasGroup canvasGroup = loadingPanel.GetComponent<CanvasGroup>();//alpha값을 조절하여 페이드아웃 효과를 주기 위해 캔버스 그룹 컴포넌트를 가져온다.
        if (canvasGroup == null)
        {
            canvasGroup = loadingPanel.AddComponent<CanvasGroup>();
        }
        float fadeTime = 0.5f;
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeTime);//1~0으로 서서히 페이드아웃.
            yield return null;
        }
        loadingPanel.SetActive(false);
    }

    void OnDestroy()
    {
        instance = null;
    }
}
