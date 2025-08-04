using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;

public class CollectionManager : MonoBehaviour
{
    // ���� ī�� �� �÷��� �ý���(�����ø)�� �����ϴ� Ŭ����.

    private static CollectionManager instance;
    public static CollectionManager Instance
    {
        get
        {
            GameObject managerObj = GameObject.Find("Managers");
            if (managerObj == null)
            {
                managerObj = new GameObject("Managers");//Managers ������Ʈ�� ������ ����
                DontDestroyOnLoad(managerObj);//�� ��ȯ �ÿ��� �ı����� �ʵ��� ����
            }
            instance = managerObj.GetComponent<CollectionManager>();//Managers ������Ʈ���� SaveLoadManager ������Ʈ�� ã��
            if (instance == null)
            {
                instance = managerObj.AddComponent<CollectionManager>();//������Ʈ�� ������ �߰�
            }
            return instance;
        }
    }

    public event Action<CollectionCard> OnCardUnlocked;//����ī�� �ر� �̺�Ʈ
    public event Action<float> OnCompletionRateChanged;//ī�� ������ ���� �̺�Ʈ
    private CollectionData currentCollectionData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCollectionManager();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCollectionManager()//�÷��� �Ŵ��� ��ü �ʱ�ȭ �޼���.
    {
        LoadCollectionData();
        Debug.Log("[CollectionManager] �÷��� �Ŵ��� �ʱ�ȭ �Ϸ�");
    }

    public void LoadCollectionData()//������ �÷��� �����͸� �ε��ϴ�  �޼���.
    {
        if (ScoreManager.Instance != null)
        {
            var saveData = ScoreManager.Instance.GetCurrentSaveData();//���ھ� �Ŵ����� ���� ���� �����͸� �ҷ��´�. -> currentSaveData�� DataStructures�� SaveDataŬ���� �����͸� ����Ű��, �� SaveData���� PlayerDataŬ������ �����Ͱ� ��ü�� ����Ǿ� ����.
            if (saveData != null)
            {
                currentCollectionData = saveData.player_data.collectionData;//�÷��̾� ������ ����� ����ī�� �����͸� �����´�.
                Debug.Log($"[CollectionManager] �÷��� ������ �ε� �Ϸ� : {currentCollectionData.unlockedCards.Count}�� ����ī�� ���� ��");
            }
        }
        if (currentCollectionData == null)//currentCollectionData�� null�̸� ������ ��ü ����
        {
            currentCollectionData = new CollectionData();
        }
        UpdateCompletionRate();//������ ������Ʈ.
    }

    public void UnlockEndingCard(EndingType endingType, string bossType)//���� �޼� �� �÷��� ī�� ����� �����ϴ� �޼���. ���� Ÿ�԰� ��� Ÿ���� �Ű������� �޾� �ر� ���θ� üũ�ϰ�, ���رݵ� ī�带 �ر��Ѵ�.
    {
        string cardId = $"{bossType}_{endingType.ToString().ToLower()}";
        if (IsCardUnlocked(cardId))//�̹� �رݵ� ī������ Ȯ��
        {
            Debug.Log($"[CollectionManager] �̹� �رݵ� ī�� : {cardId}");
        }

        var newCard = new CollectionCard//�رݵ��� �ʾҴٸ�, �� ����ī�� ��ü�� �����Ͽ� �ر� ó�� ����.
        {
            cardId = cardId,
            cardName = GetCardName(endingType, bossType),
            spritePath = $"Resources/Images/EndingCutScene/{bossType}/{cardId}",
            endingType = endingType,
            bossType = bossType,
            unlockedTime = DateTime.Now,
            isUnlocked = true
        };
        currentCollectionData.unlockedCards.Add(newCard);//�ر� ī�� �÷��� ����Ʈ�� �߰�.
        UpdateCompletionRate();//ī�� ������ ����
        SaveCollectionData();//���� �����Ϳ� �ݿ�
        OnCardUnlocked?.Invoke(newCard);//�̺�Ʈ �߻�
        Debug.Log($"[CollectionData] �� ī�� ��� ���� :{newCard.cardName}");
        HapticUX.Vibrate(200);
        //���� ȹ�� UI �߰�
    }

