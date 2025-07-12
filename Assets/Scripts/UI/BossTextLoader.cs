using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using TMPro;

public class BossTextLoader : MonoBehaviour
{
    // MainScene에서 상사의 다이얼로그를 로드하는 스크립트.
    [System.Serializable]
    public class Choice
    {
        public int choice_id;
        public string choice_text;
        public int affection_change;
        public int social_score_change;
    }

    [System.Serializable]
    public class Dialogue
    {
        public int id;
        public string boss_type;
        public string speaker;
        public string dialogue_text;
        public List<Choice> choices;
    }

    [System.Serializable]
    public class DialogueData
    {
        public List<Dialogue> dialogues;
    }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI bossDialogue;
    [SerializeField] private TextMeshProUGUI choiceFirst;
    [SerializeField] private TextMeshProUGUI choiceSecond;
    [SerializeField] private TextMeshProUGUI choiceThird;

    private DialogueData dialogueData;
    private int curretnDialogueIndex = 0;

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogue_Data.json");//json파일 경로를 가져온다.
    }


}
