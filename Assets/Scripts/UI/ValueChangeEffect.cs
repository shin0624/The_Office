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

    //250816_증감치 연출 중 다음 선택지 입력 시 바로 새로운 증감치 연출이 시작되도록 수정
    private Coroutine affectionCoroutine;//호감도 텍스트용 Hide코루틴 핸들(중복방지/중단용)
    private Coroutine socialCoroutine;//사회력 텍스트용 Hide코루틴 핸들(중복방지/중단용)

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

        KillTweenAndStopCoroutine(affectionValue, ref affectionCoroutine);//진행 중인 트윈 및 코루틴 즉시 중단, 정리.

        string prefix = change > 0 ? "+" : "";//변화량이 있으면 수치 앞에 +를 붙인다. 감소할 경우 -를 붙인 값이 change로 들어오니까 prefix에 -를 따로 붙이지 않아도 됨.
        affectionValue.text = $"{prefix}{change}";//전위식을 앞에 붙이고 수치를 뒤에 붙임
        affectionValue.color = change > 0 ? Color.blue : Color.red;//증가의 경우 파랑, 감소의 경우 빨강

        affectionValue.gameObject.SetActive(true);
        affectionValue.alpha = 1.0f;

        affectionCoroutine = StartCoroutine(HideTextAfterDelay(affectionValue));//새 코루틴으로 표시 유지 후 페이드아웃 수행
    }

    public void ShowSocialChange(int change)// 사회력 점수 증감 수치를 시각화하는 메서드
    {
        if (socialValue == null) return;
        if (change == 0) return;//수치 변화가 없으면 표시하지 않음

        KillTweenAndStopCoroutine(socialValue, ref socialCoroutine);//진행 중인 트윈 및 코루틴 즉시 중단, 정리

        string prefix = change > 0 ? "+" : "";//변화량이 있으면 수치 앞에 +를 붙인다. 감소할 경우 -를 붙인 값이 change로 들어오니까 prefix에 -를 따로 붙이지 않아도 됨.
        socialValue.text = $"{prefix}{change}";//전위식을 앞에 붙이고 수치를 뒤에 붙임
        socialValue.color = change > 0 ? Color.blue : Color.red;//증가의 경우 파랑, 감소의 경우 빨강

        socialValue.gameObject.SetActive(true);
        socialValue.alpha = 1.0f;

        socialCoroutine = StartCoroutine(HideTextAfterDelay(socialValue));//새 코루틴으로 표시 유지 후 페이드아웃 수행
    }

    // private IEnumerator HideText(TextMeshProUGUI target)//Dofade함수로 페이드인/아웃을 수행하는 코루틴 --> 228016. HideTextAfterDelay()로 대체
    // {
    //     yield return new WaitForSeconds(displayDuration);
    //     target.DOFade(0.0f, fadeDuration);
    // }

    private void HandleScoresChanged(int affectionDelta, int socialDelta)//ScoreManager에서 점수 변화가 호출되었을 때 이 이벤트가 실행된다. OnScoresChanged?.Invoke(affectionChange, socialChange)에서 각각 매개변수가 변경 메서드로 들어간다.
    {
        ShowAffectionChange(affectionDelta);//호감도 증감치 즉시 시각화
        ShowSocialChange(socialDelta);//사회력 증감치 즉시 시각화
    }

    private void KillTweenAndStopCoroutine(TextMeshProUGUI target, ref Coroutine handle)//진행 중인 트윈과 코루틴을 안전하게 정리하는 메서드.
    {
        if (target != null) target.DOKill(true);//해당 target에 걸린 DOTeen을 모두 종료하고 마지막 값을 적용
        if (handle != null)//기존 코루틴 존재 시
        {
            StopCoroutine(handle);//코루틴 중지(대기/페이드 중이던 흐름 강제 단절)
            handle = null;//핸들을 비워서 재시작 준비
        }
    }

    private IEnumerator HideTextAfterDelay(TextMeshProUGUI target)// 일정 시간 표시 후 target을 페이드아웃하는 코루틴
    {
        yield return new WaitForSeconds(displayDuration);//표시 유지 시간만큼 대기
        if (target != null)//타겟이 여전히 유효하면
        {
            target.DOKill(true);//기존 트윈 정리
            target.DOFade(0.0f, fadeDuration);//새롭게 0으로 페이드아웃 시작 
        }
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoresChanged -= HandleScoresChanged;
        }

        //250816. 진행 중이던 트윈, 코루틴 모두 안전하게 정리
        if (affectionValue != null) affectionValue.DOKill(true);//호감도 텍스트의 진행 중이던 트윈 종료
        if (socialValue != null) socialValue.DOKill(true);//사회력 텍스트 진행 중이던 트윈 종료
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
