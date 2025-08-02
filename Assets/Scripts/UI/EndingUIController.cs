using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EndingUIController : MonoBehaviour
{
    //DOTWeen 래핑 클래스를 통해 엔딩UI를 제어하는 기능을 담은 클래스. ui출력 메서드들이 TrueEndingTrigger에서 호출되고, 사용자 버튼 입력에 따라 ui 페이드 아웃 기능은 여기서 호출될 것.
    [Header("UI 요소들")]
    [SerializeField] private CanvasGroup EndingCanvasPanel;
    [SerializeField] private List<Image> endingImage;//0-true / 1-good / 2-bad
    [SerializeField] private List<Image> endingTitle;//0-true / 1-good / 2-bad
    [SerializeField] private List<Button> endingButtons;//0-컬렉션 보기 / 1- 다시하기

    void OnEnable()
    {
        endingButtons[1].onClick.AddListener(OnReplayButtonClicked);
    }

    public void ShowEnding(EndingType endingType)//엔딩타입에 따라서 그에 맞는 이미지와 타이틀을 로드하는 메서드. TrueEndingTrigger.cs 에서 호출.
    {
        StartCoroutine(EndingSequence(endingType));
    }

    private IEnumerator EndingSequence(EndingType endingType)
    {
        DotweenAnimations.FadeInBackground(EndingCanvasPanel);
        yield return new WaitForSeconds(0.5f);

        switch (endingType)
        {
            case EndingType.True:
                Debug.Log("[EndingUIController] TrueEnding");
                DotweenAnimations.FadeInEndingTitle(endingTitle[0]);
                yield return new WaitForSeconds(1.0f);//duration 은 추후 테스트 봐서 조정
                DotweenAnimations.FadeInCutsceneImage(endingImage[0]);
                yield return new WaitForSeconds(1.0f);
                break;

            case EndingType.Good:
                Debug.Log("[EndingUIController] GoodEnding");
                DotweenAnimations.FadeInEndingTitle(endingTitle[1]);
                yield return new WaitForSeconds(1.0f);//duration 은 추후 테스트 봐서 조정
                DotweenAnimations.FadeInCutsceneImage(endingImage[1]);
                yield return new WaitForSeconds(1.0f);
                break;
            case EndingType.Bad:
                Debug.Log("[EndingUIController] BadEnding");
                DotweenAnimations.FadeInEndingTitle(endingTitle[2]);
                yield return new WaitForSeconds(1.0f);//duration 은 추후 테스트 봐서 조정
                DotweenAnimations.FadeInCutsceneImage(endingImage[2]);
                yield return new WaitForSeconds(1.0f);
                break;
        }
        DotweenAnimations.FadeInEndingButtons(endingButtons);
        Debug.Log("[EndingUIController] 엔딩 시퀀스 시작");
    }

    public void OnReplayButtonClicked()//다시하기 버튼 클릭 시
    {
        DotweenAnimations.FadeOutEndingPanel(EndingCanvasPanel, 0.5f, () =>
        {
            ScoreManager.Instance?.CreateNewGame();//데이터 초기화
            FindAnyObjectByType<TrueEndingTrigger>()?.ResetEndingTrigger();//TrueEndingTrigger.cs의 트리거 리셋 메서드 호출.
            Debug.Log("[EndingUIController] 다시시작 버튼 클릭");

        });
    }

    void OnDisable()
    {
        endingButtons[1].onClick.RemoveAllListeners();
    }

}
