using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobManager : MonoBehaviour
{
    // 구글 애드몹 전면광고 관련 기능을 정의한 스크립트.
    [Header("General")]
    [SerializeField] private bool autoLoadOnStart = true;//Start에서 자동 로드를 수행할 지 여부
    [SerializeField] private bool showDebugLogs = true;//디버그 로그 출력 여부
    [SerializeField] private float showCooldownSeconds = 0.0f;//전면광고 노출 쿨다운(초), 0이면 비활성

    [Header("Ad Unit Ids")]
    [SerializeField] private string androidInterstitialId = "ca-app-pub-5233935535970305/6588300861";//안드로이드용 아이디
    [SerializeField] private string testInterstitialId = "ca-app-pub-3940256099942544/1033173712";//테스트 아이디
    private InterstitialAd interstitial;// 전면광고 인스턴스를 보관하는 참조 객체
    private bool sdkInitialized = false;//sdk 초기화 완료여부 플래그
    private bool isLoading = false;//현재 로딩 중인지 표시하는 플래그
    private float lastShowTime = -9999.0f;//마지막 광고 표시 시각(쿨다운 계산용) 
    public bool IsReady() => interstitial != null;//로드 여부 반환
    public static AdMobManager Instance { get; private set; }//싱글톤 인스턴스 선언

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);//중복 방지
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);//씬 전환 시 유지.
    }

    void Start()//초기화 진입점 : SDK 초기화
    {
        InitializeMobileAds();
        if (autoLoadOnStart)
            StartCoroutine(LoadWhenInitialized());
    }

    private void InitializeMobileAds()// 구글 애드몹 sdk 초기화 메서드. 앱 시작 시 1회 호출
    {
        if (sdkInitialized) return;

        MobileAds.Initialize(_ =>
        {
            sdkInitialized = true;
            if (showDebugLogs) Debug.Log("[AdMob] Initialized.");
        });
    }

    private IEnumerator LoadWhenInitialized()//초기화 완료를 대기하고 광고를 로드하는 메서드
    {
        while (!sdkInitialized) yield return null;
        RequestInterstitial();
    }

    public void RequestInterstitial()//전면 광고 로드를 요청하는 메서드(이미 로딩/로드된 인스턴스 정리 포함)
    {
        if (!sdkInitialized)//sdk 미초기화 시 초기화 후 재시도
        {
            if (showDebugLogs)
                Debug.Log("[AdMob] SDK not initialized. Retry after init");
            InitializeMobileAds();
            StartCoroutine(LoadWhenInitialized());
            return;
        }

        if (isLoading)//중복 로드 방지
        {
            if (showDebugLogs)
                Debug.Log("[AdMob] Already loading");
            return;
        }

        if (interstitial != null)// 기존 인스턴스 정리(충돌 방지)
        {
            interstitial.Destroy();
            interstitial = null;
        }

        string adUnitId = GetInterstitialAdUnitId();
        isLoading = true;

        AdRequest request = new AdRequest();

        InterstitialAd.Load(adUnitId, request, (ad, error) =>//각 이벤트를 등록
        {
            isLoading = false;
            if (error != null || ad == null)
            {
                if (showDebugLogs)
                    Debug.LogWarning($"[AdMob] Load failed : {error}");//실패 시 에러 체크 로직
                return;
            }
            interstitial = ad;

            interstitial.OnAdFullScreenContentOpened += () =>//광고가 열릴때
            {
                if (showDebugLogs)
                    Debug.Log("[AdMob] Opened");
            };

            interstitial.OnAdFullScreenContentClosed += () =>//닫혔을 때(표시 종료 지점) : 정리 및 프리로드
            {
                if (showDebugLogs)
                    Debug.Log("[AdMob] Closed");
                CleanUpAndPreload();
            };

            interstitial.OnAdFullScreenContentFailed += AdError =>//전체화면 표시 실패 시 : 정리 및 프리로드
            {
                if (showDebugLogs)
                    Debug.LogWarning($"[AdMob] Show failed : {AdError}");
                CleanUpAndPreload();
            };

            interstitial.OnAdImpressionRecorded += () =>//임프레션/클릭 로그
            {
                if (showDebugLogs)
                    Debug.Log("[AdMob] Impression");
            };

            interstitial.OnAdClicked += () =>
            {
                if (showDebugLogs)
                    Debug.Log("[AdMob] Clicked");
            };
            if (showDebugLogs)
                Debug.Log("[AdMob] Interstitial Loaded");
        });
    }

    public bool CanShowNow()// 쿨다운 포함 노출 가능 여부 판단 메서드
    {
        if (!IsReady()) return false;
        if (showCooldownSeconds <= 0.0f) return true;
        return (Time.unscaledTime - lastShowTime) >= showCooldownSeconds;
    }

    public void ShowIfReady()//준비/쿨다운 충족 조건 검사 후 광고를 표시하는 메서드. 미충족 시 프리로드 예약
    {
        if (!CanShowNow() || interstitial == null || !interstitial.CanShowAd())//준비상태 확인
        {
            if (showDebugLogs)
                Debug.Log("[AdMob] Not Ready or under cooldown. Preloading...");
            if (!isLoading && interstitial == null)
                RequestInterstitial();
            return;
        }
        lastShowTime = Time.unscaledTime;//쿨다운 타이머 갱신
        interstitial.Show();//광고 표시(닫힘/실패 시 콜백에서 정리/프리로드)
    }

    private void CleanUpAndPreload()//현재 인스턴스 정리 후 다음 광고를 미리 로드하는 메서드
    {
        if (interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }
        RequestInterstitial();
    }

    private string GetInterstitialAdUnitId()//플랫폼 별 광고 단위  ID를 반환하는 메서드. iOS는 아직 할지 말지 확정 안됨. 안드로이드와 테스트만.
    {
#if UNITY_ANDROID
    return testInterstitialId;//일단 테스트를 위해 폰과 데스크탑 모두 테스트아이디 할당
#elif UNITY_EDITOR
    return testInterstitialId;
#endif
    }

    private void OnDestroy()//파괴 시 리소스 정리.
    {
        if (Instance == this)
            Instance = null;
        if (interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }
    }


}
