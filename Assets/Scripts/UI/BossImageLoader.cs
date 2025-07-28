using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BossImageLoader : MonoBehaviour
{
    //MainScene���� ��� �̹����� �ε��ϴ� ��ũ��Ʈ.
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

    private IEnumerator LoadCharacterDataFix()// File.ReadAllText -> UnityWebRequest ������� �����Ͽ� json�����͸� �ε��ϴ� �޼���.
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Character_Data.json");
        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                CharacterData data = JsonUtility.FromJson<CharacterData>(json);
                Debug.Log("[BossImageLoader] Character_Data.json �ε� ����!");
                LoadBossSprite(data);
            }
            else
            {
                Debug.LogError($"[BossImageLoader] Character_Data.json �ε� ����: {request.error}");
            }
        }
    }

    private void LoadBossSprite(CharacterData data)//��� ��������Ʈ �̹����� �ε��ϴ� �޼���.
    {
        string selectedBoss = PlayerPrefs.GetString("SelectedBoss", "male_boss");//male_boss�� �⺻��.
        Character selectedCharacter = null;//List���� key�� �˻�
        foreach (var entry in data.characters)
        {
            if (entry.key == selectedBoss)
            {
                selectedCharacter = entry.value;//���õ� ������ key�� �Ͽ� CharacterŸ���� value�� ����.
                break;
            }
        }
         

        if (selectedCharacter == null)
        {
            Debug.LogError("���õ� ��� ������ ã�� �� ����" + selectedBoss);
            return;
        }

        Debug.Log("���õ� ��� : " + selectedCharacter.name);

        Sprite bossSprite = Resources.Load<Sprite>(selectedCharacter.sprite_path);//Resources���� Sprite�ε�

        if (bossSprite == null)
        {
            Debug.LogError("����� ��������Ʈ �̹����� ã�� �� ����" + selectedCharacter.sprite_path);
            return;
        }
        bossImage.sprite = bossSprite;
    }
}
