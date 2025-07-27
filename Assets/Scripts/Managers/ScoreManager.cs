using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;

public class ScoreManager : MonoBehaviour
{
    //�÷��̾��� ��ȸ�������� ��� ȣ������ �����ϴ� Ŭ����. ������ �ٽ��� �� ������ �����ϰ�, ����/���� ������ üũ. �̱������� �ۼ�. 
    private static ScoreManager instance;
    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject managerObj = GameObject.Find("Managers");
                if (managerObj == null)
                {
                    managerObj = new GameObject("Managers");
                    DontDestroyOnLoad(managerObj);
                }

                instance = managerObj.GetComponent<ScoreManager>();
                if (instance == null)
                {
                    instance = managerObj.AddComponent<ScoreManager>();
                }
            }
            return instance;
        }
    }

    [Header("���� ���� ����")]
    [SerializeField] private int affectionScore = 50;//���� ȣ����
    [SerializeField] private int socialScore = 0;//���� ��ȸ�� ����
    [SerializeField] private string currentRank = "����";// ���� ����
    [SerializeField] private int currentDialogueId = 1;// ���� ��ȭ ID(���� ��ġ)

    private SaveData currentSaveData;//���� ������ ���� ������
    private GameConfig gameConfig;//���� ���� ������
    private bool isInitialized = false;//�ʱ�ȭ �Ϸ� ����

    //�̺�Ʈ �ڵ鷯��. UI ������Ʈ �� ���� ���� ��ȭ �˸��� ���� �̺�Ʈ�̸�, �ٸ� ��ũ��Ʈ���� ���� ��ȭ�� �ǽð����� ������ �� �ֵ��� ������ �������� �ۼ�.
    public Action<int> OnAffectionChanged;//ȣ���� ���� �̺�Ʈ
    public Action<int> OnSocialScoreChanged;//��ȸ�� ���� ���� �̺�Ʈ
    public Action<string> OnRankChanged;//���� ���� �̺�Ʈ
    public Action OnBadEnding;//���� ���� �̺�Ʈ
    public Action OnTrueEnding;//Ʈ�� ���� �̺�Ʈ
    public Action<SaveData> OnGameDataLoaded;//���� ������ �ε� �̺�Ʈ

    //Getter �޼����
    public int GetAffectionScore() => affectionScore;//ȣ���� ��ȯ
    public int GetSocialScore() => socialScore;//��ȸ�� ���� ��ȯ
    public string GetCurrentRank() => currentRank;//���� ���� ��ȯ
    public int GetCurrentDialogueId() => currentDialogueId;//���� ��ȭ ID ��ȯ
    public SaveData GetCurrentSaveData() => currentSaveData;//���� ���� ������ ��ȯ

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);//�� ��ȯ �ÿ��� �ı����� �ʵ��� ����
            InitializeGame();//���� �ʱ�ȭ
        }
        else if (instance != this)
        {
            Destroy(gameObject);//�ߺ� �ν��Ͻ� ����
        }
    }

    private void InitializeGame()//���� �ʱ�ȭ �޼���. ���� ������ ���� �����͸� �ε��ϰ�, �ʱ� ���¸� �����Ѵ�.
    {
        StartCoroutine(InitializeAfterFrame());// SaveLoadManger�� �ʱ�ȭ�� �Ŀ� ����ǵ��� �ڷ�ƾ���� ó��.
    }

    private IEnumerator InitializeAfterFrame()//�� ������ ��� �� ���� �ʱ�ȭ�� �����ϴ� �ڷ�ƾ. SaveLoadManager�� ���� �ʱ�ȭ�ǵ��� ����.
    {
        yield return new WaitForEndOfFrame();// �� ������ ���

        while (SaveLoadManager.Instance == null)
        {
            yield return null;// SaveLoadManager�� �ʱ�ȭ�� ������ ���
        }

        gameConfig = SaveLoadManager.Instance.GetGameConfig();//���� ���� �ε�
        LoadOrCreateGameData();//���� ���� �� ���嵥���� Ȯ�� �� �ʱ�ȭ. ���� ������ ���� ���ο� ���� �б� ó���Ѵ�.
        isInitialized = true;//�ʱ�ȭ �Ϸ� �÷��� ����
    }

    private void LoadOrCreateGameData()//���嵥���Ͱ� �����ϸ� �ε��ϰ�, ������ �⺻������ �ʱ�ȭ �ϴ� �޼���.
    {
        if (SaveLoadManager.Instance.HasSaveData())//���� �����Ͱ� �����ϸ�
        {
            Debug.Log("[ScoreManagerr] ���� ���� ������ �߰�, �ε� ��...");
            LoadGameData();//���� ������ �ε�
        }
        else
        {
            Debug.Log("[ScoreManager] ���� ������ ����, �� ���� ����");
            CreateNewGame();
        }
    }

    private void LoadGameData()//����� ���� �����͸� �ε��ϴ� �޼���.
    {
        currentSaveData = SaveLoadManager.Instance.LoadSaveData();//���� ������ �ε�. ���� ����� �����͸� ���� ���¿� ������.
        affectionScore = currentSaveData.player_data.affection_level;//ȣ���� �ʱ�ȭ
        socialScore = currentSaveData.player_data.social_score;//��ȸ�� ���� �ʱ�ȭ
        currentRank = currentSaveData.player_data.current_rank;//���� ���� �ʱ�ȭ
        currentDialogueId = currentSaveData.player_data.current_dialogue_id;//���� ��ȭ ID

        string calculatedRank = SaveLoadManager.Instance.GetRankNameByScore(socialScore);//��ũ ���� �� ������Ʈ.
        if (currentRank != calculatedRank)//���� ���ް� ������ �õ��� ������ �ٸ���
        {
            currentRank = calculatedRank;//���� ������ ������ �������� ������Ʈ
            currentSaveData.player_data.current_rank = currentRank;//���� �����Ϳ��� �ݿ�        
        }
        //UI ������Ʈ �̺�Ʈ ȣ��
        OnAffectionChanged?.Invoke(affectionScore);//ȣ���� ���� �̺�Ʈ ȣ��
        OnSocialScoreChanged?.Invoke(socialScore);//��ȸ�� ���� ���� �̺�Ʈ ȣ��
        OnRankChanged?.Invoke(currentRank);//���� ���� �̺�Ʈ ȣ��
        OnGameDataLoaded?.Invoke(currentSaveData);//���� ������ �ε� �̺�Ʈ ȣ��
        Debug.Log($"[ScoreManager] ������ �ε� �Ϸ� - ȣ����: {affectionScore}, ��ȸ��: {socialScore}, ����: {currentRank}");
    }

    private void CreateNewGame()//�� ������ ������ �� ȣ��Ǵ� �޼���. �⺻������ �ʱ�ȭ�ϰ� �����Ѵ�.
    {
        currentSaveData = SaveLoadManager.Instance.CreateDefaultSaveData();// �⺻ ���� ������ ����
        affectionScore = currentSaveData.player_data.affection_level;//�⺻ ȣ����
        socialScore = currentSaveData.player_data.social_score;//�⺻ ��ȸ�� ����
        currentRank = SaveLoadManager.Instance.GetRankNameByScore(socialScore);//�⺻ ��ȸ�� ������ ���� ���� ���
        currentDialogueId = 1;//�⺻ ��ȭ ID

        currentSaveData.player_data.current_rank = currentRank;//���� �����Ϳ� ���� ���� �ݿ�
        currentSaveData.player_data.current_dialogue_id = currentDialogueId;//���� �����Ϳ� ���� ��ȭ ID �ݿ�

        //UI ��� ������Ʈ �̺�Ʈ ȣ�� 
        OnAffectionChanged?.Invoke(affectionScore);//ȣ���� ���� �̺�Ʈ ȣ��
        OnSocialScoreChanged?.Invoke(socialScore);//��ȸ�� ���� ���� �̺�Ʈ ȣ��
        OnRankChanged?.Invoke(currentRank);//���� ���� �̺�Ʈ ȣ��
        OnGameDataLoaded?.Invoke(currentSaveData);//���� ������ �ε� �̺�Ʈ ȣ��

        SaveGame();//�ʱ�ȭ�� �����͸� �ڵ� ����
        Debug.Log($"[ScoreManager] �� ���� ���� - ȣ����: {affectionScore}, ��ȸ��: {socialScore}, ����: {currentRank}");
    }

    public void UpdateScores(int affectionChange, int socialChange)//ȣ������ ��ȸ�� ���� ���� ������Ʈ�� �����ϴ� �޼���. �ڵ� ���� ������ Ȯ�� �� ������ �ٲ� �� ���� �ڵ� �����Ѵ�.
    {
        AddAffection(affectionChange);//ȣ���� ������Ʈ
        AddSocialScore(socialChange);//��ȸ�� ���� ������Ʈ
        if (currentSaveData.game_settings.auto_save_enabled)//�ڵ� ������ Ȱ��ȭ�Ǿ� ������
        {
            SaveGame();//�ڵ� ���� Ȱ��ȭ �� ���� ���� �� �ڵ� ����
        }
    }

    public void AddAffection(int value)//ȣ������ ������Ű�� �޼���.
    {
        int oldScore = affectionScore;//���� ȣ���� ����
        affectionScore = Mathf.Clamp(affectionScore + value, 0, 100);//ȣ���� ����(0~100) ������ ����
        if (oldScore != affectionScore)//ȣ���� ���� ��
        {
            if (oldScore > affectionScore)// ȣ���� ���� �� ���� ��� ȣ��
            {
                HapticUX.Vibrate(500);
            }
            currentSaveData.player_data.affection_level = affectionScore;//���� �����Ϳ� �ݿ�.
            OnAffectionChanged?.Invoke(affectionScore);//ȣ���� ���� �̺�Ʈ ȣ��
            CheckEndingConditions();//ȣ������ ���� ���� ���� üũ
        }
    }

    public void AddSocialScore(int value)//��ȸ�� ������ ������Ű�� �޼���.
    {
        int oldScore = socialScore;//���� ��ȸ�� ���� ����
        socialScore = Mathf.Clamp(socialScore + value, 0, 300);//��ȸ�� ���� ����(0~300) ������ ����
        if (oldScore != socialScore)//��ȸ�� ���� ���� ��
        {
            if (oldScore > socialScore)//��ȸ�� ���� ���� �� ���� ��� ȣ��
            {
                HapticUX.Vibrate(500);
            }
            currentSaveData.player_data.social_score = socialScore;//���� �����Ϳ� �ݿ�.
            OnSocialScoreChanged?.Invoke(socialScore);//��ȸ�� ���� ���� �̺�Ʈ ȣ��
            CheckRankUp();//���� ������Ʈ üũ
        }
    }

    private void CheckEndingConditions()//ȣ������ ���� ���� ������ üũ�ϴ� �޼���. Game_Config.json�� �Ӱ谪 �������� ���� ������ üũ�Ѵ�. ���� ó�� ������ ���� �ۼ�.
    {
        if (gameConfig?.affection_thresholds == null) return;//ȣ���� �Ӱ谪 ������ ������ ��ȯ.
        if (affectionScore <= gameConfig.affection_thresholds.bad_ending)//ȣ������ ���忣�� �Ӱ谪�̸�
        {
            Debug.Log("[ScoreManager] ���� ����");
            OnBadEnding?.Invoke();//���忣�� �̺�Ʈ ȣ��
            //�߰����� ���� ó�� ���� �ۼ�.
        }
        else if (affectionScore >= gameConfig.affection_thresholds.true_ending)//ȣ������ Ʈ�翣�� �Ӱ谪 �̻��̸�
        {
            Debug.Log("[ScoreManager] Ʈ�� ����");
            OnTrueEnding?.Invoke();//Ʈ�翣�� �̺�Ʈ ȣ��
            //�߰����� ���� ó�� ���� �ۼ�.
        }
    }

    private void CheckRankUp()//��ȸ�� ������ ���� ���� ������Ʈ�� üũ�ϴ� �޼���.
    {
        string newRank = SaveLoadManager.Instance.GetRankNameByScore(socialScore);//��ȸ�� ������ ���� ���� ���
        if (newRank != currentRank)//���ο� ������ ���� ���ް� �ٸ���
        {
            string oldRank = currentRank;//���� ���� ����
            currentRank = newRank;//���� ���� ������Ʈ
            currentSaveData.player_data.current_rank = currentRank;//���� �����Ϳ� �ݿ�

            OnRankChanged?.Invoke(currentRank);//���� ���� �̺�Ʈ ȣ��
            Debug.Log($"[ScoreManager] ���� ����: {oldRank} -> {currentRank}");//���� ���� �α� ���
        }
    }

    private void SaveGame()//���� �����͸� �����ϴ� �޼���.
    {
        if (currentSaveData != null)
        {
            SaveLoadManager.Instance.SaveGameData(currentSaveData);//���� ������ ����
        }
    }

    public void SetCurrentDialogue(int dialogueId)//���� ��ȭ id�� �����ϴ� �޼���. ��ȭ ���� ���¸� ����.
    {
        currentDialogueId = dialogueId;//���� ��ȭ id ������Ʈ
        currentSaveData.player_data.current_dialogue_id = dialogueId;//���� �����Ϳ� �ݿ�
    }

    public void AddCompletedDialogue(int dialogueId)//�Ϸ�� ��ȭ id�� �߰��ϴ� �޼���. ��ȭ �Ϸ� ���¸� ����.
    {
        if (!currentSaveData.player_data.completed_dialogues.Contains(dialogueId))//�̹� �Ϸ�� ��ȭ�� �ƴϸ�
        {
            currentSaveData.player_data.completed_dialogues.Add(dialogueId);//�Ϸ�� ��ȭ ��Ͽ� �߰�
        }
    }

    public string GetAffectionLevel()//ȣ���� ������ ��ȯ�ϴ� �޼���. ȣ������ ���� ������ ���ڿ��� ��ȯ�Ѵ�.
    {
        if (gameConfig?.affection_thresholds == null) return "����";//ȣ���� �Ӱ谪 ������ ������ �⺻ "����" ��ȯ
        if (affectionScore < gameConfig.affection_thresholds.low_threshold) return "����";//���� ȣ����
        else if (affectionScore < gameConfig.affection_thresholds.true_ending) return "����";//���� ȣ����
        else return "����";//���� ȣ����
    }

    //����׿� �޼����
    [ContextMenu("�� ���� ����")]
    private void DebugNewGame()//����׿� �� ���� ���� �޼���. �����Ϳ��� ���� ����.
    {
        SaveLoadManager.Instance.DeleteSaveData();//���� ���� ������ ����
        Debug.Log("[ScoreManager] ����� - �� ���� ����");
        CreateNewGame();
    }

    [ContextMenu("���� ����")]
    private void DebugSaveGame()//����׿� ���� ���� �޼���. �����Ϳ��� ���� ����.
    {
        SaveGame();//���� ���� ���� ����
        Debug.Log("[ScoreManager] ����� - ���� ���� �Ϸ�");
    }

    [ContextMenu("���� ���� ���")]
    private void DebugPrintStatus()//����׿� ���� ���� ��� �޼���. �����Ϳ��� ���� ����.
    {
        Debug.Log($"[ScoreManager] ���� ���� - ȣ����: {affectionScore}, ��ȸ��: {socialScore}, ����: {currentRank}, ��ȭ ID: {currentDialogueId}");
    }
        [ContextMenu("���� +10/+100")]
    public void DebugAddScores()//����׿� ���� ���� �޼���. �����Ϳ��� ���� ����.
    {
        UpdateScores(10, 100);
    }

    [ContextMenu("���� �ʱ�ȭ")]
    public void DebugResetScores()//����׿� ���� �ʱ�ȭ �޼���. �����Ϳ��� ���� ����.
    {
        affectionScore = 50;
        socialScore = 0;
        currentRank = "����";
        if (currentSaveData != null)
        {
            currentSaveData.player_data.affection_level = affectionScore;
            currentSaveData.player_data.social_score = socialScore;
            currentSaveData.player_data.current_rank = currentRank;
        }
        OnAffectionChanged?.Invoke(affectionScore);
        OnSocialScoreChanged?.Invoke(socialScore);
        OnRankChanged?.Invoke(currentRank);
        SaveGame();
    }

}

