using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;

public enum EndingBranchType { Affection, Rank }// ���� �б� Ÿ�� ����ü ����.

public class ScoreManager : MonoBehaviour
{
    //�÷��̾��� ��ȸ�������� ��� ȣ������ �����ϴ� Ŭ����. ������ �ٽ��� �� ������ �����ϰ�, ����/���� ������ üũ. �̱������� �ۼ�. 
    private static ScoreManager instance;
    public static ScoreManager Instance
    {
        get
        {
            // if (instance == null)
            // {
            //     GameObject managerObj = GameObject.Find("Managers");
            //     if (managerObj == null)
            //     {
            //         managerObj = new GameObject("Managers");
            //         DontDestroyOnLoad(managerObj);
            //     }

            //     instance = managerObj.GetComponent<ScoreManager>();
            //     if (instance == null)
            //     {
            //         instance = managerObj.AddComponent<ScoreManager>();
            //     }
            // }
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
    public event Action<bool, EndingBranchType> OnEndingBranchChanged;// ���� �б� �׼�
    public string rankTrue = "����";
    public string rankEnableBranch = "����";//Good/True���� �б� ���� ����. ���� �б� ������ �������� ������ ���� ���� ���� �������� �б��� �� �����Ƿ�, �̸� �����ϱ� ����
    private bool isEndingBranchEndabled = false;//Good/True���� �б� �÷���.

    //Getter �޼����
    public int GetAffectionScore() => affectionScore;//ȣ���� ��ȯ
    public int GetSocialScore() => socialScore;//��ȸ�� ���� ��ȯ
    public string GetCurrentRank() => currentRank;//���� ���� ��ȯ
    public int GetCurrentDialogueId() => currentDialogueId;//���� ��ȭ ID ��ȯ
    public SaveData GetCurrentSaveData() => currentSaveData;//���� ���� ������ ��ȯ

    //250813. affection, social ��ġ ��ȭ UI �ð�ȭ�� ���� �̺�Ʈ ��ε�ĳ��Ʈ (ScoreManager�� �̱����̶� MainScene�� ����� ��ũ��Ʈ ������ �����Ǿ� NRE�� �߻��ϹǷ�, ��ġ��ȭ �޼��尡 �ۼ��� ValueChangeEffect�� �Ϲ� ������ �� ����.)
    public event Action<int, int> OnScoresChanged;// Action�Ű������� ���� affection, social�̸�, ���� ���� �ÿ��� ScoreManager�� �̺�Ʈ�� �����ϰ� �ؼ� MainScene �� Ȱ��ȭ �� ValueChangeEffect�� �ش� �̺�Ʈ�� ����, ��Ȱ��ȭ �� �����Ѵ�. -> ���� ui�� ������ �ƹ��� �̺�Ʈ�� ���� �����Ƿ� NRE �߻�X

    private void Awake()
    {
        // if (instance == null)
        // {
        //     instance = this;
        //     DontDestroyOnLoad(gameObject);//�� ��ȯ �ÿ��� �ı����� �ʵ��� ����
        //     InitializeGame();//���� �ʱ�ȭ
        // }
        // else if (instance != this)
        // {
        //     Destroy(gameObject);//�ߺ� �ν��Ͻ� ����
        // }
        if (instance != null && instance != this)
        {
            Destroy(gameObject);//�ߺ� ����
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeGame();//���� �ʱ�ȭ
    }

    private void Start()
    {
        CheckEndingBranchValidity();//���� �귣ġ ���� ���� üũ   
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

        while (!SaveLoadManager.Instance.IsConfigLoaded())//250728 ������ ���� : SaveLoadManager�� gameConfig �ε� �Ϸ���� ���.
        {
            yield return new WaitForSeconds(0.1f);
        }

        gameConfig = SaveLoadManager.Instance.GetGameConfig();//���� ���� �ε�
        LoadOrCreateGameData();//���� ���� �� ���嵥���� Ȯ�� �� �ʱ�ȭ. ���� ������ ���� ���ο� ���� �б� ó���Ѵ�.
        isInitialized = true;//�ʱ�ȭ �Ϸ� �÷��� ����
    }

    private void LoadOrCreateGameData()//���嵥���Ͱ� �����ϸ� �ε��ϰ�, ������ �⺻������ �ʱ�ȭ �ϴ� �޼���.
    {
        if (SaveLoadManager.Instance.HasSaveData())//���� �����Ͱ� �����ϸ�
        {
            Debug.Log("[ScoreManager] ���� ���� ������ �߰�, �ε� ��...");
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

    public void CreateNewGame()//�� ������ ������ �� ȣ��Ǵ� �޼���. �⺻������ �ʱ�ȭ�ϰ� �����Ѵ�.
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

        OnScoresChanged?.Invoke(affectionChange, socialChange);//250813. ���� ��ȭ �ÿ��� �����ϴ� ���� ����ġ �ð�ȭ �̺�Ʈ.

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
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ScoreDown);//���� �ٿ� ȿ���� ��� 
            }
            else
            {
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ScoreUp);//���� �� ȿ���� ���
            }
            currentSaveData.player_data.affection_level = affectionScore;//���� �����Ϳ� �ݿ�.
            OnAffectionChanged?.Invoke(affectionScore);//ȣ���� ���� �̺�Ʈ ȣ��
            CheckEndingBranchValidity();//���� �б� �÷��� üũ
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
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ScoreDown);//���� �ٿ� ȿ���� ��� 
            }
            else
            {
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.ScoreUp);//���� �� ȿ���� ���
            }
            currentSaveData.player_data.social_score = socialScore;//���� �����Ϳ� �ݿ�.
            OnSocialScoreChanged?.Invoke(socialScore);//��ȸ�� ���� ���� �̺�Ʈ ȣ��
            CheckRankUp();//���� ������Ʈ üũ
            CheckEndingBranchValidity();// ���� �б� �÷��� üũ
        }
    }

    private void CheckEndingConditions()//ȣ������ ���� ���� ������ üũ�ϴ� �޼���. Game_Config.json�� �Ӱ谪 �������� ���� ������ üũ�Ѵ�. ���� ó�� ���� �ۼ� ��(250731)
    {
        if (gameConfig?.affection_thresholds == null) return;//ȣ���� �Ӱ谪 ������ ������ ��ȯ.
        // if (affectionScore <= gameConfig.affection_thresholds.bad_ending)//ȣ������ ���忣�� �Ӱ谪�̸�
        // {
        //     Debug.Log("[ScoreManager] ���� ����");
        //     OnBadEnding?.Invoke();//���忣�� �̺�Ʈ ȣ��
        //     //�߰����� ���� ó�� ���� �ۼ�.
        // }
        // else if (affectionScore < gameConfig.affection_thresholds.true_ending && affectionScore >= gameConfig.affection_thresholds.low_threshold)//ȣ���� 100 ���� && ȣ���� 30 �̻� && �÷��̾�����==���� �̸� True����.
        // {
        //     Debug.Log("[ScoreManager] Ʈ�� ����");
        //     OnTrueEnding?.Invoke();//Ʈ�翣�� �̺�Ʈ ȣ��
        //     //�߰����� ���� ó�� ���� �ۼ�.
        // }
        
        if(!isEndingBranchEndabled)//�б� ��ȿȭ ������ ������ ���� üũ ���� --> ���� �÷��� ���� ���� ���� �б� ����.
            return;

        bool isTrueBranch = affectionScore >= gameConfig.affection_thresholds.low_threshold && affectionScore <= gameConfig.affection_thresholds.true_ending;//ȣ���� 30 �̻� 100 ���� �� True ����(1) ����
        OnEndingBranchChanged?.Invoke(isTrueBranch, EndingBranchType.Affection);// isTrueBranch�� t/f���� ȣ���� �÷��׸� ����.
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

        if (!isEndingBranchEndabled)//�б� ��ȿȭ ������ ������ ���� üũ ����. --> ���� �÷��� ���� ���� ���� �б� ����
            return;
        bool isTrueBranch = currentRank == rankTrue;//���� ������ ������ �Ǹ� True ����(2) ����
        OnEndingBranchChanged?.Invoke(isTrueBranch, EndingBranchType.Rank);//t/f���� ���� �÷��� ����.
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

    private void CheckEndingBranchValidity()//�����б� �÷����� t/f�� �����ϴ� �޼���.
    {
        string playerRank = GetCurrentRank();
        isEndingBranchEndabled = playerRank == rankEnableBranch;// �÷��̾ ������ �� �� ���� �б� ��ȿ �÷��� = true�� �� ��.
        Debug.Log($"���� �б� �÷���  = {isEndingBranchEndabled}");
    }

    public bool IsEndingBranchEnabled()
    {
        return isEndingBranchEndabled;
    }

}

