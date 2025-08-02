using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static DataStructures;
public class UIManager : MonoBehaviour
{
    //게임의 UI를 관리하는 매니저. UI요소들의 초기화 및 업데이트를 담당
    [Header("UI 요소들")]
    [SerializeField] private Slider affectionSlider; //호감도 슬라이더
    [SerializeField] private Slider socialScoreSlider; //사회력 슬라이더
    [SerializeField] private Slider rankSlider; //직급 슬라이더

    [Header("텍스트들")]
    [SerializeField] private TextMeshProUGUI affectionText; //호감도 텍스트
    [SerializeField] private TextMeshProUGUI socialScoreText; //사회력 텍스트
    [SerializeField] private TextMeshProUGUI rankText; //직급 텍스트

    void Start()
    {
        if (ScoreManager.Instance != null)//ScoreManager 이벤트를 구독.
        {
            ScoreManager.Instance.OnAffectionChanged += UpdateAffectionUI; //호감도 변경 시 UI 업데이트
            ScoreManager.Instance.OnSocialScoreChanged += UpdateSocialScoreUI; //사회력 변경 시 UI 업데이트
            ScoreManager.Instance.OnRankChanged += UpdateRankUI; //직급 변경 시 UI 업데이트
            ScoreManager.Instance.OnGameDataLoaded += OnGameDataLoaded; // 게임 데이터 로드 시 UI 초기화

            StartCoroutine(InitializeUIAfterDelay());//UI 초기화는 딜레이 후 실행
        }
        InitializeSliders(); //슬라이더 초기화
    }

    private IEnumerator InitializeUIAfterDelay()//UI 초기화 코루틴 메서드.
    {
        yield return new WaitForSeconds(0.1f); //0.1초 대기
        if (ScoreManager.Instance != null)
        {
            UpdateAffectionUI(ScoreManager.Instance.GetAffectionScore()); //호감도 UI 업데이트
            UpdateSocialScoreUI(ScoreManager.Instance.GetSocialScore()); //사회력 UI 업데이트
            UpdateRankUI(ScoreManager.Instance.GetCurrentRank()); //직급 UI 업데이트
        }
    }

    private void InitializeSliders()//슬라이더 초기화 메서드.
    {
        if (affectionSlider != null)//호감도 슬라이더.
        {
            affectionSlider.minValue = 0;
            affectionSlider.maxValue = 100; //호감도 슬라이더 범위 설정
            affectionSlider.interactable = false; //호감도 슬라이더는 사용자 입력 불가
        }

        if (socialScoreSlider != null)//사회력 슬라이더
        {
            socialScoreSlider.minValue = 0;
            socialScoreSlider.maxValue = 100; //사회력 슬라이더 범위 설정
            socialScoreSlider.interactable = false; //사회력 슬라이더는 사용자 입력 불가
        }

        if (rankSlider != null)//직급 슬라이더
        {
            rankSlider.minValue = 1;
            rankSlider.maxValue = 12; //직급 슬라이더 범위는 12개 직급에 따라 12가 max값.
            rankSlider.interactable = false; //직급 슬라이더는 사용자 입력 불가
        }

    }

    private void UpdateAffectionUI(int affection)//호감도 UI 업데이트 메서드.
    {
        if (affectionSlider != null)
        {
            affectionSlider.value = affection; //호감도 슬라이더 값 업데이트
        }
        if (affectionText != null)
        {
            string level = ScoreManager.Instance?.GetAffectionLevel() ?? "보통";
            affectionText.text = $"호감도: {affection} ({level})"; //호감도 텍스트 업데이트
        }

        UpdateAffectionSliderColor(affection); //호감도 슬라이더 색상 업데이트
    }

    private void UpdateSocialScoreUI(int socialScore)//사회력 UI 업데이트 메서드.
    {
        if (socialScoreSlider != null)
        {
            float normalizedScore = Mathf.Clamp(socialScore / 300.0f, 0.0f, 100.0f);//사회력을 0~100 범위로 정규화(300를 100으로)
            socialScoreSlider.value = normalizedScore; //사회력 슬라이더 값 업데이트
        }
        if (socialScoreText != null)
        {
            socialScoreText.text = $"사회력: {socialScore:N0}"; //사회력 텍스트 업데이트
        }
    }

    private void UpdateRankUI(string rankName)//직급 UI 업데이트 메서드.
    {
        if (rankSlider != null)
        {
            int rankId = SaveLoadManager.Instance?.GetRankIdByScore(ScoreManager.Instance?.GetSocialScore() ?? 0) ?? 1; //사회력 점수에 따른 직급 ID 계산
            rankSlider.value = rankId; //직급 슬라이더 값 업데이트
        }

        if (rankText != null)
        {
            rankText.text = $"직급: {rankName}"; //직급 텍스트 업데이트
        }
    }

    private void UpdateAffectionSliderColor(int affection)//호감도 슬라이더 색상 업데이트 메서드. 호감도에 따라 색상이 변경됨.
    {
        if (affectionSlider?.fillRect?.GetComponent<Image>() == null) return;//슬라이더의 fillRect가 없으면 반환
        Image fillImage = affectionSlider.fillRect.GetComponent<Image>();//슬라이더의 fillRect에서 Image 컴포넌트 가져오기
        if (affection < 30)
        {
            fillImage.color = Color.red; //호감도가 30 미만일 때 슬라이더 색상을 빨간색으로 변경
        }
        else if (affection < 70)
        {
            fillImage.color = Color.blue; //호감도가 30 이상 70 미만일 때 슬라이더 색상을 파란색으로 변경
        }
        else
        {
            fillImage.color = Color.green; //호감도가 70 이상일 때 슬라이더 색상을 초록색으로 변경
        }
    }

    private void OnGameDataLoaded(SaveData saveData)//게임 데이터가 로드되었을 때 호출되는 메서드.
    {
        Debug.Log("$[UIManager] 게임 데이터 로드됨 : {saveData.player_data.player_name}");//로딩된 플레이어 이름 로그 출력
    }

    void OnDestroy()//종료 시 이벤트 구독 모두 해제.
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
