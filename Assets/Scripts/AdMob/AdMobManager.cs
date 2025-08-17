using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobManager : MonoBehaviour
{
    // ���� �ֵ�� ���鱤�� ���� ����� ������ ��ũ��Ʈ.
    [Header("General")]
    [SerializeField] private bool autoLoadOnStart = true;//Start���� �ڵ� �ε带 ������ �� ����
    [SerializeField] private bool showDebugLogs = true;//����� �α� ��� ����
    [SerializeField] private float showCooldownSeconds = 0.0f;//���鱤�� ���� ��ٿ�(��), 0�̸� ��Ȱ��

    [Header("Ad Unit Ids")]
    [SerializeField] private string androidInterstitialId = "ca-app-pub-5233935535970305/6588300861";//�ȵ���̵�� ���̵�
    [SerializeField] private string testInterstitialId = "ca-app-pub-3940256099942544/1033173712";//�׽�Ʈ ���̵�
    private InterstitialAd interstitial;// ���鱤�� �ν��Ͻ��� �����ϴ� ���� ��ü
    private bool sdkInitialized = false;//sdk �ʱ�ȭ �ϷῩ�� �÷���
    private bool isLoading = false;//���� �ε� ������ ǥ���ϴ� �÷���
    private float lastShowTime = -9999.0f;//������ ���� ǥ�� �ð�(��ٿ� ����) 
    public bool IsReady() => interstitial != null;//�ε� ���� ��ȯ
    public static AdMobManager Instance { get; private set; }//�̱��� �ν��Ͻ� ����

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);//�ߺ� ����
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);//�� ��ȯ �� ����.
    }

    void Start()//�ʱ�ȭ ������ : SDK �ʱ�ȭ
    {
        InitializeMobileAds();
        if (autoLoadOnStart)
            StartCoroutine(LoadWhenInitialized());
    }

    private void InitializeMobileAds()// ���� �ֵ�� sdk �ʱ�ȭ �޼���. �� ���� �� 1ȸ ȣ��
    {
        if (sdkInitialized) return;

        MobileAds.Initialize(_ =>
        {
            sdkInitialized = true;
            if (showDebugLogs) Debug.Log("[AdMob] Initialized.");
        });
    }

    private IEnumerator LoadWhenInitialized()//�ʱ�ȭ �ϷḦ ����ϰ� ���� �ε��ϴ� �޼���
    {
        while (!sdkInitialized) yield return null;
        RequestInterstitial();
    }

    public void RequestInterstitial()//���� ���� �ε带 ��û�ϴ� �޼���(�̹� �ε�/�ε�� �ν��Ͻ� ���� ����)
    {
        if (!sdkInitialized)//sdk ���ʱ�ȭ �� �ʱ�ȭ �� ��õ�
        {
            if (showDebugLogs)
                Debug.Log("[AdMob] SDK not initialized. Retry after init");
            InitializeMobileAds();
            StartCoroutine(LoadWhenInitialized());
            return;
        }

        if (isLoading)//�ߺ� �ε� ����
        {
            if (showDebugLogs)
                Debug.Log("[AdMob] Already loading");
            return;
        }

        if (interstitial != null)// ���� �ν��Ͻ� ����(�浹 ����)
        {
            interstitial.Destroy();
            interstitial = null;
        }

        string adUnitId = GetInterstitialAdUnitId();
        isLoading = true;

        AdRequest request = new AdRequest();

        InterstitialAd.Load(adUnitId, request, (ad, error) =>//�� �̺�Ʈ�� ���
        {
            isLoading = false;
            if (error != null || ad == null)
            {
                if (showDebugLogs)
                    Debug.LogWarning($"[AdMob] Load failed : {error}");//���� �� ���� üũ ����
                return;
            }
            interstitial = ad;

            interstitial.OnAdFullScreenContentOpened += () =>//���� ������
            {
                if (showDebugLogs)
                    Debug.Log("[AdMob] Opened");
            };

            interstitial.OnAdFullScreenContentClosed += () =>//������ ��(ǥ�� ���� ����) : ���� �� �����ε�
            {
                if (showDebugLogs)
                    Debug.Log("[AdMob] Closed");
                CleanUpAndPreload();
            };

            interstitial.OnAdFullScreenContentFailed += AdError =>//��üȭ�� ǥ�� ���� �� : ���� �� �����ε�
            {
                if (showDebugLogs)
                    Debug.LogWarning($"[AdMob] Show failed : {AdError}");
                CleanUpAndPreload();
            };

            interstitial.OnAdImpressionRecorded += () =>//��������/Ŭ�� �α�
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

    public bool CanShowNow()// ��ٿ� ���� ���� ���� ���� �Ǵ� �޼���
    {
        if (!IsReady()) return false;
        if (showCooldownSeconds <= 0.0f) return true;
        return (Time.unscaledTime - lastShowTime) >= showCooldownSeconds;
    }

    public void ShowIfReady()//�غ�/��ٿ� ���� ���� �˻� �� ���� ǥ���ϴ� �޼���. ������ �� �����ε� ����
    {
        if (!CanShowNow() || interstitial == null || !interstitial.CanShowAd())//�غ���� Ȯ��
        {
            if (showDebugLogs)
                Debug.Log("[AdMob] Not Ready or under cooldown. Preloading...");
            if (!isLoading && interstitial == null)
                RequestInterstitial();
            return;
        }
        lastShowTime = Time.unscaledTime;//��ٿ� Ÿ�̸� ����
        interstitial.Show();//���� ǥ��(����/���� �� �ݹ鿡�� ����/�����ε�)
    }

    private void CleanUpAndPreload()//���� �ν��Ͻ� ���� �� ���� ���� �̸� �ε��ϴ� �޼���
    {
        if (interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }
        RequestInterstitial();
    }

    private string GetInterstitialAdUnitId()//�÷��� �� ���� ����  ID�� ��ȯ�ϴ� �޼���. iOS�� ���� ���� ���� Ȯ�� �ȵ�. �ȵ���̵�� �׽�Ʈ��.
    {
#if UNITY_ANDROID
    return testInterstitialId;//�ϴ� �׽�Ʈ�� ���� ���� ����ũž ��� �׽�Ʈ���̵� �Ҵ�
#elif UNITY_EDITOR
    return testInterstitialId;
#endif
    }

    private void OnDestroy()//�ı� �� ���ҽ� ����.
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
