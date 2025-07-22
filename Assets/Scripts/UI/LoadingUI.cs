using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;

public class LoadingUI : MonoBehaviour
{
    //GameInitializer, SaveLoadManager�� StartScene���� �ʱ�ȭ �۾� + ���� ������ üũ �۾��� �����԰� ���ÿ� �ε� ȭ���� ����ϴ� ����� �����ϴ� ��ũ��Ʈ. �ε��� ������ �ε��г��� ��Ȱ��ȭ�ǰ�, StartPanel�� ������. StartPanel�� ��� Ȱ��ȭ����(�ε��гο� ������ �Ⱥ��̴ϱ�)
    [SerializeField] private Image progressCircle;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingText;//"�ε� ��.."�ؽ�Ʈ
    [SerializeField] private string[] loadingMessages = {
        "���� ������ �ε� ��...",
        "���� ���� Ȯ�� ��...",
        "���� �Ŵ��� �ʱ�ȭ ��...",
        "�غ� �Ϸ�!"};
    private float rotationSpeed = 360.0f;//progressCircle �ʴ� ȸ�� ����
    private float minLoadingTime = 2.0f;//�ּ� �ε� �ð�(�ʹ� ���� ������ �ʵ���)
    private bool isLoading = false;
    private float loadingStartTime;
    private int currentMessageIndex = 0;
    public bool IsLoading => isLoading;//�ܺο��� �ε� ���¸� Ȯ���� �� �ִ� ������Ƽ. 
    private static LoadingUI instance;
    public static LoadingUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<LoadingUI>();
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
    }

    void Start()
    {
        StartLoadingProgress();
    }

    void Update()
    {
        if (isLoading && progressCircle != null)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;//����ð��� ȸ���ӵ��� ���ϸ� progressCircle�� ȸ������ ���� �� ����.
            progressCircle.transform.Rotate(0.0f, 0.0f, -rotationAmount);//progressCircle�� -z������ ȸ������ �ε� ������ ����ȭ.
        }
        if (isLoading && loadingText != null)
        {
            AnimateLoadingText();//�ε� �ؽ�Ʈ �ִϸ��̼� ���(�� ������)
        }
    }

    private async void StartLoadingProgress()
    {
        isLoading = true;
        loadingStartTime = Time.time;

        Debug.Log("[LoadingUI] �ε� ����");
        await PerformLoadingTasks();//�ε� �۾����� �񵿱�� ���� ����
        Debug.Log("[LoadingUI] �ε� �Ϸ�");
        await EnsureMinimumLoadingTime();//�ּ� �ε� �ð�(2��) ����
        CompleteLoading();//�ε� �Ϸ�
    }

    private async Task PerformLoadingTasks()//���� �ε� �۾��� �����ϴ� �񵿱� �޼���. 
    {
        try//���� ����sdk, dotween �� �ʱ�ȭ ���� �߰�
        {
            UpdateLoadingMessage(0);//"���� ������ �ε���.." - SaveLoadManager �ʱ�ȭ ���
            await WaitForSaveLoadManagerInitialization();

            UpdateLoadingMessage(1);//"���� ���� Ȯ�� ��.." - ���� ������ Ȯ��
            await CheckSaveDataAsync();

            UpdateLoadingMessage(3);//"���� �Ŵ��� �ʱ�ȭ ��.." - �� Manager�� �ʱ�ȭ ���
            await WaitForGameManagersInitialization();

            UpdateLoadingMessage(4);//"�غ� �Ϸ�" - �Ϸ�
            await Task.Delay(500);//0.5�� ��� �� ����
        }
        catch (Exception e)
        {
            Debug.LogError($"[LoadingUI] �ε� �� ���� �߻� : {e.Message}");
        }
    }

    private async Task WaitForSaveLoadManagerInitialization()//SaveLoadManager �ʱ�ȭ ��� �޼���
    {
        await Task.Run(async () =>
        {
            while (SaveLoadManager.Instance == null)//�ν��Ͻ� ���� �� ���� ���
            {
                await Task.Delay(50);//50ms���� üũ
            }
            await Task.Delay(500);//�߰� �ʱ�ȭ �ð�
        });
    }
    private async Task CheckSaveDataAsync()//���� ������ Ȯ�� �޼���
    {
        await Task.Run(async () =>
        {
            bool hasSaveData = SaveLoadManager.Instance?.HasSaveData() ?? false;//���� ������ ���� ���� Ȯ��.
            if (hasSaveData)
            {
                Debug.Log("[LoadingUI] ���� ���� ������ �߰�");
            }
            else
            {
                Debug.Log("[LoadingUI] �� ���� �غ�");
            }
            await Task.Delay(400);
        });
    }
    private async Task WaitForGameManagersInitialization()// ���ӸŴ��� �ʱ�ȭ ��� �޼���.
    {
        await Task.Run(async () =>
        {
            while (ScoreManager.Instance == null)
            {
                await Task.Delay(50);
            }
            await Task.Delay(600);//GameInitializer �۾� �Ϸ� ���
        });
    }

    private async Task EnsureMinimumLoadingTime()//�ּ� �ε��ð� ���� �޼���
    {
        float elapsedTime = Time.time - loadingStartTime;//���� �ð� - �ε� ���� �ð����� �ҿ� �ð��� ���
        if (elapsedTime < minLoadingTime)//�ҿ�ð��� �ּ� �ε��ð����� ���� ��
        {
            float remaningTime = minLoadingTime - elapsedTime;//�ּҷε��ð� - ���ð����� �������� �ð��� ���Ѵ�.
            await Task.Delay((int)(remaningTime * 1000));
        }
    }

    private void UpdateLoadingMessage(int messageIndex)//�ε� �޼��� ������Ʈ �޼���
    {
        if (loadingText != null && messageIndex < loadingMessages.Length)
        {
            loadingText.text = loadingMessages[messageIndex];
        }
    }

    private void AnimateLoadingText()//�ε� �ؽ�Ʈ �ִϸ��̼� �޼���.
    {
        if (loadingText == null) return;

        float time = Time.time * 2.0f;//�ִϸ��̼� �ӵ�
        int dotCount = Mathf.FloorToInt(time) % 4;//0~3���� �� ���

        string baseText = loadingMessages[Mathf.Min(currentMessageIndex, loadingMessages.Length - 1)];//���� �ε��޽��� ������ �ε��޽��� �迭 ���̸� ���Ͽ� �� ���� ���� ���̽��ؽ�Ʈ�� ������
        if (baseText.EndsWith("..."))
        {
            baseText = baseText.Substring(0, baseText.Length - 1);//0���� ...���� �ڸ���.

        }
        loadingText.text = baseText = new string('.', dotCount);//ī��Ʈ��ŭ '.'�� �ø���. 
    }

    private void CompleteLoading()//�ε� �Ϸ� ó�� �޼���
    {
        isLoading = false;
        if (loadingPanel != null)//�ε��г� ��Ȱ��ȭ
        {
            StartCoroutine(FadeOutLaodingPanel());
        }
    }

    private IEnumerator FadeOutLaodingPanel()//�ε��г� ���̵�ƿ� ȿ�� �޼���
    {
        CanvasGroup canvasGroup = loadingPanel.GetComponent<CanvasGroup>();//alpha���� �����Ͽ� ���̵�ƿ� ȿ���� �ֱ� ���� ĵ���� �׷� ������Ʈ�� �����´�.
        if (canvasGroup == null)
        {
            canvasGroup = loadingPanel.AddComponent<CanvasGroup>();
        }
        float fadeTime = 0.5f;
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeTime);//1~0���� ������ ���̵�ƿ�.
            yield return null;
        }
        loadingPanel.SetActive(false);
    }

    void OnDestroy()
    {
        instance = null;
    }
}
