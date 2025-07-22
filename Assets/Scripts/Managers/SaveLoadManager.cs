using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using static DataStructures;
using System;
public class SaveLoadManager : MonoBehaviour
{
    //����/�ε� ���� ��ũ��Ʈ. ���� �������� ����, �ε�, json ���� ������ ����ϴ� �̱��� �Ŵ�����, ���� �ý��۰��� ��� ��ȣ�ۿ��� ó��.

    private static SaveLoadManager instance;//�̱��� �ν��Ͻ�
    public static SaveLoadManager Instance//�̱��� �ν��Ͻ� ���ٿ� ������Ƽ
    {
        get
        {
            if (instance == null)
            {
                GameObject managerObj = GameObject.Find("Managers");
                if (managerObj == null)
                {
                    managerObj = new GameObject("Managers");//Managers ������Ʈ�� ������ ����
                    DontDestroyOnLoad(managerObj);//�� ��ȯ �ÿ��� �ı����� �ʵ��� ����
                }
                instance = managerObj.GetComponent<SaveLoadManager>();//Managers ������Ʈ���� SaveLoadManager ������Ʈ�� ã��
                if (instance == null)
                {
                    instance = managerObj.AddComponent<SaveLoadManager>();//������Ʈ�� ������ �߰�
                }
            }
            return instance;//�̱��� �ν��Ͻ� ��ȯ
        }
    }

