using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public enum EndingType { True, Good, Bad}//���� Ÿ�� ����ü ����
public class TrueEndingTrigger : MonoBehaviour
{
    // True Ending���� �б��ϴ� Ʈ���� ��ũ��Ʈ.
    // True Ending �б� ���� : ȣ���� 100 ���� Bad�Ӱ谪 �̻� ���� && ������� ����
    // ���� ���� �� True Ending �ƽ� ��� -> ���� �÷��ǿ� �߰��ǰ� ���� ������ ���� ������ �ʱ�ȭ��.
    // ScoreManager.cs�� ȣ����, ���� üũ �Լ����� ���� ���� ���� �� bool���� �Ѿ�� ���̸�, �� �Լ� ��� true���� �Ѿ�� ��� True��������, �� �� �ϳ��� True�� ��� Good, �� �� False�� ��� Bad�� ����.
    // ��ȭ�� ���� ����
    // 	a. [ȣ������ 100 ���� && low �Ӱ谪 �̻��� �����ϸ� && �÷��̾ ������� ���� ->  True����] (True / True)
    // 	b. [����� ȣ������ 100�� �� ���� + ��纸�� ������ ���� ���� -> Good ����(�ŷڰ��� ���� ���� �� ������ ������ ����)](True / False)
    // 	c. [����� ȣ������ BAD �Ӱ�ġ ���� + ��纸�� ������ ���� ���� -> Bad ����(�������� OR �ǰ���� ��)](False / False)
    // 	d. [����� ȣ������ BAD �Ӱ�ġ ���� + ��纸�� ������ ���� ���� -> Bad ����(���ΰ�߷� �ذ� ��)](False / True)

    //250805. TriggerEnding()�� ���� ���� �� �÷��ǿ� ī�� �߰� ��� ����.

    private bool affectionBranch = false;
    private bool rankBranch = false;
    private bool endingTriggered = false;
    [SerializeField] private EndingUIController endingUIController;

    void OnEnable()
    {
        var scoreManager = ScoreManager.Instance;
        if (scoreManager != null)
            scoreManager.OnEndingBranchChanged += OnEndingBranchCheck;
    }
    void OnDisable()
    {
        var scoreManager = ScoreManager.Instance;
        if (scoreManager != null)
            scoreManager.OnEndingBranchChanged -= OnEndingBranchCheck;
    }
    void Start()
    {
        Debug.Log($"endingTriggered = {endingTriggered}");
    }

    private void OnEndingBranchCheck(bool value, EndingBranchType which)//ScoreManager���� �Ѿ�� t/f���� �÷��׿� ���� ���� �б⸦ �����ϴ� �޼���.
    {
        Debug.Log($"[TrueEndingTrigger] ���� �귣ġ üũ: {which} = {value}");
        if (which == EndingBranchType.Affection) affectionBranch = value;
        if (which == EndingBranchType.Rank) rankBranch = value;

        CheckEndingBranch();
    }

    private void CheckEndingBranch()//�� value ����κ��� ������ �б��ϴ� �޼���.
    {
        if (!ScoreManager.Instance.IsEndingBranchEnabled()) return;//�÷��̾� ������ "����" �̸��� ���� �ƿ� �б����� �ʵ��� ������ġ�� �߰��Ͽ� ���� ���� ���� bad������ ������ ���ܸ� ����.

        if (endingTriggered) return;//���� Ʈ���� false�̸� �б�X (���� ��ȿȭ �÷��״� ScoreManager ���� �б⿡�� �̹� ���͸� ��.)

        if (affectionBranch && rankBranch)//True���� (ȣ���� true && ���� true)
        {
            Debug.Log("True ���� ����");
            TriggerEnding(EndingType.True);
        }
        else if (affectionBranch && !rankBranch)//Good���� (ȣ���� true && ���� false)
        {
            Debug.Log("Good ���� ����");
            TriggerEnding(EndingType.Good);
        }
        else if (!affectionBranch)//Bad���� (ȣ���� false && ���� false)
        {
            Debug.Log("BAD ���� ����");
            TriggerEnding(EndingType.Bad, rankBranch);
            
        }
    }

    //250805. ���� Ʈ���� �� ���� ī�� ��� ���� ��� �߰�.
    private void TriggerEnding(EndingType type, bool rankBranchValue = false)//���� �б�޼��忡 ������ ���� ���� ������ Ʈ���ŵǴ� �޼���.
    {
        if (endingTriggered) return;//�ߺ� Ʈ���� ����
        endingTriggered = true; // ���� Ŭ���� ���� �ٽ� false�� �����ؾ� ��

        string selectBoss = PlayerPrefs.GetString("SelectedBoss", "male_boss");//���� ������ ��� ���� ��������
        if (CollectionManager.Instance != null)// ���� ī�� ��� ����.
        {
            CollectionManager.Instance.UnlockEndingCard(type, selectBoss);//���� ���õ� ��� ���ڿ� ���� ���� ���� Ÿ���� �Ű������� �Ͽ�, �׿� �ش��ϴ� ����ī�带 �ر�.
        }
        endingUIController.ShowEnding(type);
        Debug.Log($"[TrueEndingTrigger]{type} ���� Ʈ���� �Ϸ� �� �÷��� ī�� �ر�");
    }

    private void ResetEndingTrigger()// ���� �б� ���� �� Ʈ���� �ʱ�ȭ �޼���. ���� �ʱ�ȭ�� ScoreManager.cs�� CreateNewGame()�� ���.
    {
        Debug.Log("[TrueEndingTrigger] ���� Ʈ���� ����");
        affectionBranch = false;
        rankBranch = false;
        endingTriggered = false;
    }

    public void OnClickReplayOrNextBoss()//���� ���� �絵�� �Ǵ� ���� ��� ���� ȭ������ �Ѿ�� �� ���� ���Ŀ� ȣ��Ǵ� �ʱ�ȭ �޼���.
    {
        var oldSave = ScoreManager.Instance?.GetCurrentSaveData();
        var backupCollection = oldSave != null ? oldSave.player_data?.collectionData : null;//������ �� �Ŀ��� �÷��� ������ �̿��� ��� �����Ͱ� �ʱ�ȭ�Ǿ�� �ϹǷ� �÷��� ���

        ScoreManager.Instance?.CreateNewGame();//��� ����, ���� ������, UI �ʱ�ȭ 

        var newSave = ScoreManager.Instance?.GetCurrentSaveData();
        if (backupCollection != null && newSave != null && newSave.player_data != null)
        {
            newSave.player_data.collectionData = backupCollection;//�÷��� ����
            SaveLoadManager.Instance.SaveGameData(newSave);
        }
        this.ResetEndingTrigger();//TrueEndingTrigger ���� ���� ����
    }
}
