using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;

public class CollectionManager : MonoBehaviour
{
    // 엔딩 카드 등 컬렉션 시스템(사원수첩)을 제어하는 클래스.

    private static CollectionManager instance;
    public static CollectionManager Instance
    {
        get
        {
            // GameObject managerObj = GameObject.Find("Managers");
            // if (managerObj == null)
            // {
            //     managerObj = new GameObject("Managers");//Managers 오브젝트가 없으면 생성
            //     DontDestroyOnLoad(managerObj);//씬 전환 시에도 파괴되지 않도록 설정
            // }
            // instance = managerObj.GetComponent<CollectionManager>();//Managers 오브젝트에서 SaveLoadManager 컴포넌트를 찾음
            // if (instance == null)
            // {
            //     instance = managerObj.AddComponent<CollectionManager>();//컴포넌트가 없으면 추가
            // }
            return instance;
        }
    }

    public event Action<CollectionCard> OnCardUnlocked;//엔딩카드 해금 이벤트
    public event Action<float> OnCompletionRateChanged;//카드 수집률 변동 이벤트
    private CollectionData currentCollectionData;

    private readonly string[] bossOrder = { "male_boss", "female_boss", "young_boss" };//엔딩 카드 배치 순서 정의(3*3)
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
            Destroy(gameObject);//중복 방지
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeCollectionManager();
    }

    private void InitializeCollectionManager()//컬렉션 매니저 전체 초기화 메서드.
    {
        LoadCollectionData();
        Debug.Log("[CollectionManager] 컬렉션 매니저 초기화 완료");
    }

    public void LoadCollectionData()//저정된 컬렉션 데이터를 로드하는  메서드.
    {
        if (ScoreManager.Instance != null)
        {
            var saveData = ScoreManager.Instance.GetCurrentSaveData();//스코어 매니저의 현재 저장 데이터를 불러온다. -> currentSaveData는 DataStructures의 SaveData클래스 데이터를 가리키며, 이 SaveData에는 PlayerData클래스의 데이터가 객체로 저장되어 있음.
            if (saveData != null)
            {
                currentCollectionData = saveData.player_data.collectionData;//플레이어 정보에 저장된 엔딩카드 데이터를 가져온다.
                Debug.Log($"[CollectionManager] 컬렉션 데이터 로드 완료 : {currentCollectionData.unlockedCards.Count}개 엔딩카드 보유 중");
            }
        }
        if (currentCollectionData == null)//currentCollectionData가 null이면 새로이 객체 생성
        {
            currentCollectionData = new CollectionData();
        }
        UpdateCompletionRate();//수집률 업데이트.
    }
