using System;
using System.Collections;
using System.Collections.Generic;
using static AudioEnums;
using UnityEngine;

public class DataStructures : MonoBehaviour
{
    //게임 내 필요한 데이터 구조체를 정의한 스크립트. json과 c#객체 간 직/역직렬화를 위한 데이터 모델.

    [Serializable]
    public class GameFlags//게임 진행상황 추적
    {
        public bool first_meeting = true; // 선택된 상사를 처음 선택했는지 여부
        public bool promotion_achieved = false; // 승진을 달성했는지 여부
        public bool special_event_triggered = false; // 특별 이벤트가 발생했는지 여부
    }

    [Serializable]
    public class GameSettings//게임 설정 정보 저장
    {
        public float sound_volume = 0.8f;//효과음(0.0~1.0)
        public float music_volume = 0.6f;//배경음악(0.0~1.0)
        public bool auto_save_enabled = true;//자동 저장 활성화 여부
    }

    [Serializable]
    public class PlayerData//플레이어의 게임 진행 상황 저장
    {
        public string player_name = "신입사원";
        public string current_boss = "male_boss"; // 현재 상사 타입
        public int affection_level = 50;//상사 호감도(저장된 값 없을 시 기본 50)(0~100)
        public int social_score = 0; // 사회력 점수(저장된 값 없을 시 기본 0)(0~99999)
        public string current_rank = "인턴";//현재 직급 명
        public int current_dialogue_id = 1;//현재 진행 중인 대화 ID
        public List<int> completed_dialogues = new List<int>();//완료된 대화 ID 목록
        public GameFlags game_flags = new GameFlags();//게임 진행상황 추적
        public CollectionData collectionData = new CollectionData();//250805 추가 : 엔딩카드 수집 데이터 
    }

    [Serializable]
    public class SaveData//전체 저장 데이터 구조체
    {
        public string save_version = "1.0";
        public PlayerData player_data = new PlayerData();//플레이어 데이터
        public GameSettings game_settings = new GameSettings();//게임 설정 정보
        public string timestamp; //저장 시각
    }

    [Serializable]
    public class RankInfo//직급 정보
    {
        public int id;//직급 ID
        public string name;//직급 명
        public int required_score;//해당 직급으로 승진하기 위한 사회력 점수
    }

    [Serializable]
    public class RankSystem//직급 시스템 전체 구조체
    {
        public List<RankInfo> ranks; //직급 정보 리스트
    }

    [Serializable]
    public class AffectionThresholds//호감도 임계값
    {
        public int bad_ending = 0;
        public int true_ending = 100;
        public int low_threshold = 30; // 낮은 호감도
        public int high_threshold = 70; // 높은 호감도
    }

    [Serializable]
    public class GameConfig//게임 설정 전체 구조체
    {
        public RankSystem rank_system; //직급 시스템 정보
        public AffectionThresholds affection_thresholds; //호감도 임계값 정보
    }

    [Serializable]
    public class CollectionCard// 엔딩 카드 컬렉션 데이터.
    {
        public string cardId;//엔딩 카드의 고유 id. bosstype_endingType 형태이며, endingType은 모두 소문자
        public string cardName;//카드 이름
        public string spritePath;//스프라이트 경로
        public EndingType endingType;//엔딩 타입(True, Good, Bad)
        public string bossType;//상사 타입
        public DateTime unlockedTime;//해금 날짜
        public bool isUnlocked;//해금 여부
    }

    [Serializable]
    public class CollectionData//엔딩 카드 수집 데이터
    {
        public List<CollectionCard> unlockedCards = new List<CollectionCard>();//엔딩 카드 컬렉션 리스트
        public int totalCardCount = 9;//전체 수집 가능한 카드 수 = 상사 3명 * 엔딩 3개
        public float completionRate = 0.0f;//수집률
    }

    [Serializable]
    public class CardSlotInfo
    {
        public int slotIndex;//그리드에서의 위치(0~8)
        public string cardId;//카드 id
        public string bossType;//상사 타입
        public EndingType endingType;//엔딩 타입
        public bool isUnlocked;//해금 여부
        public CollectionCard unlockedCard;//해금된 카드 정보
        public string cardName;//카드 이름
    }

    [Serializable]
    public class EndingSpriteSet
    {
        public Sprite trueEndingSprite;  // True 엔딩 이미지
        public Sprite goodEndingSprite;  // Good 엔딩 이미지
        public Sprite badEndingSprite;   // Bad 엔딩 이미지
        public Sprite errorSprite;//에러 발생 시 이미지

        [TextArea(2, 4)]
        public string trueEndingMessage;  // True 엔딩 메시지
        [TextArea(2, 4)]
        public string goodEndingMessage;  // Good 엔딩 메시지
        [TextArea(2, 4)]
        public string badEndingMessage;   // Bad 엔딩 메시지
    }

    [Serializable]
    public class BGMClipEntry//bgm관리 클래스
    {
        public BGMType type;//어떤 씬의 BGM인지 명시
        public AudioClip clip;
        public bool loop = true;
    }

    [Serializable]
    public class SFXClipEntry//사운드 이펙트 관리 클래스
    {
        public SFXType type;//어떤 인터랙션의 효과음인지 명시
        public AudioClip clip;
        [Range(0.0f, 1.0f)] public float defaultVolume = 1.0f;//기본 재생 볼륨
       [Range(0.5f, 1.5f)] public float pitch = 1.0f;//기본 재생 피치
    }

}
