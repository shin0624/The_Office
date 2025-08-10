using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using static DataStructures;
public class EndingUIController : MonoBehaviour
{
    //DOTWeen 래핑 클래스를 통해 엔딩UI를 제어하는 기능을 담은 클래스. ui출력 메서드들이 TrueEndingTrigger에서 호출되고, 사용자 버튼 입력에 따라 ui 페이드 아웃 기능은 여기서 호출될 것.
    [Header("UI 요소들")]
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private CanvasGroup EndingCanvasPanel;
    [SerializeField] private Image endingImage;
    [SerializeField] private Image endingTitle;
    [SerializeField] private List<Button> endingButtons;//0-컬렉션 보기 / 1- 다시하기
    [SerializeField] private List<Sprite> endingTitleSprite;//0-true / 1-good / 2-bad
    [SerializeField] private List<Sprite> endingImageSprite;//0-true / 1-good / 2-bad
    [SerializeField] private GameObject endingMessageObject;
    [SerializeField] private TextMeshProUGUI endingMessage;

    [Header("BossType별 엔딩 컷신 SpriteSet")]
    [SerializeField] private EndingSpriteSet maleBossSprites;
    [SerializeField] private EndingSpriteSet femaleBossSprites;
    [SerializeField] private EndingSpriteSet youngBossSprites;
    private string currentBossType;//현재 선택된 bossType

    void Awake()
    {
        if (endingPanel!=null && endingPanel.activeSelf)
        {
            endingPanel.SetActive(false);
        }
    }
    void OnEnable()
    {
        endingButtons[0].onClick.AddListener(OnCollectionButtonClicked);
        endingButtons[1].onClick.AddListener(OnReplayButtonClicked);
        LoadCurrentBossType(); // 상사 타입 로드
    }

    private void ConfigureEndingSprites(EndingType endingType)// endingType과 bossType에 따른 스프라이트 설정 메서드
    {
        Sprite titleSprite = GetTitleSprite(endingType);// 타이틀 스프라이트 설정
    
        Sprite imageSprite = GetImageSprite(endingType, currentBossType);// 상사별 컷신 스프라이트 설정
        
        SetSprite(titleSprite, imageSprite);// 스프라이트 적용
    }

    private Sprite GetTitleSprite(EndingType endingType)// 엔딩 타입에 따른 타이틀 스프라이트를 반환하는 메서드.
    {
        switch (endingType)
        {
            case EndingType.True:
                return endingTitleSprite[0];
            case EndingType.Good:
                return endingTitleSprite[1];
            case EndingType.Bad:
                return endingTitleSprite[2];
            default:
                Debug.LogError($"[EndingUIController] 알 수 없는 엔딩 타입: {endingType}");
                return endingTitleSprite[0];
        }
    }

    private Sprite GetImageSprite(EndingType endingType, string bossType)// 상사 타입과 엔딩 타입에 따른 이미지 스프라이트를 반환하는 메서드
    {
        EndingSpriteSet spriteSet = GetSpriteSetByBossType(bossType);
        
        if (spriteSet == null)
        {
            Debug.LogError($"[EndingUIController] {bossType}에 대한 스프라이트 세트를 찾을 수 없습니다!");
            return null;
        }

        switch (endingType)
        {
            case EndingType.True:
                return spriteSet.trueEndingSprite;
            case EndingType.Good:
                return spriteSet.goodEndingSprite;
            case EndingType.Bad:
                return spriteSet.badEndingSprite;
            default:
                Debug.LogError($"[EndingUIController] 알 수 없는 엔딩 타입: {endingType}");
                return spriteSet.errorSprite;
        }
    }
    private EndingSpriteSet GetSpriteSetByBossType(string bossType)// 상사 타입에 따른 스프라이트 세트를 반환하는 메서드
    {
        switch (bossType)
        {
            case "male_boss":
                return maleBossSprites;
            case "female_boss":
                return femaleBossSprites;
            case "young_boss":
                return youngBossSprites;
            default:
                Debug.LogError($"[EndingUIController] 지원하지 않는 상사 타입: {bossType}");
                return maleBossSprites; // 기본값
        }
    }


