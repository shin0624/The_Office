using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private float flipDuration = 1.5f;//�ѱ�� �ִϸ��̼� �ð�
    [SerializeField] private float flipAngle = -180.0f;//�ѱ�� ȸ�� ����

    [Header("���̵� ȿ�� ����")]
    [SerializeField] private float fadeInDuration = 0.8f;//���̵��� �ð�
    [SerializeField] private float fadeOutDuration = 0.5f;//���̵�ƿ� �ð�

    [Header("�ڵ� ���� ����")]
    [SerializeField] private bool autoFlipOnStart = true;//�� ���� �� ǥ�� �ڵ� �ѱ�� �÷���
    [SerializeField] private float autoFlipDelay = 1.0f;//�ڵ� �ѱ�� �����ð�

    [Header("��ȣ�ۿ� ��ư")]
    [SerializeField] private Button backButton;//�ڷΰ��� ��ư
    [SerializeField] private List<GameObject> cardList;//�����÷��� ī�� ����Ʈ. 0-true, 1-good, 2-bad

    private bool isAnimating = false;//�ִϸ��̼� ���� �� �÷���
    private bool hasFlipped = false;//ǥ���� �Ѱ���� ����

    public bool IsAnimating => isAnimating;
    public bool HasFliped => hasFlipped;

    void Start()
    {
        InitializeScene();//�� �ʱ���� ����
        if (autoFlipOnStart)
        {
            StartCoroutine(AutoFlipCover());
        }
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

    void OnEnable()
    {
        SetUpButtons();
    }

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

    private void SetUpButtons()//�ڷΰ��� ��ư �� ǥ�� ��ư ����. ǥ���� ��ư ������Ʈ�� �޾Ƽ�, ǥ�� Ŭ�� �� ������ �ѱ�� �ִϸ��̼��� ����� ��.(�ڵ� ���� ��� ����)
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

    private IEnumerator AutoFlipCover()//�ڵ� ǥ�� �ѱ�� �ڷ�ƾ �޼���.
    {
        yield return new WaitForSeconds(autoFlipDelay);
        if (!hasFlipped && !isAnimating)
        {
            FlipCoverToContents();
        }
    }

    private void OnCoverClicked()//ǥ�� Ŭ�� �� ���� �ѱ��
    {
        if (!hasFlipped && !isAnimating)
        {
            Debug.Log("[CollectionScene] ǥ�� ���� Ŭ�� - �ѱ�� ����");
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

    private IEnumerator FlipCoverSequence()//���� ǥ�� �ѱ�� ������ �޼���.
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

        if (coverPanel != null)//ǥ�� �г� ��Ȱ��ȭ. 
        {
            coverPanel.SetActive(false);
        }
        if (cardList != null)//�����ø �� ���� ī�� ����Ʈ ���� ����.
        {
            DotweenAnimations.ShowCollectionCardsSequentially(cardList);
        }
        hasFlipped = true;
        isAnimating = false;
        Debug.Log("[CollectionScene] ǥ�� �ѱ�� �ִϸ��̼� �Ϸ�");
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

    void OnDestroy()//DOTWeen ����.
    {
        if (coverTransform != null)
            DotweenAnimations.KillTweensOnTransform(coverTransform);
    }


}
