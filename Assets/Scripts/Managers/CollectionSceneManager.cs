using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataStructures;

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
    [SerializeField] private float flipDuration = 0.5f;//넘기기 애니메이션 시간
    [SerializeField] private float flipAngle = -180.0f;//넘기기 회전 각도

    [Header("페이드 효과 설정")]
    [SerializeField] private float fadeInDuration = 0.8f;//페이드인 시간
    [SerializeField] private float fadeOutDuration = 0.5f;//페이드아웃 시간

    [Header("자동 진행 설정")]
    //[SerializeField] private bool autoFlipOnStart = true;//씬 진입 시 표지 자동 넘기기 플래그
    [SerializeField] private float autoFlipDelay = 0.5f;//자동 넘기기 지연시간

    [Header("상호작용 버튼")]
    [SerializeField] private Button backButton;//뒤로가기 버튼
    [SerializeField] private List<GameObject> cardList;//엔딩컬렉션 카드 리스트.
    //0-male_true, 1-male_good, 2-male_bad
    //3-female_true, 4-female_good, 5-female_bad
    //6-young-true, 7-young-good, 8-young_bad  


    private bool isAnimating = false;//애니메이션 진행 중 플래그
    private bool hasFlipped = false;//표지를 넘겼는지 여부

    public bool IsAnimating => isAnimating;
    public bool HasFliped => hasFlipped;
    void OnEnable()
    {
        InitializeScene();//씬 초기상태 설정
        SetUpButtons();
    }
    void Start()
    {
        StartCoroutine(AutoFlipCover());
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

    private void SetUpButtons()//뒤로가기 버튼 및 표지 버튼 설정. 
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private IEnumerator AutoFlipCover()//자동 표지 넘기기 코루틴 메서드.
    {
        yield return new WaitForSeconds(autoFlipDelay);
        if (hasFlipped==false && isAnimating==false)
        {
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

    private IEnumerator FlipCoverSequence()//표지 넘기기 시퀀스 메서드.
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
        Debug.Log("[CollectionScene] FlipCoverSequence통과");
        if (coverPanel != null)//표지 패널 비활성화. 
        {
            coverPanel.SetActive(false);
        }
        // if (cardList != null)//사원수첩 내 엔딩 카드 리스트 순차 등장.
        // {
        //     DotweenAnimations.ShowCollectionCardsSequentially(cardList);
        // }

        LoadAndShowCollectionCards();//컬렉션 데이터 로드 및 엔딩 카드 표시

        hasFlipped = true;
        isAnimating = false;
        Debug.Log("[CollectionScene] 표지 넘기기 애니메이션 완료");
    }

    private void LoadAndShowCollectionCards()//엔딩 카드를 로드하고 표시하는 메서드.
    {
        if (CollectionManager.Instance != null)
        {
            CollectionManager.Instance.LoadCollectionData();//컬렉션 데이터 로드
            var unlockedCards = CollectionManager.Instance.GetUnlockedCards();//해금된 카드 리스트를 가져온다.
            Debug.Log($"[CollectionScene] 잠금 해제된 카드 수: {unlockedCards.Count}/9");

            if (cardList != null && cardList.Count > 0)//DOTWeen 애니메이션을 사용한 카드 리스트 애니메이션 재생
            {
                UpdateCardVisibility(unlockedCards);//해금된 카드만 표시하도록 설정
                DotweenAnimations.ShowCollectionCardsSequentially(cardList);
            }
        }
    }

    private void UpdateCardVisibility(List<CollectionCard> unlockedCards)// 카드 표시 상태를 업데이트 하는 메서드.
    {
        for (int i = 0; i < cardList.Count; i++)//카드 리스트와 각 카드들에 대해 해금 상태를 확인한다.
        {
            var cardObj = cardList[i];
            if (cardObj != null)
            {
                bool isUnlocked = i < unlockedCards.Count;//해당 인덱스에 맞는 카드가 잠금 해제되었는지 확인 
                var image = cardObj.GetComponent<Image>();
                if (image != null)
                {
                    image.color = isUnlocked ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1.0f);//미해금된 카드는 어둡게 표시 
                }
                cardObj.SetActive(true);
            }
        }
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
        if (this != null && gameObject != null) // 안전 체크
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    private void RemoveButtons()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
        }
    }

    void OnDestroy()//DOTWeen 정리.
    {
        if (coverTransform != null)
            DotweenAnimations.KillTweensOnTransform(coverTransform);
    }


}
