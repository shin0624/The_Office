using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ValueChangeEffect : MonoBehaviour
{
    // UI ���� ��ȸ�� ����, ȣ���� ������ġ�� �ð�ȭ�ϰ� ���̵���/�ƿ� ȿ���� �����ϴ� �޼��带 ������ Ŭ����
    [SerializeField] private TextMeshProUGUI affectionValue;
    [SerializeField] private TextMeshProUGUI socialValue;
    private float displayDuration = 0.5f;//���� ǥ�� ���� �ð�
    private float fadeDuration = 0.5f;// ���̵� �ð�

    //250816_����ġ ���� �� ���� ������ �Է� �� �ٷ� ���ο� ����ġ ������ ���۵ǵ��� ����
    private Coroutine affectionCoroutine;//ȣ���� �ؽ�Ʈ�� Hide�ڷ�ƾ �ڵ�(�ߺ�����/�ߴܿ�)
    private Coroutine socialCoroutine;//��ȸ�� �ؽ�Ʈ�� Hide�ڷ�ƾ �ڵ�(�ߺ�����/�ߴܿ�)

    void OnEnable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoresChanged += HandleScoresChanged;
        affectionValue.alpha = 0.0f;
        socialValue.alpha = 0.0f;
    }

    public void ShowAffectionChange(int change)// ȣ���� ���� ��ġ�� �ð�ȭ�ϴ� �޼���. change�� ��ġ.
    {
        if (affectionValue == null) return;
        if (change == 0) return;//��ġ ��ȭ�� ������ ǥ������ ����

        KillTweenAndStopCoroutine(affectionValue, ref affectionCoroutine);//���� ���� Ʈ�� �� �ڷ�ƾ ��� �ߴ�, ����.

        string prefix = change > 0 ? "+" : "";//��ȭ���� ������ ��ġ �տ� +�� ���δ�. ������ ��� -�� ���� ���� change�� �����ϱ� prefix�� -�� ���� ������ �ʾƵ� ��.
        affectionValue.text = $"{prefix}{change}";//�������� �տ� ���̰� ��ġ�� �ڿ� ����
        affectionValue.color = change > 0 ? Color.blue : Color.red;//������ ��� �Ķ�, ������ ��� ����

        affectionValue.gameObject.SetActive(true);
        affectionValue.alpha = 1.0f;

        affectionCoroutine = StartCoroutine(HideTextAfterDelay(affectionValue));//�� �ڷ�ƾ���� ǥ�� ���� �� ���̵�ƿ� ����
    }

    public void ShowSocialChange(int change)// ��ȸ�� ���� ���� ��ġ�� �ð�ȭ�ϴ� �޼���
    {
        if (socialValue == null) return;
        if (change == 0) return;//��ġ ��ȭ�� ������ ǥ������ ����

        KillTweenAndStopCoroutine(socialValue, ref socialCoroutine);//���� ���� Ʈ�� �� �ڷ�ƾ ��� �ߴ�, ����

        string prefix = change > 0 ? "+" : "";//��ȭ���� ������ ��ġ �տ� +�� ���δ�. ������ ��� -�� ���� ���� change�� �����ϱ� prefix�� -�� ���� ������ �ʾƵ� ��.
        socialValue.text = $"{prefix}{change}";//�������� �տ� ���̰� ��ġ�� �ڿ� ����
        socialValue.color = change > 0 ? Color.blue : Color.red;//������ ��� �Ķ�, ������ ��� ����

        socialValue.gameObject.SetActive(true);
        socialValue.alpha = 1.0f;

        socialCoroutine = StartCoroutine(HideTextAfterDelay(socialValue));//�� �ڷ�ƾ���� ǥ�� ���� �� ���̵�ƿ� ����
    }

    // private IEnumerator HideText(TextMeshProUGUI target)//Dofade�Լ��� ���̵���/�ƿ��� �����ϴ� �ڷ�ƾ --> 228016. HideTextAfterDelay()�� ��ü
    // {
    //     yield return new WaitForSeconds(displayDuration);
    //     target.DOFade(0.0f, fadeDuration);
    // }

    private void HandleScoresChanged(int affectionDelta, int socialDelta)//ScoreManager���� ���� ��ȭ�� ȣ��Ǿ��� �� �� �̺�Ʈ�� ����ȴ�. OnScoresChanged?.Invoke(affectionChange, socialChange)���� ���� �Ű������� ���� �޼���� ����.
    {
        ShowAffectionChange(affectionDelta);//ȣ���� ����ġ ��� �ð�ȭ
        ShowSocialChange(socialDelta);//��ȸ�� ����ġ ��� �ð�ȭ
    }

    private void KillTweenAndStopCoroutine(TextMeshProUGUI target, ref Coroutine handle)//���� ���� Ʈ���� �ڷ�ƾ�� �����ϰ� �����ϴ� �޼���.
    {
        if (target != null) target.DOKill(true);//�ش� target�� �ɸ� DOTeen�� ��� �����ϰ� ������ ���� ����
        if (handle != null)//���� �ڷ�ƾ ���� ��
        {
            StopCoroutine(handle);//�ڷ�ƾ ����(���/���̵� ���̴� �帧 ���� ����)
            handle = null;//�ڵ��� ����� ����� �غ�
        }
    }

    private IEnumerator HideTextAfterDelay(TextMeshProUGUI target)// ���� �ð� ǥ�� �� target�� ���̵�ƿ��ϴ� �ڷ�ƾ
    {
        yield return new WaitForSeconds(displayDuration);//ǥ�� ���� �ð���ŭ ���
        if (target != null)//Ÿ���� ������ ��ȿ�ϸ�
        {
            target.DOKill(true);//���� Ʈ�� ����
            target.DOFade(0.0f, fadeDuration);//���Ӱ� 0���� ���̵�ƿ� ���� 
        }
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoresChanged -= HandleScoresChanged;
        }

        //250816. ���� ���̴� Ʈ��, �ڷ�ƾ ��� �����ϰ� ����
        if (affectionValue != null) affectionValue.DOKill(true);//ȣ���� �ؽ�Ʈ�� ���� ���̴� Ʈ�� ����
        if (socialValue != null) socialValue.DOKill(true);//��ȸ�� �ؽ�Ʈ ���� ���̴� Ʈ�� ����
        if (affectionCoroutine != null)
        {
            StopCoroutine(affectionCoroutine);
            affectionCoroutine = null;
        }
        if (socialCoroutine != null)
        {
            StopCoroutine(socialCoroutine);
            socialCoroutine = null;
        }
    }
}
