using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using TMPro;

public class BossTextLoader : MonoBehaviour
{
    //상사의 대사와 플레이어의 선택지를 UI에 표시하는 클래스.
    //기본적인 대화 진행(다음 대화로 이동)과 선택지 버튼 생성

    [System.Serializable]
    public class Choice//Dialogue_Data.json의 "choices"에 해당하는 클래스.
    {
        public int choice_id;
        public string choice_text;
        public int affection_change;
        public int social_score_change;
    }

    [System.Serializable]
    public class Dialogue//Dialogue_Data.json의 "dialogues"에 해당하는 클래스.
    {
        public int id;
        public string boss_type;
        public string speaker;
        public string dialogue_text;
        public List<Choice> choices;//"dialogue"에 포함된 choices를 리스트로 저장.
    }

    [System.Serializable]
    public class DialogueData//Dialogue_Data.json 전체를 감싸는 클래스.
    {
        public List<Dialogue> dialogues;//Dialogue_Data.json의 "dialogues"를 리스트로 저장.
    }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI bossDialogueText;//상사의 대사가 출력될  TMProUGUI 컴포넌트.
    [SerializeField] private Transform choicesParent;// 선택지 버튼들을 담을 부모 오브젝트.(Vertical Layout Group이 적용된 오브젝트)
    [SerializeField] private GameObject choiceButtonPrefab;//선택지 버튼 프리팹.

    private DialogueData dialogueData;//Dialogue_Data.json의 데이터를 저장할 변수.
    private int currentDialogueIndex = 0;//현재 대화의 인덱스.

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogue_Data.json");//StreamingAssets 폴더에서 Dialogue_Data.json 파일의 경로를 가져옴.
        string json = File.ReadAllText(path, Encoding.UTF8);//파일의 내용을 읽어옴.
        dialogueData = JsonUtility.FromJson<DialogueData>(json);//읽어온 JSON 문자열을 DialogueData 객체로 변환.
        ShowDialogue(currentDialogueIndex);//초기 대화 표시.
    }

    public void ShowDialogue(int index)// 대화를 표시하는 메서드
    {
        if (index < 0 || index > dialogueData.dialogues.Count)//인덱스가 유효하지 않으면 반환.
            return;

        Dialogue dialogue = dialogueData.dialogues[index];//현재 인덱스에 해당하는 대화 객체를 가져옴.
        bossDialogueText.text = dialogue.dialogue_text;//상사의 대사 텍스트를 출력
        foreach (Transform child in choicesParent)//선택지 버튼들을 초기화
        {
            Destroy(child.gameObject);//기존의 선택지 버튼들을 모두 제거.
        }

        foreach (var choice in dialogue.choices)// 선택지 버튼을 생성.
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicesParent);//선택지 버튼 프리팹을 인스턴스화하여 부모 오브젝트에 추가.
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = choice.choice_text;//선택지 텍스트를 설정.

            int nextIndex = currentDialogueIndex + 1;//다음 대화 인덱스 계산.
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log($"선택지 : {choice.choice_text} | 호감도 변화 : {choice.affection_change} | 사회적 점수 변화 : {choice.social_score_change}");//점수 변화 처리
                currentDialogueIndex = nextIndex;//현재 대화 인덱스를 업데이트.
                
                if (currentDialogueIndex < dialogueData.dialogues.Count)//다음 대화가 존재하면
                {
                    ShowDialogue(currentDialogueIndex);//다음 대화를 표시.
                }
                else
                {
                    Debug.Log("대화가 끝났습니다."); // 대화가 끝났음을 알림
                }
            });
        }

    }
}

