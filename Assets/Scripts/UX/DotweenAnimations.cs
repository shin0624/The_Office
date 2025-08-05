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
    // CollectionScene에서 사용할 애니메이션은 책 표지 넘기기와 같은 애니메이션
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

    //-------------------------------CollectScene 용 애니메이션

    public static Tween FlipBookCover(RectTransform coverTransform, float flipAngle = -180.0f, float duration = 1.5f, Ease ease = Ease.OutExpo)//사원 수첩의 표지 3d 회전 넘기기 애니메이션.
    {
        // return coverTransform.DORotate(new Vector3(0.0f, flipAngle, 0.0f), duration).SetEase(ease);
        return coverTransform.DORotate(new Vector3(0.0f, flipAngle, 0.0f), duration, RotateMode.FastBeyond360).SetEase(ease);
    }

    public static Tween FadeOutBookCover(CanvasGroup canvasGroup, float duration = 0.5f)//사원 수첩 내지 페이드아웃 메서드.
    {
        return canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad);
    }

    public static Tween FadeInBookCover(CanvasGroup canvasGroup, float duration = 0.5f)//사원 수첩 내지 페이드인 메서드.
    {
        canvasGroup.alpha = 0.0f;
        return canvasGroup.DOFade(1.0f, duration).SetEase(Ease.OutQuad);
    }


    public static void ShowCollectionCard(GameObject card, float duration = 2.0f, float delay = 0.0f)// 사원수첩 내 엔딩 컬렉션 카드 등장 애니메이션 메서드.
    {
        // 카드의 원본 크기 저장
        Vector3 originalScale = new Vector3(3.45f, 18.89f, 1.0f);
        Vector3 popScale = new Vector3(3.45f * 1.2f, 18.89f * 1.2f, 1.0f); // 1.2배 확대
        card.transform.localScale = Vector3.zero;
        var canvasGroup = card.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;//애니메이션 중 상호작용 차단
        }

        var sequence = DOTween.Sequence();
        sequence.Append(card.transform.DOScale(popScale, duration ));//0 -> 1.2배 크기로 확대 + 페이드인
        if (canvasGroup != null)
        {
            sequence.Join(canvasGroup.DOFade(1.0f, duration * 0.6f));
        }
        sequence.Append(card.transform.DOScale(originalScale, duration * 0.4f));//1.2배 -> 원본 크기로 축소 
        sequence.SetDelay(delay).SetEase(Ease.OutBack).OnComplete(() =>
                                                    {
                                                        card.transform.localScale = originalScale;//최종 상태 보장
                                                        if (canvasGroup != null)
                                                        {
                                                            canvasGroup.alpha = 1.0f;
                                                            canvasGroup.interactable = true;//상호작용 복구
                                                        }
                                                    });
        // sequence.Append(card.transform.DOScale(card.transform.localScale*1.2f, duration)).Join(canvasGroup!=null ? canvasGroup.DOFade(1.0f, duration) : null);
        // sequence.Append(card.transform.DOScale(card.transform.localScale, duration));
    }

    public static void ShowCollectionCardsSequentially(List<GameObject> cards, float staggerDelay = 0.1f, float duration = 0.6f)//사원 수첩 내 컬렉션 카드 순차 등장 애니메이션 메서드.
    {
        for (int i = 0; i < cards.Count; i++)
        {
            float delay = i * staggerDelay;
            ShowCollectionCard(cards[i], duration, delay);
        }
    }

    public static void NewCollectionItemEffect(GameObject item, float duration = 1.0f)// 사원수첩 내 새 아이템 획득 효과 메서드.
    {
        var sequence = DOTween.Sequence();
        var image = item.GetComponent<Image>();
        if (image != null)// 황금빛 나는 효과
        {
            Color originalColor = image.color;
            sequence.Append(image.DOColor(Color.yellow, duration * 0.3f)).Append(image.DOColor(originalColor, duration * 0.3f)).SetLoops(2);
        }
        sequence.Join(item.transform.DOScale(1.1f, duration * 0.2f).SetLoops(4, LoopType.Yoyo));// 스케일 펄스 효과
    }

    public static Tween FadeOutCollectionScene(CanvasGroup canvasGroup, float duration = 0.5f, Action onComplete = null)//사원수첩 씬 전체 페이드아웃 메서드.
    {
        return canvasGroup.DOFade(0.0f, duration).SetEase(Ease.InQuad).OnComplete(() => onComplete?.Invoke());
    }

    public static void SlidePageTransition(RectTransform currentPage, RectTransform nextPage, bool slideLeft = true, float duration = 0.5f, Action onComplete = null)//사원수첩 페이지 전환 효과 메서드 (좌우 슬라이드)
    {
        float slideDistance = currentPage.rect.width;
        Vector3 slideDirection = slideLeft ? Vector3.left : Vector3.right;

        var sequence = DOTween.Sequence();

        // 현재 페이지 슬라이드 아웃
        sequence.Append(currentPage.DOMove(currentPage.position + slideDirection * slideDistance, duration));

        // 다음 페이지 슬라이드 인 (반대 방향에서 시작)
        nextPage.position = nextPage.position - slideDirection * slideDistance;
        sequence.Join(nextPage.DOMove(nextPage.position + slideDirection * slideDistance, duration));

        sequence.SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    currentPage.gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
    }

    public static void KillTweensOnTransform(Transform target)// 특정 Transform의 DOTween 애니메이션 정지 메서드.
    {
        target.DOKill();
    }
}
