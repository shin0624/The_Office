using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class BossTextLoader : MonoBehaviour
{
    //����� ���� �÷��̾��� �������� UI�� ǥ���ϴ� Ŭ����.
    //�⺻���� ��ȭ ����(���� ��ȭ�� �̵�)�� ������ ��ư ����

    [Serializable]
    public class Choice//Dialogue_Data.json�� "choices"�� �ش��ϴ� Ŭ����.
    {
        public int choice_id;
        public string choice_text;
        public int affection_change;
        public int social_score_change;
    }

    [Serializable]
    public class Dialogue//Dialogue_Data.json�� "dialogues"�� �ش��ϴ� Ŭ����.
    {
        public int id;
        public string boss_type;
        public string speaker;
        public string dialogue_text;
        public List<Choice> choices;//"dialogue"�� ���Ե� choices�� ����Ʈ�� ����.
    }

    [Serializable]
    public class DialogueData//Dialogue_Data.json ��ü�� ���δ� Ŭ����.
    {
        public List<Dialogue> dialogues;//Dialogue_Data.json�� "dialogues"�� ����Ʈ�� ����.
    }

    [Header("UI ��ҵ�")]
    [SerializeField] private TextMeshProUGUI bossDialogueText;//����� ��簡 ��µ�  TMProUGUI ������Ʈ.
    [SerializeField] private Transform choicesParent;// ������ ��ư���� ���� �θ� ������Ʈ.(Vertical Layout Group�� ����� ������Ʈ)
    [SerializeField] private GameObject choiceButtonPrefab;//������ ��ư ������.

    private DialogueData dialogueData;//Dialogue_Data.json�� �����͸� ������ ����.
    private int currentDialogueIndex = 0;//���� ��ȭ�� �ε���.
    private string selectedBossType;//���õ� ����� Ÿ��

    void Start()
    {
        selectedBossType = PlayerPrefs.GetString("SelectedBoss", "male_boss"); //PlayerPrefs���� ���õ� ��� Ÿ���� ������. �⺻���� "male_boss".
        LoadDialogueData();//Dialogue_Data.json ������ �ε�.
        ShowNextDialogue();//�ʱ� ��ȭ ǥ��.
    }

    private void LoadDialogueData()//Dialogue_Data.json ������ �ε��ϴ� �޼���.
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogue_Data.json");//StreamingAssets �������� Dialogue_Data.json ������ ��θ� ������.
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path, Encoding.UTF8);//������ ������ �о��.
            dialogueData = JsonUtility.FromJson<DialogueData>(json);//�о�� JSON ���ڿ��� DialogueData ��ü�� ��ȯ.
            Debug.Log("[BossTextLoader] Dialogue_Data.json �ε� �Ϸ�");
        }
        else
        {
            Debug.LogError("[BossTextLoader] Dialogue_Data.json ������ ã�� �� �����ϴ�.");
        }
    }

    public void ShowNextDialogue()//���� ��ȭ�� ǥ���ϴ� �޼���.
    {
        if (dialogueData?.dialogues == null) return;// ��ȭ �����Ͱ� �ε���� �ʾ����� ��ȯ.                                                
        Dialogue currentDialogue = null;//���� ��� Ÿ�԰� ��ġ�ϴ� ��ȭ ã��
        for (int i = currentDialogueIndex; i < dialogueData.dialogues.Count; i++)
        {
            if (dialogueData.dialogues[i].boss_type == selectedBossType.Replace("_boss", ""))//���õ� ��� Ÿ�԰� ��ġ�ϴ� ��ȭ�� ã�´�.
            {
                currentDialogue = dialogueData.dialogues[i];
                currentDialogueIndex = i;//���� ��ȭ �ε����� ������Ʈ.
                break;
            }
        }

        if (currentDialogue == null)
        {
            bossDialogueText.text = "��ȭ�� �������ϴ�.";
            return;
        }
        ShowDialogue(currentDialogue);
    }

    public void ShowDialogue(Dialogue dialogue)// ��ȭ�� ǥ���ϴ� �޼���
    {
        bossDialogueText.text = dialogue.dialogue_text;// ��� ��� ǥ��

        // ���� ������ ��ư ����
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var choice in dialogue.choices)// ������ ��ư ����
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicesParent);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = choice.choice_text;
            btnObj.GetComponent<Button>().onClick.AddListener(() => { OnChoiceSelected(choice); });// ��ư Ŭ�� �̺�Ʈ ���
        }
        if (ScoreManager.Instance != null)// ScoreManager�� ���� ��ȭ ID ����
        {
            ScoreManager.Instance.SetCurrentDialogue(dialogue.id);
        }
    }

    private void OnChoiceSelected(Choice choice)//������ ��ư Ŭ�� �� ȣ��Ǵ� �޼���.
    {
        Debug.Log($"���� : {choice.choice_text}, ȣ���� ��ȭ: {choice.affection_change:+0;-#}, ��ȸ�� ��ȭ: {choice.social_score_change:+0;-#}");
        if (ScoreManager.Instance != null)//ScoreManager�� ���� ���� ������Ʈ ����
        {
            ScoreManager.Instance.UpdateScores(choice.affection_change, choice.social_score_change);
        }
        currentDialogueIndex++;//���� ��ȭ�� �̵�
        ShowNextDialogue();//���� ��ȭ ǥ��
    }
}

