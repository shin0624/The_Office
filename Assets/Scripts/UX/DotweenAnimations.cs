using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public static class DotweenAnimations
{
    // DOTween 래핑 메서드 모음 클래스.
    // 엔딩 분기(true, good, bad)에 맞추어서 이미지, 타이틀에 들어가는 요소가 다를 것.
    private static bool isInitialized = false;
    public static void DotweenInit()
    {
        if (!isInitialized)
        {
            DOTween.Init();
            isInitialized = true;
        }
    }

    public static void FadeInBackground(CanvasGroup canvasGroup, float duration = 1.0f)//엔딩 패널 페이드인에 사용되는 메서드.
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.DOFade(1.0f, duration).SetEase(Ease.OutQuad);//캔버스그룹의 알파값을 0->1로 duration 동안 부드럽게 변화시킴.
    }

    public static void FadeInCutsceneImage(Image image, float duration = 1.0f, float delay = 0.5f)//엔딩 이미지를 페이드인 하는 메서드.
    {
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        image.DOFade(1.0f, duration).SetDelay(delay).SetEase(Ease.InOutQuad);//이미지를 delay 후에 페이드인
    }

    public static void FadeInEndingTitle(Image titleText, float duration = 1.0f, float delay = 0.5f)//엔딩 타이틀 페이드인 메서드. 타이틀은 canva로 만든 이미지 사용.
    {
        var sequence = DOTween.Sequence();
        sequence.Append(titleText.DOFade(1.0f, duration).SetDelay(delay).SetEase(Ease.InOutQuad));//타이틀을 delay 후에 페이드인 
    }

    public static void FadeInEndingButtons(List<Button> buttons, float duration = 1.0f, float delay = 0.5f)// 버튼 페이드인 메서드
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            button.GetComponent<CanvasGroup>().alpha = 0.0f;
            var sequence = DOTween.Sequence();
            sequence.Append(button.GetComponent<CanvasGroup>().DOFade(1.0f, duration).SetDelay(delay).SetEase(Ease.InOutQuad));
        }
    }

    public static void FadeOutEndingPanel(CanvasGroup canvasGroup, float duration = 1.0f, Action onComplete = null)//엔딩 패널 전체 페이드아웃 후 비활성화 하는 메서드.
    {
        canvasGroup.DOFade(0.0f, duration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            canvasGroup.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

}
