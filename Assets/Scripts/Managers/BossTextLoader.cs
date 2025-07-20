using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using TMPro;

public class BossTextLoader : MonoBehaviour
{
    //����� ���� �÷��̾��� �������� UI�� ǥ���ϴ� Ŭ����.
    //�⺻���� ��ȭ ����(���� ��ȭ�� �̵�)�� ������ ��ư ����

    [System.Serializable]
    public class Choice//Dialogue_Data.json�� "choices"�� �ش��ϴ� Ŭ����.
    {
        public int choice_id;
        public string choice_text;
        public int affection_change;
        public int social_score_change;
    }

    [System.Serializable]
    public class Dialogue//Dialogue_Data.json�� "dialogues"�� �ش��ϴ� Ŭ����.
    {
        public int id;
        public string boss_type;
        public string speaker;
        public string dialogue_text;
        public List<Choice> choices;//"dialogue"�� ���Ե� choices�� ����Ʈ�� ����.
    }

    [System.Serializable]
    public class DialogueData//Dialogue_Data.json ��ü�� ���δ� Ŭ����.
    {
        public List<Dialogue> dialogues;//Dialogue_Data.json�� "dialogues"�� ����Ʈ�� ����.
    }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI bossDialogueText;//����� ��簡 ��µ�  TMProUGUI ������Ʈ.
    [SerializeField] private Transform choicesParent;// ������ ��ư���� ���� �θ� ������Ʈ.(Vertical Layout Group�� ����� ������Ʈ)
    [SerializeField] private GameObject choiceButtonPrefab;//������ ��ư ������.

    private DialogueData dialogueData;//Dialogue_Data.json�� �����͸� ������ ����.
    private int currentDialogueIndex = 0;//���� ��ȭ�� �ε���.

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogue_Data.json");//StreamingAssets �������� Dialogue_Data.json ������ ��θ� ������.
        string json = File.ReadAllText(path, Encoding.UTF8);//������ ������ �о��.
        dialogueData = JsonUtility.FromJson<DialogueData>(json);//�о�� JSON ���ڿ��� DialogueData ��ü�� ��ȯ.
        ShowDialogue(currentDialogueIndex);//�ʱ� ��ȭ ǥ��.
    }

    public void ShowDialogue(int index)// ��ȭ�� ǥ���ϴ� �޼���
    {
        if (index < 0 || index > dialogueData.dialogues.Count)//�ε����� ��ȿ���� ������ ��ȯ.
            return;

        Dialogue dialogue = dialogueData.dialogues[index];//���� �ε����� �ش��ϴ� ��ȭ ��ü�� ������.
        bossDialogueText.text = dialogue.dialogue_text;//����� ��� �ؽ�Ʈ�� ���
        foreach (Transform child in choicesParent)//������ ��ư���� �ʱ�ȭ
        {
            Destroy(child.gameObject);//������ ������ ��ư���� ��� ����.
        }

        foreach (var choice in dialogue.choices)// ������ ��ư�� ����.
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choicesParent);//������ ��ư �������� �ν��Ͻ�ȭ�Ͽ� �θ� ������Ʈ�� �߰�.
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = choice.choice_text;//������ �ؽ�Ʈ�� ����.

            int nextIndex = currentDialogueIndex + 1;//���� ��ȭ �ε��� ���.
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log($"������ : {choice.choice_text} | ȣ���� ��ȭ : {choice.affection_change} | ��ȸ�� ���� ��ȭ : {choice.social_score_change}");//���� ��ȭ ó��
                currentDialogueIndex = nextIndex;//���� ��ȭ �ε����� ������Ʈ.
                
                if (currentDialogueIndex < dialogueData.dialogues.Count)//���� ��ȭ�� �����ϸ�
                {
                    ShowDialogue(currentDialogueIndex);//���� ��ȭ�� ǥ��.
                }
                else
                {
                    Debug.Log("��ȭ�� �������ϴ�."); // ��ȭ�� �������� �˸�
                }
            });
        }

    }
}

