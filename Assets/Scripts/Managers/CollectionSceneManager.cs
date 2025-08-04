using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataStructures;

public class CollectionSceneManager : MonoBehaviour
{
    //���� �÷��� �� ���� ���� �� �� �ִ� ��� ��ø �� "CollectionScene" �� ���� �� ��� ���� ��ũ��Ʈ.
    [Header("��� ��ø �г� ����")]
    [SerializeField] private GameObject coverPanel;//ǥ��
    [SerializeField] private GameObject contentPanel;//����
    [SerializeField] private CanvasGroup coverCanvasGroup;
    [SerializeField] private CanvasGroup contentCanvasGroup;

    [Header("ǥ�� �ѱ�� �ִϸ��̼� ����")]
    [SerializeField] private RectTransform coverTransform;//ǥ�� Ʈ������
    [SerializeField] private float flipDuration = 0.5f;//�ѱ�� �ִϸ��̼� �ð�
    [SerializeField] private float flipAngle = -180.0f;//�ѱ�� ȸ�� ����

    [Header("���̵� ȿ�� ����")]
    [SerializeField] private float fadeInDuration = 0.8f;//���̵��� �ð�
    [SerializeField] private float fadeOutDuration = 0.5f;//���̵�ƿ� �ð�

    [Header("�ڵ� ���� ����")]
    //[SerializeField] private bool autoFlipOnStart = true;//�� ���� �� ǥ�� �ڵ� �ѱ�� �÷���
    [SerializeField] private float autoFlipDelay = 0.5f;//�ڵ� �ѱ�� �����ð�

    [Header("��ȣ�ۿ� ��ư")]
    [SerializeField] private Button backButton;//�ڷΰ��� ��ư
    [SerializeField] private List<GameObject> cardList;//�����÷��� ī�� ����Ʈ.
    //0-male_true, 1-male_good, 2-male_bad
    //3-female_true, 4-female_good, 5-female_bad
    //6-young-true, 7-young-good, 8-young_bad  


    private bool isAnimating = false;//�ִϸ��̼� ���� �� �÷���
    private bool hasFlipped = false;//ǥ���� �Ѱ���� ����

    public bool IsAnimating => isAnimating;
    public bool HasFliped => hasFlipped;
    void OnEnable()
    {
        InitializeScene();//�� �ʱ���� ����
        SetUpButtons();
    }
    void Start()
    {
        StartCoroutine(AutoFlipCover());
    }
    
 #if UNITY_ANDROID
    void Update()//�ȵ���̵�� �ڷΰ��� ��ư ó��
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

    private void InitializeScene()//�� �ʱ� ���� ������ �޼���.
    {
        Debug.Log("[CollectionScene] �����ø �� �ʱ�ȭ");
        if (coverPanel != null)
        {
            coverPanel.SetActive(true);//ǥ�� �г��� ���� Ȱ��ȭ
            if (coverCanvasGroup != null)
                coverCanvasGroup.alpha = 1.0f;
        }
        if (contentPanel != null)
        {
            contentPanel.SetActive(false);//���� �г��� ��Ȱ��ȭ. 
            if (contentCanvasGroup != null)
                contentCanvasGroup.alpha = 0.0f;
        }
        if (coverTransform != null)//ǥ���� Ʈ������ �ʱ� ���� ����. 
        {
            coverTransform.rotation = Quaternion.identity;
            coverTransform.localScale = Vector3.one;
        }
        hasFlipped = false;
        isAnimating = false;
    }

    private void SetUpButtons()//�ڷΰ��� ��ư �� ǥ�� ��ư ����. 
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private IEnumerator AutoFlipCover()//�ڵ� ǥ�� �ѱ�� �ڷ�ƾ �޼���.
    {
        yield return new WaitForSeconds(autoFlipDelay);
        if (hasFlipped==false && isAnimating==false)
        {
            FlipCoverToContents();
        }
    }

    public void FlipCoverToContents()//ǥ������ ������ �ѱ�� �޼���.
    {
        if (isAnimating || hasFlipped)
        {
            Debug.LogWarning("[CollectionScene] �̹� �ִϸ��̼� ���̰ų� �ѱ� �Ϸ� �����Դϴ�.");
            return;
        }
        StartCoroutine(FlipCoverSequence());
    }

