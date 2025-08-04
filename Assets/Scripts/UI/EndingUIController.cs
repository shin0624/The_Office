using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingUIController : MonoBehaviour
{
    //DOTWeen ���� Ŭ������ ���� ����UI�� �����ϴ� ����� ���� Ŭ����. ui��� �޼������ TrueEndingTrigger���� ȣ��ǰ�, ����� ��ư �Է¿� ���� ui ���̵� �ƿ� ����� ���⼭ ȣ��� ��.
    [Header("UI ��ҵ�")]
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private CanvasGroup EndingCanvasPanel;
    [SerializeField] private Image endingImage;
    [SerializeField] private Image endingTitle;
    [SerializeField] private List<Button> endingButtons;//0-�÷��� ���� / 1- �ٽ��ϱ�
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

    public void ShowEnding(EndingType endingType)//����Ÿ�Կ� ���� �׿� �´� �̹����� Ÿ��Ʋ�� �ε��ϴ� �޼���. TrueEndingTrigger.cs ���� ȣ��.
    {
        Debug.Log($"[EndingUIController] ShowEnding ȣ���: {endingType}");
        StartCoroutine(EndingSequence(endingType));
    }

    private IEnumerator EndingSequence(EndingType endingType)
    {
        Debug.Log($"[EndingUIController] EndingSequence ����: {endingType}");
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
        yield return new WaitForSeconds(1.0f);//duration �� ���� �׽�Ʈ ���� ����

        DotweenAnimations.FadeInCutsceneImage(endingImage);
        yield return new WaitForSeconds(1.0f);//duration �� ���� �׽�Ʈ ���� ����

        DotweenAnimations.FadeInEndingButtons(endingButtons);

        Debug.Log("[EndingUIController] ���� ������ ����");
    }

    private void OnReplayButtonClicked()//�ٽ��ϱ� ��ư Ŭ�� ��
    {
        DotweenAnimations.FadeOutEndingPanel(EndingCanvasPanel, 0.5f, () =>
        {
            FindAnyObjectByType<TrueEndingTrigger>()?.OnClickReplayOrNextBoss();//TrueEndingTrigger.cs�� Ʈ���� ���� �޼��� ȣ��. ������ �ʱ�ȭ �� Ʈ���� ���� ����.
            Debug.Log("[EndingUIController] �ٽý��� ��ư Ŭ��");

        });
        SceneManager.LoadScene("StartScene");
    }

    private void OnCollectionButtonClicked()//�÷��� ���� ��ư Ŭ�� ��
    {
        Debug.Log("[EndingUIController] �÷��� ��ư Ŭ�� - �����ø ������ �̵�");
        DotweenAnimations.FadeOutEndingPanel(EndingCanvasPanel, 0.5f, () =>
        {
            SceneManager.LoadScene("CollectionScene"); // �����ø ������ �̵�
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
