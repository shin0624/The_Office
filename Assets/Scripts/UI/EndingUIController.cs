using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using static DataStructures;
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
    [SerializeField] private GameObject endingMessageObject;
    [SerializeField] private TextMeshProUGUI endingMessage;

    [Header("BossType�� ���� �ƽ� SpriteSet")]
    [SerializeField] private EndingSpriteSet maleBossSprites;
    [SerializeField] private EndingSpriteSet femaleBossSprites;
    [SerializeField] private EndingSpriteSet youngBossSprites;
    private bool isExiting = false;//��ư ��Ÿ �� �ڷ�ƾ/Ʈ�� �ݹ��� �ߺ� ������ �����ϱ� ���� �÷���.
    private string currentBossType;//���� ���õ� bossType

    void Awake()
    {
        if (endingPanel!=null && endingPanel.activeSelf)
        {
            endingPanel.SetActive(false);
        }
    }
    void OnEnable()
    {
         if (endingButtons != null && endingButtons.Count >= 2)
        {
            endingButtons[0].onClick.AddListener(OnCollectionButtonClicked);
            endingButtons[1].onClick.AddListener(OnReplayButtonClicked);
        }
        else
        {
            Debug.LogError("[EndingUIController] endingButtons�� 2�� �̻� �Ҵ���� �ʾҽ��ϴ�!");
        }
        LoadCurrentBossType();
    }

    private void ConfigureEndingSprites(EndingType endingType)// endingType�� bossType�� ���� ��������Ʈ ���� �޼���
    {
        Sprite titleSprite = GetTitleSprite(endingType);// Ÿ��Ʋ ��������Ʈ ����

        Sprite imageSprite = GetImageSprite(endingType, currentBossType);// ��纰 �ƽ� ��������Ʈ ����
        string messageText = GetEndingMessage(endingType, currentBossType);//��� ���� �� �����޽��� ����

        SetSprite(titleSprite, imageSprite);// ��������Ʈ ����
        SetEndingMessageText(messageText);
    }