    private IEnumerator FlipCoverSequence()//ǥ�� �ѱ�� ������ �޼���.
    {
        isAnimating = true;
        Debug.Log("[CollectionScene] ǥ�� �ѱ�� �ִϸ��̼� ����");
        HapticUX.Vibrate(100);//���� ��ƽ �ǵ��

        if (contentPanel != null && !contentPanel.activeSelf)//���� �г� �̸� Ȱ��ȭ
        {
            contentPanel.SetActive(true);
        }
        var flipTween = DotweenAnimations.FlipBookCover(coverTransform, flipAngle, flipDuration);//ǥ�� �ѱ�� �ִϸ��̼� ����.
        var fadeOutTween = DotweenAnimations.FadeOutBookCover(coverCanvasGroup, fadeOutDuration);//ǥ���� �Ѿ�� ���̵�ƿ� ����
        var fadeInTween = DotweenAnimations.FadeInBookCover(contentCanvasGroup, fadeInDuration);//ǥ���� ������� ������ ���̵��� ��.

        yield return flipTween.WaitForCompletion();//�ִϸ��̼� �Ϸ� ���
        Debug.Log("[CollectionScene] FlipCoverSequence���");
        if (coverPanel != null)//ǥ�� �г� ��Ȱ��ȭ. 
        {
            coverPanel.SetActive(false);
        }
        // if (cardList != null)//�����ø �� ���� ī�� ����Ʈ ���� ����.
        // {
        //     DotweenAnimations.ShowCollectionCardsSequentially(cardList);
        // }

        LoadAndShowCollectionCards();//�÷��� ������ �ε� �� ���� ī�� ǥ��

        hasFlipped = true;
        isAnimating = false;
        Debug.Log("[CollectionScene] ǥ�� �ѱ�� �ִϸ��̼� �Ϸ�");
    }

    private void LoadAndShowCollectionCards()//���� ī�带 �ε��ϰ� ǥ���ϴ� �޼���.
    {
        if (CollectionManager.Instance != null)
        {
            CollectionManager.Instance.LoadCollectionData();//�÷��� ������ �ε�
            var unlockedCards = CollectionManager.Instance.GetUnlockedCards();//�رݵ� ī�� ����Ʈ�� �����´�.
            Debug.Log($"[CollectionScene] ��� ������ ī�� ��: {unlockedCards.Count}/9");

            if (cardList != null && cardList.Count > 0)//DOTWeen �ִϸ��̼��� ����� ī�� ����Ʈ �ִϸ��̼� ���
            {
                UpdateCardVisibility(unlockedCards);//�رݵ� ī�常 ǥ���ϵ��� ����
                DotweenAnimations.ShowCollectionCardsSequentially(cardList);
            }
        }
    }

    private void UpdateCardVisibility(List<CollectionCard> unlockedCards)// ī�� ǥ�� ���¸� ������Ʈ �ϴ� �޼���.
    {
        for (int i = 0; i < cardList.Count; i++)//ī�� ����Ʈ�� �� ī��鿡 ���� �ر� ���¸� Ȯ���Ѵ�.
        {
            var cardObj = cardList[i];
            if (cardObj != null)
            {
                bool isUnlocked = i < unlockedCards.Count;//�ش� �ε����� �´� ī�尡 ��� �����Ǿ����� Ȯ�� 
                var image = cardObj.GetComponent<Image>();
                if (image != null)
                {
                    image.color = isUnlocked ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1.0f);//���رݵ� ī��� ��Ӱ� ǥ�� 
                }
                cardObj.SetActive(true);
            }
        }
    }

    private void OnBackButtonClicked()//�ڷΰ��� �޼���
    {
        Debug.Log("[CollectionScene] �ڷΰ��� Ŭ�� : ���� ������ ���ư��� CollectionScene�� ������� �ʱ�ȭ");
        HapticUX.Vibrate(50);
        StartCoroutine(ExitSceneWithFade());
    }

    private IEnumerator ExitSceneWithFade()// �ڷΰ��� ��ư Ŭ�� �� ���̵�ƿ� �� ���� ������ ���ư��� �޼���.
    {
        isAnimating = true;
        CanvasGroup activeCanvasGroup = hasFlipped ? contentCanvasGroup : coverCanvasGroup;//���� Ȱ��ȭ�� �г��� ã�� ���̵�ƿ��� �����Ѵ�.
        if (activeCanvasGroup != null)
        {
            var fadeOutTween = DotweenAnimations.FadeOutCollectionScene(activeCanvasGroup, fadeOutDuration);
            yield return fadeOutTween.WaitForCompletion();
        }
        if (this != null && gameObject != null) // ���� üũ
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

    void OnDestroy()//DOTWeen ����.
    {
        if (coverTransform != null)
            DotweenAnimations.KillTweensOnTransform(coverTransform);
    }


}