    private string saveFilePath;//���� ���� ���
    private string configFilePath;//���� ���� ���
    private GameConfig gameConfig;//�ε�� ���� ���� ������

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);//�� ��ȯ �ÿ��� �ı����� �ʵ��� ����
            InitializePaths();//���� ��� �ʱ�ȭ
            LoadGameConfig();//���� ���� �ε�
        }
        else if (instance != this)
        {
            Destroy(gameObject);//�ߺ� �ν��Ͻ� ����
        }
    }

    private void InitializePaths()//���� �� ���� ������ ��ü ��θ� �����ϴ� �޼���. �÷��� ���� �ùٸ� ��θ� �����Ѵ�.
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "Save_Data.json");//���� ���� ���. persistentDataPath�� �÷����� ���� �ٸ��� ������. 
        configFilePath = Path.Combine(Application.streamingAssetsPath, "Game_Config.json");//��Ʈ���� ���� �������� ���� ���� ���. streamingAssetsPath�� ���� �� �б� �������� ����.
        Debug.Log($"[SaveLoadManager] ���� ���: {saveFilePath}");
        Debug.Log($"[SaveLoadManager] ���� ���� ���: {configFilePath}");
    }

    private void LoadGameConfig()//���� ���� ������ �ε��ϴ� �޼���.
    {
        try
        {
            if (File.Exists(configFilePath))//���� ������ ��ο� �����ϸ�
            {
                string json = File.ReadAllText(configFilePath, Encoding.UTF8);//���� ������ �о��.
                gameConfig = JsonUtility.FromJson<GameConfig>(json);//JSON ���ڿ��� GameConfig ��ü�� ��ȯ
                Debug.Log("[SaveLoadManager] Game_Config.json �ε� �Ϸ�");
            }
            else
            {
                Debug.LogError("[SaveLoadManager] Game_Config.json ������ �������� �ʽ��ϴ�.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] Game_Config.json �ε� �� ���� �߻�: {e.Message}");//���� �б� ���� ó��
            CreateDefaultGameConfig();//�⺻ ���� ������ ����
        }
    }

    private void CreateDefaultGameConfig()// �⺻ ���� ������ �����ϴ� �޼���. ���� ������ ���� �� ȣ��ȴ�.
    {
        gameConfig = new GameConfig();// �� GameConfig ��ü ����
        gameConfig.rank_system = new RankSystem();//���� �ý��� �ʱ�ȭ
        gameConfig.rank_system.ranks = new List<RankInfo>()//���� ���� ����Ʈ �ʱ�ȭ
        {
            new RankInfo { id = 1, name = "����", required_score = 0 },
            new RankInfo { id = 2, name = "���", required_score = 30 },
            new RankInfo { id = 3, name = "����", required_score = 40 },
            new RankInfo { id = 4, name = "�븮", required_score = 50 },
            new RankInfo { id = 5, name = "����", required_score = 60 },
            new RankInfo { id = 6, name = "����", required_score = 70 },
            new RankInfo { id = 7, name = "����", required_score = 80 },
            new RankInfo { id = 8, name = "�̻�", required_score = 100 },
            new RankInfo { id = 9, name = "��", required_score = 150 },
            new RankInfo { id = 10, name = "����", required_score = 190 },
            new RankInfo { id = 11, name = "�λ���", required_score = 250 },
            new RankInfo { id = 12, name = "����", required_score = 300 }
        };
        gameConfig.affection_thresholds = new AffectionThresholds();//ȣ���� �Ӱ谪 �ʱ�ȭ
    }

    public bool HasSaveData()//���� ������ ���� ���θ� Ȯ���ϴ� �޼���.
    {
        return File.Exists(saveFilePath);//���� ������ �����ϸ� true ��ȯ
    }

    public SaveData LoadSaveData()//Sabe_Data.json������ �о SaveData ��ü�� ��ȯ�ϴ� �޼���. ���� �ջ� �� �⺻ �����͸� �����ϵ��� ����ó���� �ϰ�, utf8���ڵ����� �ѱ� ������ ����.
    {
        try
        {
            if (HasSaveData())//���� �����Ͱ� �����ϸ�
            {
                string json = File.ReadAllText(saveFilePath, Encoding.UTF8);//���� ������ �о�´�.
                SaveData data = JsonUtility.FromJson<SaveData>(json);//JSON ���ڿ��� SaveData ��ü�� ��ȯ
                Debug.Log("[SaveLoadManager] Save_Data.json �ε� �Ϸ�");
                return data;//��ȯ�� SaveData ��ü ��ȯ
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] Save_Data.json �ε� �� ���� �߻�: {e.Message}");//���� �б� ���� ó��
        }
        return CreateDefaultSaveData();//���� �����Ͱ� ���ų� ���� �߻� �� �⺻ �����͸� �����Ͽ� ��ȯ
    }

    public SaveData CreateDefaultSaveData()// �⺻ ���� �����͸� �����ϴ� �޼���. Save_Data.json�� ���ų� �ջ�� ��� ȣ��.
    {
        Debug.Log("[SaveLoadManager] �⺻ ���� ������ ����");
        SaveData defaultData = new SaveData();//�� SaveData ��ü ����
        defaultData.player_data.affection_level = 50;//�⺻ ȣ���� 50
        defaultData.player_data.social_score = 0;//�⺻ ��ȸ�� ���� 0
        defaultData.timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");//���� �ð��� ����
        return defaultData;//������ �⺻ SaveData ��ü ��ȯ
    }

    public void SaveGameData(SaveData data)// SaveData ��ü�� json ���·� �����ϴ� �޼���.
    {
        try
        {
            data.timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");//���� �ð� ������Ʈ
            string json = JsonUtility.ToJson(data, true);//SaveData ��ü�� JSON ���ڿ��� ��ȯ
            File.WriteAllText(saveFilePath, json, Encoding.UTF8);//���Ͽ� JSON ���ڿ��� UTF-8 ���ڵ����� ����
            Debug.Log("[SaveLoadManager] Save_Data.json ���� �Ϸ�");//���� �Ϸ� �α� ���
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] Save_Data.json ���� �� ���� �߻�: {e.Message}");//���� ���� ���� ó��
        }
    }

    public GameConfig GetGameConfig()//���� ������ ��ȯ�ϴ� �޼���.
    {
        return gameConfig;//�ε�� GameConfig ��ü ��ȯ
    }

    public string GetRankNameByScore(int socialScore)//��ȸ�� ������ ���� ���� ���� ����ϴ� �޼���. Game_Config.json�� rank_system.ranks�� �����Ѵ�.
    {
        if (gameConfig?.rank_system?.ranks == null) return "����";//���� ������ ������ �⺻ "����" ��ȯ

        string rankName = "����";
        foreach (var rank in gameConfig.rank_system.ranks)//���� ���� ����Ʈ�� ��ȸ
        {
            if (socialScore >= rank.required_score)//��ȸ�� ������ �ش� ������ �䱸 ���� �̻��̸�
                rankName = rank.name;//���� ���� ������Ʈ
        }
        return rankName;//���� ���� �� ��ȯ
    }

    public int GetRankIdByScore(int socialScore)//��ȸ�� ������ ���� ���� ID�� ����ϴ� �޼���. Game_Config.json�� rank_system.ranks�� �����Ѵ�.
    {
        if (gameConfig?.rank_system?.ranks == null) return 1;//���� ������ ������ �⺻ ID 1(����) ��ȯ.
        int rankId = 1;//�⺻ ���� id
        foreach (var rank in gameConfig.rank_system.ranks)// ���� ���� ����Ʈ�� ��ȸ.
        {
            if (socialScore >= rank.required_score)//��ȸ�� ������ �ش� ������ �䱸 ���� �̻��̸�
            {
                rankId = rank.id;//���� ID�� ������Ʈ
            }
        }
        return rankId;//���� ���� ID ��ȯ
    }

    public void DeleteSaveData()//���� �����͸� �����ϴ� �޼���.
    {
        try
        {
            if (HasSaveData())
            {
                File.Delete(saveFilePath);// ���� ���� ����.
                Debug.Log("[SaveLoadManager] Save_Data.json ���� �Ϸ�");//���� �Ϸ� �α� ���
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] Save_Data.json ���� �� ���� �߻�: {e.Message}");//���� ���� ���� ó��
        }
    }

}