    private void LoadCurrentBossType()//현재 선택된 상사 타입 로드하는 메서드.
    {
        currentBossType = PlayerPrefs.GetString("SelectedBoss", "male_boss");
        Debug.Log($"[EndingUIController] 현재 상사 타입 : {currentBossType}");
    }

    public void ShowEnding(EndingType endingType)//엔딩타입에 따라서 그에 맞는 이미지와 타이틀을 로드하는 메서드. TrueEndingTrigger.cs 에서 호출.
    {
        Debug.Log($"[EndingUIController] ShowEnding 호출됨: {endingType}");
        StartCoroutine(EndingSequence(endingType));
    }

    private IEnumerator EndingSequence(EndingType endingType)//250810. switch문을 각각 엔딩 타이틀, 엔딩 컷신 로드 메서드로 분리.
    {
        Debug.Log($"[EndingUIController] EndingSequence 시작: {endingType}");
        if (endingPanel != null && !endingPanel.activeSelf)
        {
            endingPanel.SetActive(true);
        }
        yield return new WaitForEndOfFrame();
        DotweenAnimations.FadeInBackground(EndingCanvasPanel);
        yield return new WaitForSeconds(0.5f);

       ConfigureEndingSprites(endingType);  // 기존 switch문을 ConfigureEndingSprites 메서드로 대체

        DotweenAnimations.FadeInEndingTitle(endingTitle);
        yield return new WaitForSeconds(1.0f);//duration 은 추후 테스트 봐서 조정

        DotweenAnimations.FadeInCutsceneImage(endingImage);
        yield return new WaitForSeconds(1.0f);//duration 은 추후 테스트 봐서 조정

        DotweenAnimations.FadeInEndingButtons(endingButtons);

        Debug.Log("[EndingUIController] 엔딩 시퀀스 시작");
    }

    private void OnReplayButtonClicked()//다시하기 버튼 클릭 시
    {
        DotweenAnimations.FadeOutEndingPanel(EndingCanvasPanel, 0.5f, () =>
        {
            FindAnyObjectByType<TrueEndingTrigger>()?.OnClickReplayOrNextBoss();//TrueEndingTrigger.cs의 트리거 리셋 메서드 호출. 데이터 초기화 및 트리거 변수 리셋.
            Debug.Log("[EndingUIController] 다시시작 버튼 클릭");

        });
        SceneManager.LoadScene("StartScene");
    }

    private void OnCollectionButtonClicked()//컬렉션 보기 버튼 클릭 시
    {
        Debug.Log("[EndingUIController] 컬렉션 버튼 클릭 - 사원수첩 씬으로 이동");
        DotweenAnimations.FadeOutEndingPanel(EndingCanvasPanel, 0.5f, () =>
        {
            SceneManager.LoadScene("CollectionScene"); // 사원수첩 씬으로 이동
        });
    }

    private void SetSprite(Sprite titleSprite, Sprite imageSprite)
    {
        if (titleSprite != null)
        {
            endingTitle.sprite = titleSprite;
        }
        else
        {
            Debug.LogWarning("[EndingUIController] 타이틀 스프라이트가 null입니다!");
        }

        if (imageSprite != null)
        {
            endingImage.sprite = imageSprite;
        }
        else
        {
            Debug.LogWarning("[EndingUIController] 이미지 스프라이트가 null입니다!");
        }
    }

    private void SetEndingMessage()//엔딩 시퀀스에서 엔딩 카드가 보여질 때 동시에 나타나는 엔딩메시지를 출력하는 메서드.
    {
        if (!endingMessageObject.activeSelf)
            endingMessageObject.SetActive(true);
        DotweenAnimations.FadeInBackground(endingMessageObject.GetComponent<CanvasGroup>());
    }


    void OnDisable()
    {
        endingButtons[0].onClick.RemoveAllListeners();
        endingButtons[1].onClick.RemoveAllListeners();
    }
}