    public bool IsCardUnlocked(string cardId)//ī�� �ر� ���� Ȯ�� �޼���.
    {
        for (int i = 0; i < currentCollectionData.unlockedCards.Count; i++)//�ر�ī�� ����Ʈ�� ���� �� ī�� ���̵� ��ġ�ϴ� �� ���θ� üũ�Ͽ� �ر� ���θ� �Ǵ��Ѵ�.
        {
            if (currentCollectionData.unlockedCards[i].cardId == cardId)
            {
                return true;//������ cardId ���Ұ� �ִٸ� �� ī��� �̹� �رݵ� ��.
            }
        }
        return false;
    }

    public List<CollectionCard> GetUnlockedCards()//��� ������ ��� ī����� ��ȯ�ϴ� �޼���.
    {
        List<CollectionCard> result = new List<CollectionCard>();//���� ī�� ����Ʈ ��ü ����
        for (int i = 0; i < currentCollectionData.unlockedCards.Count; i++)
        {
            result.Add(currentCollectionData.unlockedCards[i]);//�ر� ����Ʈ�� �����ϴ� ��� �رݵ� ī����� ���� ī�� ����Ʈ�� result�� �߰��ϰ� ��ȯ�Ѵ�.
        }
        return result;
    }

    public List<CollectionCard> GetUnlockedCardsByBoss(string bossType)// Ư�� ��� Ÿ�Կ� �´� �رݵ� ī�带 ��ȯ�ϴ� �޼���.
    {
        List<CollectionCard> result = new List<CollectionCard>();
        for (int i = 0; i < currentCollectionData.unlockedCards.Count; i++)
        {
            if (currentCollectionData.unlockedCards[i].bossType == bossType)
            {
                result.Add(currentCollectionData.unlockedCards[i]);
            }
        }
        return result;
    }

    public List<CollectionCard> GetUnlockedCardsByEndingType(EndingType endingType)//Ư�� ���� Ÿ�Կ� �´� �رݵ� ī�带 ��ȯ�ϴ� �޼���. 
    {
        List<CollectionCard> result = new List<CollectionCard>();
        for (int i = 0; i < currentCollectionData.unlockedCards.Count; i++)
        {
            if (currentCollectionData.unlockedCards[i].endingType == endingType)
            {
                result.Add(currentCollectionData.unlockedCards[i]);
            }
        }
        return result;
    }

    private void UpdateCompletionRate()//���� ī�� ������ ������Ʈ �޼���.
    {
        if (currentCollectionData.totalCardCount > 0)//1�� �̻��� ī�带 ������ ���
        {
            currentCollectionData.completionRate = (float)currentCollectionData.unlockedCards.Count / currentCollectionData.totalCardCount * 100.0f;//(�رݵ� ī�� ��/9)*100
            OnCompletionRateChanged?.Invoke(currentCollectionData.completionRate);//������ ���� �̺�Ʈ �߻�.
        }
    }

    private void SaveCollectionData()//�÷��� �����͸� �����ϴ� �޼���.
    {
        if (ScoreManager.Instance != null)
        {
            var saveData = ScoreManager.Instance.GetCurrentSaveData();//���ھ� �Ŵ����� ���� ���� ���� �����͸� �ε�
            if (saveData != null)
            {
                saveData.player_data.collectionData = currentCollectionData;//���� �÷��� �����͸� ����
                SaveLoadManager.Instance.SaveGameData(saveData);//���̺�ε� �Ŵ����� ���� �÷��� �����Ͱ� ���Ե� �÷��̾� �����͸� ����.
                Debug.Log("[CollectionManager] �÷��� ������ ���� �Ϸ�");
            }
        }
    }

    private string GetCardName(EndingType endingType, string bossType)//CardName�� �����ϴ� �޼���.
    {
        string bossName = "���";
        switch (bossType)
        {
            case "male_boss":
                bossName = "���� ����";
                break;
            case "female_boss":
                bossName = "���� ����";
                break;
            case "young_boss":
                bossName = "���� ����";
                break;

        }
        string endingName = "Ending";
        switch (endingType)
        {
            case EndingType.True:
                endingName = "TrueEnding";
                break;
            case EndingType.Good:
                endingName = "GoodEnding";
                break;
            case EndingType.Bad:
                endingName = "BadEnding";
                break;
        }
        return $"{bossName} {endingName}";

    }
    
}
