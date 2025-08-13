using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Networking;
using Unity.VisualScripting;
using System.Collections;

public class BossTextLoader : MonoBehaviour
{
    //상사의 대사와 플레이어의 선택지를 UI에 표시하는 클래스.
    //기본적인 대화 진행(다음 대화로 이동)과 선택지 버튼 생성

    [Serializable]
    public class Choice//Dialogue_Data.json의 "choices"에 해당하는 클래스.
    {
        public int choice_id;
        public string choice_text;
        public int affection_change;
        public int social_score_change;
    }

    [Serializable]
    public class Dialogue//Dialogue_Data.json의 "dialogues"에 해당하는 클래스.
    {
        public int id;
        public string boss_type;
        public string speaker;
        public string dialogue_text;
        public List<Choice> choices;//"dialogue"에 포함된 choices를 리스트로 저장.
    }

    [Serializable]
    public class DialogueData//Dialogue_Data.json 전체를 감싸는 클래스.
    {
        public List<Dialogue> dialogues;//Dialogue_Data.json의 "dialogues"를 리스트로 저장.
    }

    [Header("UI 요소들")]
    [SerializeField] private TextMeshProUGUI bossDialogueText;//상사의 대사가 출력될  TMProUGUI 컴포넌트.
    [SerializeField] private Transform choicesParent;// 선택지 버튼들을 담을 부모 오브젝트.(Vertical Layout Group이 적용된 오브젝트)
    [SerializeField] private GameObject choiceButtonPrefab;//선택지 버튼 프리팹.

    private DialogueData dialogueData;//Dialogue_Data.json의 데이터를 저장할 변수.
    private int currentDialogueIndex = 0;//현재 대화의 인덱스.
    private string selectedBossType;//선택된 상사의 타입

    void Start()
    {
        selectedBossType = PlayerPrefs.GetString("SelectedBoss", "male_boss"); //PlayerPrefs에서 선택된 상사 타입을 가져옴. 기본값은 "male_boss".
        StartCoroutine(LoadDialogueDataFixed());//Dialogue_Data.json 파일을 로드.
        //ShowNextDialogue();//초기 대화 표시. --> LoadDialogueDataFixed()에서 호출함.
    }

    private IEnumerator LoadDialogueDataFixed()//Dialogue_Data.json 파일을 로드할 때 UnityWebRequest를 사용하여 Android APK 압축 구조를 유니티에서 자동 처리하도록 하는 메서드.
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Dialogue_Data.json");//StreamingAssets 폴더에서 Dialogue_Data.json 파일의 경로를 가져옴.
        using (UnityWebRequest request = UnityWebRequest.Get(filePath))// request 객체를 통해 GET으로 파일경로를 가져온다. using 문으로 작업 종료 후 자동 리소스 해제가 수행됨.
        {
            yield return request.SendWebRequest();//파일 로드 완료까지 대기한다.
            if (request.result == UnityWebRequest.Result.Success)//파일을 받아오는데 성공하면
            {
                string json = request.downloadHandler.text;//downloadHandler.text를 사용하여 json을 문자열로 반환한다.
                dialogueData = JsonUtility.FromJson<DialogueData>(json);
                Debug.Log("[BossTextLoader] Android JSON 로드 성공!");
                ShowNextDialogue();//로드 완료 후 UI 업데이트
            }
            else
            {
                Debug.LogError($"[BossTextLoader] Android JSON 로드 실패: {request.error}");
            }
        }
    }

    public void ShowNextDialogue()//다음 대화를 표시하는 메서드.
    {
        if (dialogueData?.dialogues == null) return;// 대화 데이터가 로드되지 않았으면 반환.                                                
        Dialogue currentDialogue = null;
        for (int i = currentDialogueIndex; i < dialogueData.dialogues.Count; i++)//현재 상사 타입과 일치하는 대화 찾기
        {
            if (dialogueData.dialogues[i].boss_type == selectedBossType.Replace("_boss", ""))//선택된 상사 타입과 일치하는 대화를 찾는다.
            {
                currentDialogue = dialogueData.dialogues[i];
                currentDialogueIndex = i;//현재 대화 인덱스를 업데이트.
                break;
            }
        }

        if (currentDialogue == null)
        {
            bossDialogueText.text = "대화가 끝났습니다.";
            return;
        }
        ShowDialogue(currentDialogue);
    }

    public void ShowDialogue(Dialogue dialogue)// 대화를 표시하는 메서드
    {
        bossDialogueText.text = dialogue.dialogue_text;// 상사 대사 표시

        // 기존 선택지 버튼 삭제
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var choice in dialogue.choices)// 선택지 버튼 생성
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicesParent);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = choice.choice_text;
            btnObj.GetComponent<Button>().onClick.AddListener(() => { OnChoiceSelected(choice); });// 버튼 클릭 이벤트 등록
        }
        if (ScoreManager.Instance != null)// ScoreManager에 현재 대화 ID 저장
        {
            ScoreManager.Instance.SetCurrentDialogue(dialogue.id);
        }
    }

    private void OnChoiceSelected(Choice choice)//선택지 버튼 클릭 시 호출되는 메서드.
    {
        Debug.Log($"선택 : {choice.choice_text}, 호감도 변화: {choice.affection_change:+0;-#}, 사회력 변화: {choice.social_score_change:+0;-#}");
        if (ScoreManager.Instance != null)//ScoreManager를 통한 점수 업데이트 진행
        {
            ScoreManager.Instance.UpdateScores(choice.affection_change, choice.social_score_change);
        }
        currentDialogueIndex++;//다음 대화로 이동
        ShowNextDialogue();//다음 대화 표시
    }
}

