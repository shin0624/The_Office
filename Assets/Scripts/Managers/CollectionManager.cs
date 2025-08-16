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
            // GameObject managerObj = GameObject.Find("Managers");
            // if (managerObj == null)
            // {
            //     managerObj = new GameObject("Managers");//Managers ������Ʈ�� ������ ����
            //     DontDestroyOnLoad(managerObj);//�� ��ȯ �ÿ��� �ı����� �ʵ��� ����
            // }
            // instance = managerObj.GetComponent<CollectionManager>();//Managers ������Ʈ���� SaveLoadManager ������Ʈ�� ã��
            // if (instance == null)
            // {
            //     instance = managerObj.AddComponent<CollectionManager>();//������Ʈ�� ������ �߰�
            // }
            return instance;
        }
    }

    public event Action<CollectionCard> OnCardUnlocked;//����ī�� �ر� �̺�Ʈ
    public event Action<float> OnCompletionRateChanged;//ī�� ������ ���� �̺�Ʈ
    private CollectionData currentCollectionData;

    private readonly string[] bossOrder = { "male_boss", "female_boss", "young_boss" };//���� ī�� ��ġ ���� ����(3*3)
    private readonly EndingType[] endingOrder = { EndingType.True, EndingType.Good, EndingType.Bad };

    void Awake()
    {
        // if (instance == null)
        // {
        //     instance = this;
        //     DontDestroyOnLoad(gameObject);
        //     InitializeCollectionManager();
        // }
        // else if (instance != this)
        // {
        //     Destroy(gameObject);
        // }

        if (instance != null && instance != this)
        {
            Destroy(gameObject);//�ߺ� ����
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeCollectionManager();
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
//-------------------------------250805_3*3�׸��忡 ���߾� ���� ī�带 �ε��ϴ� �޼��� ����
    public void UnlockEndingCard(EndingType endingType, string bossType)//���� �޼� �� �÷��� ī�� ����� �����ϴ� �޼���. ���� Ÿ�԰� ��� Ÿ���� �Ű������� �޾� �ر� ���θ� üũ�ϰ�, ���رݵ� ī�带 �ر��Ѵ�.
    {
        string cardId = $"{bossType}_{endingType.ToString().ToLower()}";
        if (IsCardUnlocked(cardId))//�̹� �رݵ� ī������ Ȯ��
        {
            Debug.Log($"[CollectionManager] �̹� �رݵ� ī�� : {cardId}");
            return;
        }

        int cardIndex = GetCardIndex(bossType, endingType);//����ī�� �ε��� ���(3*3 ������ ����)

        var newCard = new CollectionCard//�رݵ��� �ʾҴٸ�, �� ����ī�� ��ü�� �����Ͽ� �ر� ó�� ����.
        {
            cardId = cardId,
            cardName = GetCardName(endingType, bossType),
            spritePath = $"Images/EndingCutScene/{bossType}/{cardId}",
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

    private int GetCardIndex(string bossType, EndingType endingType)//���� ī�� �ε��� ��� �޼���. (0�� : maleBoss(true, good, bad) / 1�� : female_boss(true, good, bad) / 2�� : young_boss(true, good, bad))
    {
        int bossIndex = GetBossIndex(bossType);//��� Ÿ������ ���� ��ȯ�ϰ�, ���� Ÿ������ ���� ��ȯ�Ѵ�.
        int endingIndex = GetEndingIndex(endingType);
        return bossIndex * 3 + endingIndex;//3*3�׸��忡�� �ε��� ��� : �� * 3 + ��
    }

    private int GetBossIndex(string bossType)//��� Ÿ������ �� �ε����� ��ȯ�ϴ� �޼���
    {
        for (int i = 0; i < bossOrder.Length; i++)
        {
            if (bossOrder[i] == bossType)
            {
                return i;
            }
        }return 0;//�⺻���� 0��
    }

    private int GetEndingIndex(EndingType endingType)//���� Ÿ������ �� �ε����� ��ȯ�ϴ� �޼���
    {
        for (int i = 0; i < endingOrder.Length; i++)
        {
            if (endingOrder[i] == endingType)
            {
                return i;
            } 
        }
        return 0;//�⺻���� 0��
    }

    public List<CardSlotInfo> GetOrderedCardSlots()//������� ���ĵ� ��� ī�� ���� ������ ��ȯ�ϴ� �޼���.(���/���� ����)
    {
        List<CardSlotInfo> slots = new List<CardSlotInfo>();
        for (int bossIdx = 0; bossIdx < bossOrder.Length; bossIdx++)
        {
            for (int endingIdx = 0; endingIdx < endingOrder.Length; endingIdx++)
            {
                string bossType = bossOrder[bossIdx];
                EndingType endingType = endingOrder[endingIdx];
                string cardId = $"{bossType}_{endingType.ToString().ToLower()}";

                CollectionCard unlockedCard = GetUnlockedCard(cardId);//�ش� ī�尡 �رݵǾ����� ���� Ȯ�� 

                slots.Add(new CardSlotInfo
                {
                    slotIndex = bossIdx * 3 + endingIdx,
                    cardId = cardId,
                    bossType = bossType,
                    endingType = endingType,
                    isUnlocked = unlockedCard != null,
                    unlockedCard = unlockedCard,
                    cardName = GetCardName(endingType, bossType)
                });
            }
        }
        return slots;
    }

    private CollectionCard GetUnlockedCard(string cardId)//Ư�� ī�� id�� �رݵ� ī�带 ã�� �޼���.
    {
        for (int i = 0; i < currentCollectionData.unlockedCards.Count; i++)
        {
            if (currentCollectionData.unlockedCards[i].cardId == cardId)
            {
                return currentCollectionData.unlockedCards[i];
            }
        }
        return null;
    }

    public bool IsCardUnlocked(string cardId)//ī�� �ر� ���� Ȯ�� �޼���.
    {
        return GetUnlockedCard(cardId) != null;
    }
    //------------------------------- �� 3*3 �׸��带 ������� �ʾ��� ���� �޼����

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