//-------------------------------250805_3*3그리드에 맞추어 엔딩 카드를 로드하는 메서드 선언
    public void UnlockEndingCard(EndingType endingType, string bossType)//엔딩 달성 시 컬렉션 카드 잠금을 해제하는 메서드. 엔딩 타입과 상사 타입을 매개변수로 받아 해금 여부를 체크하고, 미해금된 카드를 해금한다.
    {
        string cardId = $"{bossType}_{endingType.ToString().ToLower()}";
        if (IsCardUnlocked(cardId))//이미 해금된 카드인지 확인
        {
            Debug.Log($"[CollectionManager] 이미 해금된 카드 : {cardId}");
            return;
        }

        int cardIndex = GetCardIndex(bossType, endingType);//엔딩카드 인덱스 계산(3*3 순서에 맞춤)

        var newCard = new CollectionCard//해금되지 않았다면, 새 엔딩카드 객체를 생성하여 해금 처리 수행.
        {
            cardId = cardId,
            cardName = GetCardName(endingType, bossType),
            spritePath = $"Images/EndingCutScene/{bossType}/{cardId}",
            endingType = endingType,
            bossType = bossType,
            unlockedTime = DateTime.Now,
            isUnlocked = true
        };

        currentCollectionData.unlockedCards.Add(newCard);//해금 카드 컬렉션 리스트에 추가.
        UpdateCompletionRate();//카드 수집률 변경
        SaveCollectionData();//저장 데이터에 반영
        OnCardUnlocked?.Invoke(newCard);//이벤트 발생
        Debug.Log($"[CollectionData] 새 카드 잠금 해제 :{newCard.cardName}");
        HapticUX.Vibrate(200);
        //추후 획득 UI 추가
    }

    private int GetCardIndex(string bossType, EndingType endingType)//엔딩 카드 인덱스 계산 메서드. (0행 : maleBoss(true, good, bad) / 1행 : female_boss(true, good, bad) / 2행 : young_boss(true, good, bad))
    {
        int bossIndex = GetBossIndex(bossType);//상사 타입으로 행을 반환하고, 엔딩 타입으로 열을 반환한다.
        int endingIndex = GetEndingIndex(endingType);
        return bossIndex * 3 + endingIndex;//3*3그리드에서 인덱스 계산 : 행 * 3 + 열
    }

    private int GetBossIndex(string bossType)//상사 타입으로 행 인덱스를 반환하는 메서드
    {
        for (int i = 0; i < bossOrder.Length; i++)
        {
            if (bossOrder[i] == bossType)
            {
                return i;
            }
        }return 0;//기본값은 0행
    }

    private int GetEndingIndex(EndingType endingType)//엔딩 타입으로 열 인덱스를 반환하는 메서드
    {
        for (int i = 0; i < endingOrder.Length; i++)
        {
            if (endingOrder[i] == endingType)
            {
                return i;
            } 
        }
        return 0;//기본값은 0열
    }

    public List<CardSlotInfo> GetOrderedCardSlots()//순서대로 정렬된 모든 카드 슬롯 정보를 반환하는 메서드.(잠금/해제 포함)
    {
        List<CardSlotInfo> slots = new List<CardSlotInfo>();
        for (int bossIdx = 0; bossIdx < bossOrder.Length; bossIdx++)
        {
            for (int endingIdx = 0; endingIdx < endingOrder.Length; endingIdx++)
            {
                string bossType = bossOrder[bossIdx];
                EndingType endingType = endingOrder[endingIdx];
                string cardId = $"{bossType}_{endingType.ToString().ToLower()}";

                CollectionCard unlockedCard = GetUnlockedCard(cardId);//해당 카드가 해금되었는지 여부 확인 

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

    private CollectionCard GetUnlockedCard(string cardId)//특정 카드 id로 해금된 카드를 찾는 메서드.
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

    public bool IsCardUnlocked(string cardId)//카드 해금 여부 확인 메서드.
    {
        return GetUnlockedCard(cardId) != null;
    }
    //------------------------------- ▼ 3*3 그리드를 사용하지 않았을 때의 메서드들

    public List<CollectionCard> GetUnlockedCards()//잠금 해제된 모든 카드들을 반환하는 메서드.
    {
        List<CollectionCard> result = new List<CollectionCard>();//엔딩 카드 리스트 객체 생성
        for (int i = 0; i < currentCollectionData.unlockedCards.Count; i++)
        {
            result.Add(currentCollectionData.unlockedCards[i]);//해금 리스트에 존재하는 모든 해금된 카드들을 엔딩 카드 리스트인 result에 추가하고 반환한다.
        }
        return result;
    }

    public List<CollectionCard> GetUnlockedCardsByBoss(string bossType)// 특정 상사 타입에 맞는 해금된 카드를 반환하는 메서드.
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

    public List<CollectionCard> GetUnlockedCardsByEndingType(EndingType endingType)//특정 엔딩 타입에 맞는 해금된 카드를 반환하는 메서드. 
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

    private void UpdateCompletionRate()//엔딩 카드 수집률 업데이트 메서드.
    {
        if (currentCollectionData.totalCardCount > 0)//1개 이상의 카드를 수집한 경우
        {
            currentCollectionData.completionRate = (float)currentCollectionData.unlockedCards.Count / currentCollectionData.totalCardCount * 100.0f;//(해금된 카드 수/9)*100
            OnCompletionRateChanged?.Invoke(currentCollectionData.completionRate);//수집률 변경 이벤트 발생.
        }
    }

    private void SaveCollectionData()//컬렉션 데이터를 저장하는 메서드.
    {
        if (ScoreManager.Instance != null)
        {
            var saveData = ScoreManager.Instance.GetCurrentSaveData();//스코어 매니저를 통해 현재 저장 데이터를 로드
            if (saveData != null)
            {
                saveData.player_data.collectionData = currentCollectionData;//현재 컬렉션 데이터를 저장
                SaveLoadManager.Instance.SaveGameData(saveData);//세이브로드 매니저를 통해 컬렉션 데이터가 포함된 플레이어 데이터를 저장.
                Debug.Log("[CollectionManager] 컬렉션 데이터 저장 완료");
            }
        }
    }

    private string GetCardName(EndingType endingType, string bossType)//CardName을 생성하는 메서드.
    {
        string bossName = "상사";
        switch (bossType)
        {
            case "male_boss":
                bossName = "남자 부장";
                break;
            case "female_boss":
                bossName = "여자 부장";
                break;
            case "young_boss":
                bossName = "젊은 꼰대";
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
