using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

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

    void Start()
    {
        StartCoroutine(LoadCharacterDataFix());
    }

    private IEnumerator LoadCharacterDataFix()// File.ReadAllText -> UnityWebRequest 방식으로 변경하여 json데이터를 로드하는 메서드.
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Character_Data.json");
        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                CharacterData data = JsonUtility.FromJson<CharacterData>(json);
                Debug.Log("[BossImageLoader] Character_Data.json 로드 성공!");
                LoadBossSprite(data);
            }
            else
            {
                Debug.LogError($"[BossImageLoader] Character_Data.json 로드 실패: {request.error}");
            }
        }
    }

    private void LoadBossSprite(CharacterData data)//상사 스프라이트 이미지를 로드하는 메서드.
    {
        string selectedBoss = PlayerPrefs.GetString("SelectedBoss", "male_boss");//male_boss는 기본값.
        Character selectedCharacter = null;//List에서 key로 검색
        foreach (var entry in data.characters)
        {
            if (entry.key == selectedBoss)
            {
                selectedCharacter = entry.value;//선택된 보스를 key로 하여 Character타입의 value에 저장.
                break;
            }
        }
         

        if (selectedCharacter == null)
        {
            Debug.LogError("선택된 상사 정보를 찾을 수 없음" + selectedBoss);
            return;
        }

        Debug.Log("선택된 상사 : " + selectedCharacter.name);

        Sprite bossSprite = Resources.Load<Sprite>(selectedCharacter.sprite_path);//Resources에서 Sprite로드

        if (bossSprite == null)
        {
            Debug.LogError("상사의 스프라이트 이미지를 찾을 수 없음" + selectedCharacter.sprite_path);
            return;
        }
        bossImage.sprite = bossSprite;
    }
}
