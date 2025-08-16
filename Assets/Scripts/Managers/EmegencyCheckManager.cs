using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmegencyCheckManager : MonoBehaviour
{
    //StartScene ���� �� LastQuitMethod == EmergencyQuit�� �о ���� ������ ������ ������ ��� ������ ���� �����丮�� ǥ���ϵ��� �����ϴ� ��ũ��Ʈ.
    [SerializeField] private GameObject emergencyPanel;//��������� �ȳ��ϴ� �г�
    [SerializeField] private TextMeshProUGUI emergencyText;//��������� �ȳ��ϴ� �ؽ�Ʈ
    [SerializeField] private Button closeButton;//�ݱ� ��ư

    void Start()
    {
        ShowEmergencyNoticePanel();
    }

    private void ShowEmergencyNoticePanel()//���� ���� �� ������ ���� ����� EmergencyQuit�̸� ��� �˾��� ���� �޼���.
    {
        if (SaveLoadManager.Instance == null) return;

        if (SaveLoadManager.Instance.WasLastQuitEmergency())//WasLastQuitEmergency==true�� ��� : ���� �������� ������ῴ��.
        {
            var save = ScoreManager.Instance?.GetCurrentSaveData();//���� ���̺굥���� �ε�
            string timeStamp = save?.timestamp ?? "�� �� ����";//���̺굥������ Ÿ�ӽ������� �����´�.

            if (emergencyPanel != null)
            {
                emergencyPanel.SetActive(true);
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.PanelOpen);
                if (emergencyText != null)
                {
                    emergencyText.text =
                    $"���� ������ ������ ����Ǿ� ��� ������ �����߽��ϴ�.\n" +
                    $"���� �ð� : {timeStamp}\n" +
                    $"������ �ݺ��Ǹ� �����ڿ��� ������ �ּ���.";
                }
                if (closeButton != null)//�ݱ� ��ư�� �г�Ŭ����, ������� �÷��� �ʱ�ȭ�� ���δ�.
                {
                    closeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.AddListener(() =>
                    {
                        emergencyPanel.SetActive(false);
                        AudioManager.Instance.PlaySFX(AudioEnums.SFXType.PanelClose);
                        SaveLoadManager.Instance.ClearLastQuitFlag();
                    });
                }
            }
            else
            {
                Debug.LogWarning("[StartScene] emergencyBanner�� �Ҵ���� �ʾҽ��ϴ�. �α׷� ��ü�մϴ�.");
                Debug.Log($"[StartScene] ���� ���� �ð�: {timeStamp}");
                SaveLoadManager.Instance.ClearLastQuitFlag();
            }
        }
    }
}
