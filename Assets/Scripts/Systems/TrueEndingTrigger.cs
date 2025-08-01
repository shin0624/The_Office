using System.Collections;
using System.Collections.Generic;
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


    private bool affectionBranch = false;
    private bool rankBranch = false;
    private bool endingTriggered = false;

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
            endingTriggered = true;// �б� ��� � �����̶� �ϳ��� ����Ǹ� �ٷ� endingTringger�� true�� �ǰ�, ���� �߰� ȣ�⿡���� �ߺ� ���� ó�� x --> �̴� ���� ���� �÷��� �ʱ�ȭ �ʿ�. 
            TriggerEnding(EndingType.True);
            Debug.Log("True ���� ����");
        }
        else if (affectionBranch && !rankBranch)//Good���� (ȣ���� true && ���� false)
        {
            endingTriggered = true;
            TriggerEnding(EndingType.Good);
            Debug.Log("Good ���� ����");
        }
        else if (!affectionBranch)//Bad���� (ȣ���� false && ���� false)
        {
            endingTriggered = true;
            TriggerEnding(EndingType.Bad, rankBranch);
            Debug.Log("BAD ���� ����");
        }
    }

    private void TriggerEnding(EndingType type, bool rankBranchValue = false)//���� �б�޼��忡 ������ ���� ���� ������ Ʈ���ŵǴ� �޼���.
    {
        if (endingTriggered) return;//�ߺ� Ʈ���� ����
        endingTriggered = true; // ���� Ŭ���� ���� �ٽ� false�� �����ؾ� ��

        switch (type)// ���� ���� �ʿ��� ����/�ʱ�ȭ/�ݷ��� ���� �� ȣ��
        {
            case EndingType.True:
                Debug.Log("�� True Ending! ȸ�� �ְ� ���� ��� ��");
                // True ���� �ƽ�/�̹���/����/�÷��� �� �߰�
                break;

            case EndingType.Good:
                Debug.Log("�� Good Ending! ���� ���� �ŷڰ��� ���� ��");
                // Good���� ����, ���
                break;

            case EndingType.Bad:
                if (rankBranchValue)
                    Debug.Log("Bad ����(���ΰ���� ��) -- ������ ������ �ذ�");
                else
                    Debug.Log("Bad ����(�ǰ����/����) -- ����ϰ� ����");
                // ���� ����/�޽���/���� �����ϰ� ����
                break;
        }
        // ScoreManager ��� ���� ���� �� �ʱ�ȭ
        // ���� �� MainScene/��缱�� ������ �̵� ����
    }

    public void ResetEndingTrigger()// ���� �б� ���� �� Ʈ���� �ʱ�ȭ �޼���. ���� �ʱ�ȭ�� ScoreManager.cs�� CreateNewGame()�� ���.
    {
        affectionBranch = false;
        rankBranch = false;
        endingTriggered = false;
    }

    public void OnClickReplayOrNextBoss()//���� ���� �絵�� �Ǵ� ���� ��� ���� ȭ������ �Ѿ�� �� ���� ���Ŀ� ȣ��Ǵ� �ʱ�ȭ �޼���.
    {
        ScoreManager.Instance?.CreateNewGame();//��� ����, ���� ������, UI �ʱ�ȭ 
        this.ResetEndingTrigger();//TrueEndingTrigger ���� ���� ����

        //���� ���� �ۼ� ����
    }
}
