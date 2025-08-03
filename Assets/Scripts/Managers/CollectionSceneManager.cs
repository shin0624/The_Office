using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollectionSceneManager : MonoBehaviour
{
    //엔딩 컬렉션 및 도움말 등을 볼 수 있는 사원 수첩 씬 "CollectionScene" 의 연출 및 제어를 위한 스크립트.
    [Header("사원 수첩 패널 설정")]
    [SerializeField] private GameObject coverPanel;//표지
    [SerializeField] private GameObject contentPanel;//내지
    [SerializeField] private CanvasGroup coverCanvasGroup;
    [SerializeField] private CanvasGroup contentCanvasGroup;

    [Header("표지 넘기기 애니메이션 설정")]
    [SerializeField] private RectTransform coverTransform;//표지 트랜스폼
    [SerializeField] private float flipDuration = 1.5f;//넘기기 애니메이션 시간
    [SerializeField] private float flipAngle = -180.0f;//넘기기 회전 각도

    [Header("페이드 효과 설정")]
    [SerializeField] private float fadeInDuration = 0.8f;//페이드인 시간
    [SerializeField] private float fadeOutDuration = 0.5f;//페이드아웃 시간

    [Header("자동 진행 설정")]
    [SerializeField] private bool autoFlipOnStart = true;//씬 진입 시 표지 자동 넘기기 플래그
    [SerializeField] private float autoFlipDelay = 1.0f;//자동 넘기기 지연시간

    [Header("상호작용 버튼")]
    [SerializeField] private Button backButton;//뒤로가기 버튼
    [SerializeField] private List<GameObject> cardList;//엔딩컬렉션 카드 리스트. 0-true, 1-good, 2-bad

    private bool isAnimating = false;//애니메이션 진행 중 플래그
    private bool hasFlipped = false;//표지를 넘겼는지 여부

    public bool IsAnimating => isAnimating;
    public bool HasFliped => hasFlipped;

    void Start()
    {
        InitializeScene();//씬 초기상태 설정
        if (autoFlipOnStart)
        {
            StartCoroutine(AutoFlipCover());
        }
    }
    
 #if UNITY_ANDROID
    void Update()//안드로이드용 뒤로가기 버튼 처리
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isAnimating)
        {
            OnBackButtonClicked();
        }
    }
#endif

    void OnEnable()
    {
        SetUpButtons();
    }

    void OnDisable()
    {
        RemoveButtons();
    }

    private void InitializeScene()//씬 초기 상태 설정용 메서드.
    {
        Debug.Log("[CollectionScene] 사원수첩 씬 초기화");
        if (coverPanel != null)
        {
            coverPanel.SetActive(true);//표지 패널을 먼저 활성화
            if (coverCanvasGroup != null)
                coverCanvasGroup.alpha = 1.0f;

        }
        if (contentPanel != null)
        {
            contentPanel.SetActive(false);//내지 패널은 비활성화. 
            if (contentCanvasGroup != null)
                contentCanvasGroup.alpha = 0.0f;
        }
        if (coverTransform != null)//표지의 트랜스폼 초기 상태 설정. 
        {
            coverTransform.rotation = Quaternion.identity;
            coverTransform.localScale = Vector3.one;
        }
        hasFlipped = false;
        isAnimating = false;
    }

    private void SetUpButtons()//뒤로가기 버튼 및 표지 버튼 설정. 표지에 버튼 컴포넌트를 달아서, 표지 클릭 시 페이지 넘기기 애니메이션이 실행될 것.(자동 수동 모두 가능)
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        if (coverPanel != null)
        {
            Button coverButton = coverPanel.GetComponent<Button>();
            if (coverButton == null)
                coverButton = coverPanel.AddComponent<Button>();

            coverButton.onClick.AddListener(OnCoverClicked);
        }
    }

    private IEnumerator AutoFlipCover()//자동 표지 넘기기 코루틴 메서드.
    {
        yield return new WaitForSeconds(autoFlipDelay);
        if (!hasFlipped && !isAnimating)
        {
            FlipCoverToContents();
        }
    }

    private void OnCoverClicked()//표지 클릭 시 수동 넘기기
    {
        if (!hasFlipped && !isAnimating)
        {
            Debug.Log("[CollectionScene] 표지 수동 클릭 - 넘기기 시작");
            FlipCoverToContents();
        }
    }

    public void FlipCoverToContents()//표지에서 내지로 넘기는 메서드.
    {
        if (isAnimating || hasFlipped)
        {
            Debug.LogWarning("[CollectionScene] 이미 애니메이션 중이거나 넘김 완료 상태입니다.");
            return;
        }
        StartCoroutine(FlipCoverSequence());
    }

    private IEnumerator FlipCoverSequence()//수동 표지 넘기기 시퀀스 메서드.
    {
        isAnimating = true;
        Debug.Log("[CollectionScene] 표지 넘기기 애니메이션 시작");
        HapticUX.Vibrate(100);//진동 햅틱 피드백

        if (contentPanel != null && !contentPanel.activeSelf)//내지 패널 미리 활성화
        {
            contentPanel.SetActive(true);
        }
        var flipTween = DotweenAnimations.FlipBookCover(coverTransform, flipAngle, flipDuration);//표지 넘기기 애니메이션 실행.
        var fadeOutTween = DotweenAnimations.FadeOutBookCover(coverCanvasGroup, fadeOutDuration);//표지가 넘어가며 페이드아웃 실행
        var fadeInTween = DotweenAnimations.FadeInBookCover(contentCanvasGroup, fadeInDuration);//표지가 사라지면 내지가 페이드인 됨.

        yield return flipTween.WaitForCompletion();//애니메이션 완료 대기

        if (coverPanel != null)//표지 패널 비활성화. 
        {
            coverPanel.SetActive(false);
        }
        if (cardList != null)//사원수첩 내 엔딩 카드 리스트 순차 등장.
        {
            DotweenAnimations.ShowCollectionCardsSequentially(cardList);
        }
        hasFlipped = true;
        isAnimating = false;
        Debug.Log("[CollectionScene] 표지 넘기기 애니메이션 완료");
    }

    private void OnBackButtonClicked()//뒤로가기 메서드
    {
        Debug.Log("[CollectionScene] 뒤로가기 클릭 : 이전 씬으로 돌아가고 CollectionScene은 원래대로 초기화");
        HapticUX.Vibrate(50);
        StartCoroutine(ExitSceneWithFade());
    }

    private IEnumerator ExitSceneWithFade()// 뒤로가기 버튼 클릭 시 페이드아웃 후 이전 씬으로 돌아가는 메서드.
    {
        isAnimating = true;
        CanvasGroup activeCanvasGroup = hasFlipped ? contentCanvasGroup : coverCanvasGroup;//현재 활성화된 패널을 찾고 페이드아웃을 수행한다.
        if (activeCanvasGroup != null)
        {
            var fadeOutTween = DotweenAnimations.FadeOutCollectionScene(activeCanvasGroup, fadeOutDuration);
            yield return fadeOutTween.WaitForCompletion();
        }
        SceneManager.LoadScene("MainScene");
    }

    private void RemoveButtons()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
        }

        if (coverPanel != null)
        {
            Button coverButton = coverPanel.GetComponent<Button>();
            if (coverButton != null)
            {
                coverButton.onClick.RemoveAllListeners();
            }
        }
    }

    void OnDestroy()//DOTWeen 정리.
    {
        if (coverTransform != null)
            DotweenAnimations.KillTweensOnTransform(coverTransform);
    }


}
