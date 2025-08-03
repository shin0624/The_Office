using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static DataStructures;
public class UIManager : MonoBehaviour
{
    //������ UI�� �����ϴ� �Ŵ���. UI��ҵ��� �ʱ�ȭ �� ������Ʈ�� ���
    [Header("UI ��ҵ�")]
    [SerializeField] private Slider affectionSlider; //ȣ���� �����̴�
    [SerializeField] private Slider socialScoreSlider; //��ȸ�� �����̴�
    [SerializeField] private Slider rankSlider; //���� �����̴�

    [Header("�ؽ�Ʈ��")]
    [SerializeField] private TextMeshProUGUI affectionText; //ȣ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI socialScoreText; //��ȸ�� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI rankText; //���� �ؽ�Ʈ

    void Start()
    {
        if (ScoreManager.Instance != null)//ScoreManager �̺�Ʈ�� ����.
        {
            ScoreManager.Instance.OnAffectionChanged += UpdateAffectionUI; //ȣ���� ���� �� UI ������Ʈ
            ScoreManager.Instance.OnSocialScoreChanged += UpdateSocialScoreUI; //��ȸ�� ���� �� UI ������Ʈ
            ScoreManager.Instance.OnRankChanged += UpdateRankUI; //���� ���� �� UI ������Ʈ
            ScoreManager.Instance.OnGameDataLoaded += OnGameDataLoaded; // ���� ������ �ε� �� UI �ʱ�ȭ

            StartCoroutine(InitializeUIAfterDelay());//UI �ʱ�ȭ�� ������ �� ����
        }
        InitializeSliders(); //�����̴� �ʱ�ȭ
    }

    private IEnumerator InitializeUIAfterDelay()//UI �ʱ�ȭ �ڷ�ƾ �޼���.
    {
        yield return new WaitForSeconds(0.1f); //0.1�� ���
        if (ScoreManager.Instance != null)
        {
            UpdateAffectionUI(ScoreManager.Instance.GetAffectionScore()); //ȣ���� UI ������Ʈ
            UpdateSocialScoreUI(ScoreManager.Instance.GetSocialScore()); //��ȸ�� UI ������Ʈ
            UpdateRankUI(ScoreManager.Instance.GetCurrentRank()); //���� UI ������Ʈ
        }
    }

    private void InitializeSliders()//�����̴� �ʱ�ȭ �޼���.
    {
        if (affectionSlider != null)//ȣ���� �����̴�.
        {
            affectionSlider.minValue = 0;
            affectionSlider.maxValue = 100; //ȣ���� �����̴� ���� ����
            affectionSlider.interactable = false; //ȣ���� �����̴��� ����� �Է� �Ұ�
        }

        if (socialScoreSlider != null)//��ȸ�� �����̴�
        {
            socialScoreSlider.minValue = 0;
            socialScoreSlider.maxValue = 100; //��ȸ�� �����̴� ���� ����
            socialScoreSlider.interactable = false; //��ȸ�� �����̴��� ����� �Է� �Ұ�
        }

        if (rankSlider != null)//���� �����̴�
        {
            rankSlider.minValue = 1;
            rankSlider.maxValue = 12; //���� �����̴� ������ 12�� ���޿� ���� 12�� max��.
            rankSlider.interactable = false; //���� �����̴��� ����� �Է� �Ұ�
        }

    }

    private void UpdateAffectionUI(int affection)//ȣ���� UI ������Ʈ �޼���.
    {
        if (affectionSlider != null)
        {
            affectionSlider.value = affection; //ȣ���� �����̴� �� ������Ʈ
        }
        if (affectionText != null)
        {
            string level = ScoreManager.Instance?.GetAffectionLevel() ?? "����";
            affectionText.text = $"ȣ����: {affection} ({level})"; //ȣ���� �ؽ�Ʈ ������Ʈ
        }

        UpdateAffectionSliderColor(affection); //ȣ���� �����̴� ���� ������Ʈ
    }

    private void UpdateSocialScoreUI(int socialScore)//��ȸ�� UI ������Ʈ �޼���.
    {
        if (socialScoreSlider != null)
        {
            float normalizedScore = Mathf.Clamp(socialScore / 300.0f, 0.0f, 100.0f);//��ȸ���� 0~100 ������ ����ȭ(300�� 100����)
            socialScoreSlider.value = normalizedScore; //��ȸ�� �����̴� �� ������Ʈ
        }
        if (socialScoreText != null)
        {
            socialScoreText.text = $"��ȸ��: {socialScore:N0}"; //��ȸ�� �ؽ�Ʈ ������Ʈ
        }
    }

    private void UpdateRankUI(string rankName)//���� UI ������Ʈ �޼���.
    {
        if (rankSlider != null)
        {
            int rankId = SaveLoadManager.Instance?.GetRankIdByScore(ScoreManager.Instance?.GetSocialScore() ?? 0) ?? 1; //��ȸ�� ������ ���� ���� ID ���
            rankSlider.value = rankId; //���� �����̴� �� ������Ʈ
        }

        if (rankText != null)
        {
            rankText.text = $"����: {rankName}"; //���� �ؽ�Ʈ ������Ʈ
        }
    }

    private void UpdateAffectionSliderColor(int affection)//ȣ���� �����̴� ���� ������Ʈ �޼���. ȣ������ ���� ������ �����.
    {
        if (affectionSlider?.fillRect?.GetComponent<Image>() == null) return;//�����̴��� fillRect�� ������ ��ȯ
        Image fillImage = affectionSlider.fillRect.GetComponent<Image>();//�����̴��� fillRect���� Image ������Ʈ ��������
        if (affection < 30)
        {
            fillImage.color = Color.red; //ȣ������ 30 �̸��� �� �����̴� ������ ���������� ����
        }
        else if (affection < 70)
        {
            fillImage.color = Color.blue; //ȣ������ 30 �̻� 70 �̸��� �� �����̴� ������ �Ķ������� ����
        }
        else
        {
            fillImage.color = Color.green; //ȣ������ 70 �̻��� �� �����̴� ������ �ʷϻ����� ����
        }
    }

    private void OnGameDataLoaded(SaveData saveData)//���� �����Ͱ� �ε�Ǿ��� �� ȣ��Ǵ� �޼���.
    {
        Debug.Log("$[UIManager] ���� ������ �ε�� : {saveData.player_data.player_name}");//�ε��� �÷��̾� �̸� �α� ���
    }

    void OnDestroy()//���� �� �̺�Ʈ ���� ��� ����.
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnAffectionChanged -= UpdateAffectionUI;
            ScoreManager.Instance.OnSocialScoreChanged -= UpdateSocialScoreUI;
            ScoreManager.Instance.OnRankChanged -= UpdateRankUI;
            ScoreManager.Instance.OnGameDataLoaded -= OnGameDataLoaded;
        }
    }
}