//----------���� �ƽ� ���� �޼���
    private Sprite GetTitleSprite(EndingType endingType)// ���� Ÿ�Կ� ���� Ÿ��Ʋ ��������Ʈ�� ��ȯ�ϴ� �޼���.
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
                Debug.LogError($"[EndingUIController] �� �� ���� ���� Ÿ��: {endingType}");
                return endingTitleSprite[0];
        }
    }

    private Sprite GetImageSprite(EndingType endingType, string bossType)// ��� Ÿ�԰� ���� Ÿ�Կ� ���� �̹��� ��������Ʈ�� ��ȯ�ϴ� �޼���
    {
        EndingSpriteSet spriteSet = GetSpriteSetByBossType(bossType);
        
        if (spriteSet == null)
        {
            Debug.LogError($"[EndingUIController] {bossType}�� ���� ��������Ʈ ��Ʈ�� ã�� �� �����ϴ�!");
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
                Debug.LogError($"[EndingUIController] �� �� ���� ���� Ÿ��: {endingType}");
                return spriteSet.errorSprite;
        }
    }
    private EndingSpriteSet GetSpriteSetByBossType(string bossType)// ��� Ÿ�Կ� ���� ��������Ʈ ��Ʈ�� ��ȯ�ϴ� �޼���
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
                Debug.LogError($"[EndingUIController] �������� �ʴ� ��� Ÿ��: {bossType}");
                return maleBossSprites; // �⺻��
        }
    }

    //----------���� �޽��� ���� �޼���

    private string GetEndingMessage(EndingType endingType, string bossType)// ��� Ÿ�԰� ���� Ÿ�Կ� ���� ���� �޽����� ��ȯ�ϴ� �޼���
    {
        EndingSpriteSet spriteSet = GetSpriteSetByBossType(bossType);
        if (spriteSet == null)
        {
            Debug.LogError($"[EndingUIController] {bossType}�� ���� ��������Ʈ ��Ʈ�� ã�� �� �����ϴ�!");
            return "���� �޽����� �ҷ��� �� �����ϴ�.";
        }

        switch (endingType)
        {
            case EndingType.True: 
                return string.IsNullOrEmpty(spriteSet.trueEndingMessage) ? "True ���� ����Ʈ �ؽ�Ʈ" : spriteSet.trueEndingMessage;

            case EndingType.Good: 
                return string.IsNullOrEmpty(spriteSet.goodEndingMessage) ? "Good ���� ����Ʈ �ؽ�Ʈ" : spriteSet.goodEndingMessage;

            case EndingType.Bad:  
                return string.IsNullOrEmpty(spriteSet.badEndingMessage) ? "Bad ���� ����Ʈ �ؽ�Ʈ" : spriteSet.badEndingMessage;

            default:
                Debug.LogError($"[EndingUIController] �� �� ���� ���� Ÿ��: {endingType}");
                return "EndingType Error : Please Report developer.";
        }
    }

    private void SetEndingMessageText(string messageText)// ���� �޽��� �ؽ�Ʈ ���� �޼���
    {
        if (endingMessage != null)
        {
            endingMessage.text = messageText;
        }
        else
        {
            Debug.LogWarning("[EndingUIController] endingMessage TextMeshPro�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }

    private void SetEndingMessage()//���� �޽��� ������Ʈ Ȱ��ȭ �޼���
    {
        if (endingMessageObject != null && !endingMessageObject.activeSelf)
        {
            endingMessageObject.SetActive(true);
        }

        var canvasGroup = endingMessageObject?.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            DotweenAnimations.FadeInBackground(canvasGroup);
        }
        else
        {
            Debug.LogWarning("[EndingUIController] endingMessageObject�� CanvasGroup�� �����ϴ�!");
        }
    }

//-----------------------------------

    private void LoadCurrentBossType()//���� ���õ� ��� Ÿ�� �ε��ϴ� �޼���.
    {
        currentBossType = PlayerPrefs.GetString("SelectedBoss", "male_boss");
        Debug.Log($"[EndingUIController] ���� ��� Ÿ�� : {currentBossType}");
    }

    public void ShowEnding(EndingType endingType)//����Ÿ�Կ� ���� �׿� �´� �̹����� Ÿ��Ʋ�� �ε��ϴ� �޼���. TrueEndingTrigger.cs ���� ȣ��.
    {
        Debug.Log($"[EndingUIController] ShowEnding ȣ���: {endingType}");
        StartCoroutine(EndingSequence(endingType));
    }

    private IEnumerator EndingSequence(EndingType endingType)//250810. switch���� ���� ���� Ÿ��Ʋ, ���� �ƽ� �ε� �޼���� �и�.
    {
        Debug.Log($"[EndingUIController] EndingSequence ����: {endingType}");
        if (endingPanel != null && !endingPanel.activeSelf)
        {
            endingPanel.SetActive(true);
        }
        yield return new WaitForEndOfFrame();
        DotweenAnimations.FadeInBackground(EndingCanvasPanel);
        yield return new WaitForSeconds(0.5f);

       ConfigureEndingSprites(endingType);  // ���� switch���� ConfigureEndingSprites �޼���� ��ü

        DotweenAnimations.FadeInEndingTitle(endingTitle);
        //yield return new WaitForSeconds(1.0f);//duration �� ���� �׽�Ʈ ���� ����

        DotweenAnimations.FadeInCutsceneImage(endingImage);
        //yield return new WaitForSeconds(1.0f);//duration �� ���� �׽�Ʈ ���� ����

        SetEndingMessage();//���� �޽��� ǥ��
        yield return new WaitForSeconds(0.2f);

        DotweenAnimations.FadeInEndingButtons(endingButtons);

        Debug.Log("[EndingUIController] ���� ������ �Ϸ�");
    }

    private void OnReplayButtonClicked()//�ٽ��ϱ� ��ư Ŭ�� ��
    {
        if (isExiting) return;
        isExiting = true;

        AdMobManager.Instance?.ShowIfReady();//250817. ���� �� �ٽ��ϱ� ��ư Ŭ�� �� ���� ���� ���

        SetButtonsInteractable(false);//�ߺ� Ŭ�� ����
        AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ButtonClick);

        DotweenAnimations.FadeOutEndingPanel(EndingCanvasPanel, 0.5f, () =>
        {
            FindAnyObjectByType<TrueEndingTrigger>()?.OnClickReplayOrNextBoss();//TrueEndingTrigger.cs�� Ʈ���� ���� �޼��� ȣ��. ������ �ʱ�ȭ �� Ʈ���� ���� ����.
            Debug.Log("[EndingUIController] �ٽý��� ��ư Ŭ��");
            SceneManager.LoadScene("StartScene");//�� �ε带 �ݹ� ������ ȣ��
        });
    }

    private void OnCollectionButtonClicked()//�÷��� ���� ��ư Ŭ�� ��
    {
        if (isExiting) return;
        isExiting = true;

        AdMobManager.Instance?.ShowIfReady();//250817. ���� �� �÷��� ���� ��ư Ŭ�� �� ���� ���� ���

        SetButtonsInteractable(false);//�ߺ� Ŭ�� ����
        AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ButtonClick);
        Debug.Log("[EndingUIController] �÷��� ��ư Ŭ�� - �����ø ������ �̵�");
    
        DotweenAnimations.FadeOutEndingPanel(EndingCanvasPanel, 0.5f, () =>
        {
            //���� ��ó ��� : ���� ���� �÷��� ���� ��ư���� CollectionScene���� �̵��Ͽ��� ��.
            PlayerPrefs.SetString("CollectionEntrySource", "FromEndingPanel");//250810. CollectionScene ������ 2��(EndingPanel-CollectionButton / MainScene-FoyerPanel) ���� ���� CollectionScene ���� �� �ڷΰ��� ��ư Ŭ�� �� �ٸ� ����� ����Ǿ�� �ϹǷ�, PlayerPrefs���� ���� ��ó�� ����ϰ�, �̸� �������� CollectionScene������ ���� �ൿ�� ����.
            PlayerPrefs.Save();//�÷��̾��������� ����.
            FindAnyObjectByType<TrueEndingTrigger>()?.OnClickReplayOrNextBoss();// ������ �� ���Ŀ��� ���� �����Ͱ� ��� �ʱ�ȭ�ǰ� �÷��� �����͸� �����Ǿ�� �ϹǷ� ���⼭�� ȣ��.
            SceneManager.LoadScene("CollectionScene"); // �����ø ������ �̵�
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
            Debug.LogWarning("[EndingUIController] Ÿ��Ʋ ��������Ʈ�� null�Դϴ�!");
        }

        if (imageSprite != null)
        {
            endingImage.sprite = imageSprite;
        }
        else
        {
            Debug.LogWarning("[EndingUIController] �̹��� ��������Ʈ�� null�Դϴ�!");
        }
    }

    private void SetButtonsInteractable(bool value)
    {
        if (endingButtons != null)
        {
            for (int i = 0; i < endingButtons.Count; i++)
            {
                if (endingButtons[i] != null) endingButtons[i].interactable = value;
            }
        }
    }


    void OnDisable()
    {
        if (endingButtons != null && endingButtons.Count >= 2)
        {
            endingButtons[0].onClick.RemoveAllListeners();
            endingButtons[1].onClick.RemoveAllListeners();
        }
    }
}
