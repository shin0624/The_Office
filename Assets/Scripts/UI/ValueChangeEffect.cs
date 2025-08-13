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

        string prefix = change > 0 ? "+" : "";//��ȭ���� ������ ��ġ �տ� +�� ���δ�. ������ ��� -�� ���� ���� change�� �����ϱ� prefix�� -�� ���� ������ �ʾƵ� ��.
        affectionValue.text = $"{prefix}{change}";//�������� �տ� ���̰� ��ġ�� �ڿ� ����
        affectionValue.color = change > 0 ? Color.blue : Color.red;//������ ��� �Ķ�, ������ ��� ����

        affectionValue.alpha = 1.0f;
        affectionValue.gameObject.SetActive(true);
        StopCoroutine(HideText(affectionValue));//��� �����ٰ� ���� �ð� �� ���������
        StartCoroutine(HideText(affectionValue));
    }

    public void ShowSocialChange(int change)// ��ȸ�� ���� ���� ��ġ�� �ð�ȭ�ϴ� �޼���
    {
        if (socialValue == null) return;
        if (change == 0) return;//��ġ ��ȭ�� ������ ǥ������ ����

        string prefix = change > 0 ? "+" : "";//��ȭ���� ������ ��ġ �տ� +�� ���δ�. ������ ��� -�� ���� ���� change�� �����ϱ� prefix�� -�� ���� ������ �ʾƵ� ��.
        socialValue.text = $"{prefix}{change}";//�������� �տ� ���̰� ��ġ�� �ڿ� ����
        socialValue.color = change > 0 ? Color.blue : Color.red;//������ ��� �Ķ�, ������ ��� ����

        socialValue.alpha = 1.0f;
        socialValue.gameObject.SetActive(true);
        StopCoroutine(HideText(socialValue));//��� �����ٰ� ���� �ð� �� ���������
        StartCoroutine(HideText(socialValue));
    }

    private IEnumerator HideText(TextMeshProUGUI target)//Dofade�Լ��� ���̵���/�ƿ��� �����ϴ� �ڷ�ƾ
    {
        yield return new WaitForSeconds(displayDuration);
        target.DOFade(0.0f, fadeDuration);
    }
    private void HandleScoresChanged(int affectionDelta, int socialDelta)//ScoreManager���� ���� ��ȭ�� ȣ��Ǿ��� �� �� �̺�Ʈ�� ����ȴ�. OnScoresChanged?.Invoke(affectionChange, socialChange)���� ���� �Ű������� ���� �޼���� ����.
    {
        ShowAffectionChange(affectionDelta);
        ShowSocialChange(socialDelta);
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoresChanged -= HandleScoresChanged;
        }
    }
}
