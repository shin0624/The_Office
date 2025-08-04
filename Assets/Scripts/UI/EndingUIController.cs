using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    }

    public void ShowEnding(EndingType endingType)//엔딩타입에 따라서 그에 맞는 이미지와 타이틀을 로드하는 메서드. TrueEndingTrigger.cs 에서 호출.
    {
        Debug.Log($"[EndingUIController] ShowEnding 호출됨: {endingType}");
        StartCoroutine(EndingSequence(endingType));
    }

    private IEnumerator EndingSequence(EndingType endingType)
    {
        Debug.Log($"[EndingUIController] EndingSequence 시작: {endingType}");
        if (endingPanel != null && !endingPanel.activeSelf)
        {
            endingPanel.SetActive(true);
        }
        yield return new WaitForEndOfFrame();
        DotweenAnimations.FadeInBackground(EndingCanvasPanel);
        yield return new WaitForSeconds(0.5f);

        switch (endingType)
        {
            case EndingType.True:
                Debug.Log("[EndingUIController] TrueEnding");
                SetSprite(endingTitleSprite[0], endingImageSprite[0]);
                break;

            case EndingType.Good:
                Debug.Log("[EndingUIController] GoodEnding");
                SetSprite(endingTitleSprite[1], endingImageSprite[1]);
                break;

            case EndingType.Bad:
                Debug.Log("[EndingUIController] BadEnding");
                SetSprite(endingTitleSprite[2], endingImageSprite[2]);
                break;
        }

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
        endingTitle.sprite = titleSprite;
        endingImage.sprite = imageSprite;
    }


    void OnDisable()
    {
        endingButtons[0].onClick.RemoveAllListeners();
        endingButtons[1].onClick.RemoveAllListeners();
    }
}
