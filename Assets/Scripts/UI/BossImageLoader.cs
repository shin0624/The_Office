using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class BossImageLoader : MonoBehaviour
{
    //MainScene에서 상사 이미지를 로드하는 스크립트.
    [SerializeField] private Image bossImage;

    [System.Serializable]
    public class Character
    {
        public string name;
        public string sprite_path;
    }

    [System.Serializable]
    public class CharacterEntry
    {
        public string key;
        public Character value;
    }

    [System.Serializable]
    public class CharacterData
    {
        public List<CharacterEntry> characters;
    }

    private void Start()
    {
        string selectedBoss = PlayerPrefs.GetString("SelectedBoss", "male_boss");//male_boss는 기본값.
        string path = Path.Combine(Application.streamingAssetsPath, "Character_Data.json");
        string json = File.ReadAllText(path);// 파일에서 텍스트를 읽어 json에 저장 
        CharacterData data = JsonUtility.FromJson<CharacterData>(json);

        Character selectedCharacter = null;//List에서 key로 검색
        foreach (var entry in data.characters)
        {
            if (entry.key == selectedBoss)
            {
                selectedCharacter = entry.value;//선택된 보스를 key로 하여 Character타입의 value에 저장.
                break;
            }
        }
        Debug.Log("선택된 상사 : " + selectedCharacter.name);

        if (selectedCharacter == null)
        {
            Debug.LogError("선택된 상사 정보를 찾을 수 없음" + selectedBoss);
            return;
        }

        Sprite bossSprite = Resources.Load<Sprite>(selectedCharacter.sprite_path);//Resources에서 Sprite로드
        if (bossSprite == null)
        {
            Debug.LogError("상사의 스프라이트 이미지를 찾을 수 없음" + selectedCharacter.sprite_path);
            return;
        }
        bossImage.sprite = bossSprite;
    }
}
