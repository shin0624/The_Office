using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public static class DotweenAnimations
{
    // DOTween ���� �޼��� ���� Ŭ����.
    // ���� �б�(true, good, bad)�� ���߾ �̹���, Ÿ��Ʋ�� ���� ��Ұ� �ٸ� ��.
    private static bool isInitialized = false;
    public static void DotweenInit()
    {
        if (!isInitialized)
        {
            DOTween.Init();
            isInitialized = true;
        }
    }

    public static void FadeInBackground(CanvasGroup canvasGroup, float duration = 1.0f)//���� �г� ���̵��ο� ���Ǵ� �޼���.
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.DOFade(1.0f, duration).SetEase(Ease.OutQuad);//ĵ�����׷��� ���İ��� 0->1�� duration ���� �ε巴�� ��ȭ��Ŵ.
    }

    public static void FadeInCutsceneImage(Image image, float duration = 1.0f, float delay = 0.5f)//���� �̹����� ���̵��� �ϴ� �޼���.
    {
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        image.DOFade(1.0f, duration).SetDelay(delay).SetEase(Ease.InOutQuad);//�̹����� delay �Ŀ� ���̵���
    }

    public static void FadeInEndingTitle(Image titleText, float duration = 1.0f, float delay = 0.5f)//���� Ÿ��Ʋ ���̵��� �޼���. Ÿ��Ʋ�� canva�� ���� �̹��� ���.
    {
        var sequence = DOTween.Sequence();
        sequence.Append(titleText.DOFade(1.0f, duration).SetDelay(delay).SetEase(Ease.InOutQuad));//Ÿ��Ʋ�� delay �Ŀ� ���̵��� 
    }

    public static void FadeInEndingButtons(List<Button> buttons, float duration = 1.0f, float delay = 0.5f)// ��ư ���̵��� �޼���
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            button.GetComponent<CanvasGroup>().alpha = 0.0f;
            var sequence = DOTween.Sequence();
            sequence.Append(button.GetComponent<CanvasGroup>().DOFade(1.0f, duration).SetDelay(delay).SetEase(Ease.InOutQuad));
        }
    }

    public static void FadeOutEndingPanel(CanvasGroup canvasGroup, float duration = 1.0f, Action onComplete = null)//���� �г� ��ü ���̵�ƿ� �� ��Ȱ��ȭ �ϴ� �޼���.
    {
        canvasGroup.DOFade(0.0f, duration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            canvasGroup.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

}
