using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ValueChangeEffect : MonoBehaviour
{
    // UI 에서 사회력 점수, 호감도 증감수치를 시각화하고 페이드인/아웃 효과를 연출하는 메서드를 정의한 클래스
    [SerializeField] private TextMeshProUGUI affectionValue;
    [SerializeField] private TextMeshProUGUI socialValue;
    private float displayDuration = 0.5f;//점수 표시 지속 시간
    private float fadeDuration = 0.5f;// 페이드 시간

    void OnEnable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoresChanged += HandleScoresChanged;
        affectionValue.alpha = 0.0f;
        socialValue.alpha = 0.0f;
    }

    public void ShowAffectionChange(int change)// 호감도 증감 수치를 시각화하는 메서드. change는 수치.
    {
        if (affectionValue == null) return;
        if (change == 0) return;//수치 변화가 없으면 표시하지 않음

        string prefix = change > 0 ? "+" : "";//변화량이 있으면 수치 앞에 +를 붙인다. 감소할 경우 -를 붙인 값이 change로 들어오니까 prefix에 -를 따로 붙이지 않아도 됨.
        affectionValue.text = $"{prefix}{change}";//전위식을 앞에 붙이고 수치를 뒤에 붙임
        affectionValue.color = change > 0 ? Color.blue : Color.red;//증가의 경우 파랑, 감소의 경우 빨강

        affectionValue.alpha = 1.0f;
        affectionValue.gameObject.SetActive(true);
        StopCoroutine(HideText(affectionValue));//즉시 보였다가 일정 시간 후 사라지도록
        StartCoroutine(HideText(affectionValue));
    }

    public void ShowSocialChange(int change)// 사회력 점수 증감 수치를 시각화하는 메서드
    {
        if (socialValue == null) return;
        if (change == 0) return;//수치 변화가 없으면 표시하지 않음

        string prefix = change > 0 ? "+" : "";//변화량이 있으면 수치 앞에 +를 붙인다. 감소할 경우 -를 붙인 값이 change로 들어오니까 prefix에 -를 따로 붙이지 않아도 됨.
        socialValue.text = $"{prefix}{change}";//전위식을 앞에 붙이고 수치를 뒤에 붙임
        socialValue.color = change > 0 ? Color.blue : Color.red;//증가의 경우 파랑, 감소의 경우 빨강

        socialValue.alpha = 1.0f;
        socialValue.gameObject.SetActive(true);
        StopCoroutine(HideText(socialValue));//즉시 보였다가 일정 시간 후 사라지도록
        StartCoroutine(HideText(socialValue));
    }

    private IEnumerator HideText(TextMeshProUGUI target)//Dofade함수로 페이드인/아웃을 수행하는 코루틴
    {
        yield return new WaitForSeconds(displayDuration);
        target.DOFade(0.0f, fadeDuration);
    }
    private void HandleScoresChanged(int affectionDelta, int socialDelta)//ScoreManager에서 점수 변화가 호출되었을 때 이 이벤트가 실행된다. OnScoresChanged?.Invoke(affectionChange, socialChange)에서 각각 매개변수가 변경 메서드로 들어간다.
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
