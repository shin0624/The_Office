using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EndingUIController : MonoBehaviour
{
    //DOTWeen ���� Ŭ������ ���� ����UI�� �����ϴ� ����� ���� Ŭ����. ui��� �޼������ TrueEndingTrigger���� ȣ��ǰ�, ����� ��ư �Է¿� ���� ui ���̵� �ƿ� ����� ���⼭ ȣ��� ��.
    [Header("UI ��ҵ�")]
    [SerializeField] private CanvasGroup EndingCanvasPanel;
    [SerializeField] private List<Image> endingImage;//0-true / 1-good / 2-bad
    [SerializeField] private List<Image> endingTitle;//0-true / 1-good / 2-bad
    [SerializeField] private List<Button> endingButtons;//0-�÷��� ���� / 1- �ٽ��ϱ�

    void OnEnable()
    {
        endingButtons[1].onClick.AddListener(OnReplayButtonClicked);
    }

    public void ShowEnding(EndingType endingType)//����Ÿ�Կ� ���� �׿� �´� �̹����� Ÿ��Ʋ�� �ε��ϴ� �޼���. TrueEndingTrigger.cs ���� ȣ��.
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
                yield return new WaitForSeconds(1.0f);//duration �� ���� �׽�Ʈ ���� ����
                DotweenAnimations.FadeInCutsceneImage(endingImage[0]);
                yield return new WaitForSeconds(1.0f);
                break;

            case EndingType.Good:
                Debug.Log("[EndingUIController] GoodEnding");
                DotweenAnimations.FadeInEndingTitle(endingTitle[1]);
                yield return new WaitForSeconds(1.0f);//duration �� ���� �׽�Ʈ ���� ����
                DotweenAnimations.FadeInCutsceneImage(endingImage[1]);
                yield return new WaitForSeconds(1.0f);
                break;
            case EndingType.Bad:
                Debug.Log("[EndingUIController] BadEnding");
                DotweenAnimations.FadeInEndingTitle(endingTitle[2]);
                yield return new WaitForSeconds(1.0f);//duration �� ���� �׽�Ʈ ���� ����
                DotweenAnimations.FadeInCutsceneImage(endingImage[2]);
                yield return new WaitForSeconds(1.0f);
                break;
        }
        DotweenAnimations.FadeInEndingButtons(endingButtons);
        Debug.Log("[EndingUIController] ���� ������ ����");
    }

    public void OnReplayButtonClicked()//�ٽ��ϱ� ��ư Ŭ�� ��
    {
        DotweenAnimations.FadeOutEndingPanel(EndingCanvasPanel, 0.5f, () =>
        {
            ScoreManager.Instance?.CreateNewGame();//������ �ʱ�ȭ
            FindAnyObjectByType<TrueEndingTrigger>()?.ResetEndingTrigger();//TrueEndingTrigger.cs�� Ʈ���� ���� �޼��� ȣ��.
            Debug.Log("[EndingUIController] �ٽý��� ��ư Ŭ��");

        });
    }

    void OnDisable()
    {
        endingButtons[1].onClick.RemoveAllListeners();
    }

}
