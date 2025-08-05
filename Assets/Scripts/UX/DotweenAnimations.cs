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
    // CollectionScene���� ����� �ִϸ��̼��� å ǥ�� �ѱ��� ���� �ִϸ��̼�
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

    //-------------------------------CollectScene �� �ִϸ��̼�

    public static Tween FlipBookCover(RectTransform coverTransform, float flipAngle = -180.0f, float duration = 1.5f, Ease ease = Ease.OutExpo)//��� ��ø�� ǥ�� 3d ȸ�� �ѱ�� �ִϸ��̼�.
    {
        // return coverTransform.DORotate(new Vector3(0.0f, flipAngle, 0.0f), duration).SetEase(ease);
        return coverTransform.DORotate(new Vector3(0.0f, flipAngle, 0.0f), duration, RotateMode.FastBeyond360).SetEase(ease);
    }

    public static Tween FadeOutBookCover(CanvasGroup canvasGroup, float duration = 0.5f)//��� ��ø ���� ���̵�ƿ� �޼���.
    {
        return canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad);
    }

    public static Tween FadeInBookCover(CanvasGroup canvasGroup, float duration = 0.5f)//��� ��ø ���� ���̵��� �޼���.
    {
        canvasGroup.alpha = 0.0f;
        return canvasGroup.DOFade(1.0f, duration).SetEase(Ease.OutQuad);
    }


    public static void ShowCollectionCard(GameObject card, float duration = 2.0f, float delay = 0.0f)// �����ø �� ���� �÷��� ī�� ���� �ִϸ��̼� �޼���.
    {
        // ī���� ���� ũ�� ����
        Vector3 originalScale = new Vector3(3.45f, 18.89f, 1.0f);
        Vector3 popScale = new Vector3(3.45f * 1.2f, 18.89f * 1.2f, 1.0f); // 1.2�� Ȯ��
        card.transform.localScale = Vector3.zero;
        var canvasGroup = card.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;//�ִϸ��̼� �� ��ȣ�ۿ� ����
        }

        var sequence = DOTween.Sequence();
        sequence.Append(card.transform.DOScale(popScale, duration ));//0 -> 1.2�� ũ��� Ȯ�� + ���̵���
        if (canvasGroup != null)
        {
            sequence.Join(canvasGroup.DOFade(1.0f, duration * 0.6f));
        }
        sequence.Append(card.transform.DOScale(originalScale, duration * 0.4f));//1.2�� -> ���� ũ��� ��� 
        sequence.SetDelay(delay).SetEase(Ease.OutBack).OnComplete(() =>
                                                    {
                                                        card.transform.localScale = originalScale;//���� ���� ����
                                                        if (canvasGroup != null)
                                                        {
                                                            canvasGroup.alpha = 1.0f;
                                                            canvasGroup.interactable = true;//��ȣ�ۿ� ����
                                                        }
                                                    });
        // sequence.Append(card.transform.DOScale(card.transform.localScale*1.2f, duration)).Join(canvasGroup!=null ? canvasGroup.DOFade(1.0f, duration) : null);
        // sequence.Append(card.transform.DOScale(card.transform.localScale, duration));
    }

    public static void ShowCollectionCardsSequentially(List<GameObject> cards, float staggerDelay = 0.1f, float duration = 0.6f)//��� ��ø �� �÷��� ī�� ���� ���� �ִϸ��̼� �޼���.
    {
        for (int i = 0; i < cards.Count; i++)
        {
            float delay = i * staggerDelay;
            ShowCollectionCard(cards[i], duration, delay);
        }
    }

    public static void NewCollectionItemEffect(GameObject item, float duration = 1.0f)// �����ø �� �� ������ ȹ�� ȿ�� �޼���.
    {
        var sequence = DOTween.Sequence();
        var image = item.GetComponent<Image>();
        if (image != null)// Ȳ�ݺ� ���� ȿ��
        {
            Color originalColor = image.color;
            sequence.Append(image.DOColor(Color.yellow, duration * 0.3f)).Append(image.DOColor(originalColor, duration * 0.3f)).SetLoops(2);
        }
        sequence.Join(item.transform.DOScale(1.1f, duration * 0.2f).SetLoops(4, LoopType.Yoyo));// ������ �޽� ȿ��
    }

    public static Tween FadeOutCollectionScene(CanvasGroup canvasGroup, float duration = 0.5f, Action onComplete = null)//�����ø �� ��ü ���̵�ƿ� �޼���.
    {
        return canvasGroup.DOFade(0.0f, duration).SetEase(Ease.InQuad).OnComplete(() => onComplete?.Invoke());
    }

    public static void SlidePageTransition(RectTransform currentPage, RectTransform nextPage, bool slideLeft = true, float duration = 0.5f, Action onComplete = null)//�����ø ������ ��ȯ ȿ�� �޼��� (�¿� �����̵�)
    {
        float slideDistance = currentPage.rect.width;
        Vector3 slideDirection = slideLeft ? Vector3.left : Vector3.right;

        var sequence = DOTween.Sequence();

        // ���� ������ �����̵� �ƿ�
        sequence.Append(currentPage.DOMove(currentPage.position + slideDirection * slideDistance, duration));

        // ���� ������ �����̵� �� (�ݴ� ���⿡�� ����)
        nextPage.position = nextPage.position - slideDirection * slideDistance;
        sequence.Join(nextPage.DOMove(nextPage.position + slideDirection * slideDistance, duration));

        sequence.SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    currentPage.gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
    }

    public static void KillTweensOnTransform(Transform target)// Ư�� Transform�� DOTween �ִϸ��̼� ���� �޼���.
    {
        target.DOKill();
    }
}
