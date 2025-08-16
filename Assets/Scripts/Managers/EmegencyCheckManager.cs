using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmegencyCheckManager : MonoBehaviour
{
    //StartScene 진입 시 LastQuitMethod == EmergencyQuit를 읽어서 이전 세션이 비정상 종료일 경우 비정상 종료 히스토리를 표시하도록 설정하는 스크립트.
    [SerializeField] private GameObject emergencyPanel;//긴급저장을 안내하는 패널
    [SerializeField] private TextMeshProUGUI emergencyText;//긴급저장을 안내하는 텍스트
    [SerializeField] private Button closeButton;//닫기 버튼

    void Start()
    {
        ShowEmergencyNoticePanel();
    }

    private void ShowEmergencyNoticePanel()//게임 시작 시 마지막 저장 방법이 EmergencyQuit이면 경고 팝업을 띄우는 메서드.
    {
        if (SaveLoadManager.Instance == null) return;

        if (SaveLoadManager.Instance.WasLastQuitEmergency())//WasLastQuitEmergency==true인 경우 : 이전 종료방법이 긴급종료였음.
        {
            var save = ScoreManager.Instance?.GetCurrentSaveData();//현재 세이브데이터 로드
            string timeStamp = save?.timestamp ?? "알 수 없음";//세이브데이터의 타임스탬프를 가져온다.

            if (emergencyPanel != null)
            {
                emergencyPanel.SetActive(true);
                AudioManager.Instance.PlaySFX(AudioEnums.SFXType.PanelOpen);
                if (emergencyText != null)
                {
                    emergencyText.text =
                    $"이전 세션이 비정상 종료되어 긴급 저장을 수행했습니다.\n" +
                    $"저장 시각 : {timeStamp}\n" +
                    $"문제가 반복되면 개발자에게 문의해 주세요.";
                }
                if (closeButton != null)//닫기 버튼에 패널클로즈, 긴급저장 플래그 초기화를 붙인다.
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
                Debug.LogWarning("[StartScene] emergencyBanner가 할당되지 않았습니다. 로그로 대체합니다.");
                Debug.Log($"[StartScene] 응급 저장 시각: {timeStamp}");
                SaveLoadManager.Instance.ClearLastQuitFlag();
            }
        }
    }
}
