using System;
using System.Collections;
using System.Collections.Generic;
using static AudioEnums;
using UnityEngine;

public class DataStructures : MonoBehaviour
{
    //���� �� �ʿ��� ������ ����ü�� ������ ��ũ��Ʈ. json�� c#��ü �� ��/������ȭ�� ���� ������ ��.

    [Serializable]
    public class GameFlags//���� �����Ȳ ����
    {
        public bool first_meeting = true; // ���õ� ��縦 ó�� �����ߴ��� ����
        public bool promotion_achieved = false; // ������ �޼��ߴ��� ����
        public bool special_event_triggered = false; // Ư�� �̺�Ʈ�� �߻��ߴ��� ����
    }

    [Serializable]
    public class GameSettings//���� ���� ���� ����
    {
        public float sound_volume = 0.8f;//ȿ����(0.0~1.0)
        public float music_volume = 0.6f;//�������(0.0~1.0)
        public bool auto_save_enabled = true;//�ڵ� ���� Ȱ��ȭ ����
    }

    [Serializable]
    public class PlayerData//�÷��̾��� ���� ���� ��Ȳ ����
    {
        public string player_name = "���Ի��";
        public string current_boss = "male_boss"; // ���� ��� Ÿ��
        public int affection_level = 50;//��� ȣ����(����� �� ���� �� �⺻ 50)(0~100)
        public int social_score = 0; // ��ȸ�� ����(����� �� ���� �� �⺻ 0)(0~99999)
        public string current_rank = "����";//���� ���� ��
        public int current_dialogue_id = 1;//���� ���� ���� ��ȭ ID
        public List<int> completed_dialogues = new List<int>();//�Ϸ�� ��ȭ ID ���
        public GameFlags game_flags = new GameFlags();//���� �����Ȳ ����
        public CollectionData collectionData = new CollectionData();//250805 �߰� : ����ī�� ���� ������ 
    }

    [Serializable]
    public class SaveData//��ü ���� ������ ����ü
    {
        public string save_version = "1.0";
        public PlayerData player_data = new PlayerData();//�÷��̾� ������
        public GameSettings game_settings = new GameSettings();//���� ���� ����
        public string timestamp; //���� �ð�
    }

    [Serializable]
    public class RankInfo//���� ����
    {
        public int id;//���� ID
        public string name;//���� ��
        public int required_score;//�ش� �������� �����ϱ� ���� ��ȸ�� ����
    }

    [Serializable]
    public class RankSystem//���� �ý��� ��ü ����ü
    {
        public List<RankInfo> ranks; //���� ���� ����Ʈ
    }

    [Serializable]
    public class AffectionThresholds//ȣ���� �Ӱ谪
    {
        public int bad_ending = 0;
        public int true_ending = 100;
        public int low_threshold = 30; // ���� ȣ����
        public int high_threshold = 70; // ���� ȣ����
    }

    [Serializable]
    public class GameConfig//���� ���� ��ü ����ü
    {
        public RankSystem rank_system; //���� �ý��� ����
        public AffectionThresholds affection_thresholds; //ȣ���� �Ӱ谪 ����
    }

    [Serializable]
    public class CollectionCard// ���� ī�� �÷��� ������.
    {
        public string cardId;//���� ī���� ���� id. bosstype_endingType �����̸�, endingType�� ��� �ҹ���
        public string cardName;//ī�� �̸�
        public string spritePath;//��������Ʈ ���
        public EndingType endingType;//���� Ÿ��(True, Good, Bad)
        public string bossType;//��� Ÿ��
        public DateTime unlockedTime;//�ر� ��¥
        public bool isUnlocked;//�ر� ����
    }

    [Serializable]
    public class CollectionData//���� ī�� ���� ������
    {
        public List<CollectionCard> unlockedCards = new List<CollectionCard>();//���� ī�� �÷��� ����Ʈ
        public int totalCardCount = 9;//��ü ���� ������ ī�� �� = ��� 3�� * ���� 3��
        public float completionRate = 0.0f;//������
    }

    [Serializable]
    public class CardSlotInfo
    {
        public int slotIndex;//�׸��忡���� ��ġ(0~8)
        public string cardId;//ī�� id
        public string bossType;//��� Ÿ��
        public EndingType endingType;//���� Ÿ��
        public bool isUnlocked;//�ر� ����
        public CollectionCard unlockedCard;//�رݵ� ī�� ����
        public string cardName;//ī�� �̸�
    }

    [Serializable]
    public class EndingSpriteSet
    {
        public Sprite trueEndingSprite;  // True ���� �̹���
        public Sprite goodEndingSprite;  // Good ���� �̹���
        public Sprite badEndingSprite;   // Bad ���� �̹���
        public Sprite errorSprite;//���� �߻� �� �̹���

        [TextArea(2, 4)]
        public string trueEndingMessage;  // True ���� �޽���
        [TextArea(2, 4)]
        public string goodEndingMessage;  // Good ���� �޽���
        [TextArea(2, 4)]
        public string badEndingMessage;   // Bad ���� �޽���
    }

    [Serializable]
    public class BGMClipEntry//bgm���� Ŭ����
    {
        public BGMType type;//� ���� BGM���� ���
        public AudioClip clip;
        public bool loop = true;
    }

    [Serializable]
    public class SFXClipEntry//���� ����Ʈ ���� Ŭ����
    {
        public SFXType type;//� ���ͷ����� ȿ�������� ���
        public AudioClip clip;
        [Range(0.0f, 1.0f)] public float defaultVolume = 1.0f;//�⺻ ��� ����
       [Range(0.5f, 1.5f)] public float pitch = 1.0f;//�⺻ ��� ��ġ
    }

}
