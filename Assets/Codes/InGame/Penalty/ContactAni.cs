using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactAni : MonoBehaviour
{
    public RectTransform a;     // left
    public RectTransform b;     // right
    public RectTransform c;     // vs image

    public GameObject ImpactGame;

    void Start()
    {
        PlaySequence();
    }

    void PlaySequence()
    {
        // 초기 위치 및 스케일 설정
        a.anchoredPosition = new Vector2(-800f, a.anchoredPosition.y);
        b.anchoredPosition = new Vector2(800f, b.anchoredPosition.y);
        c.localScale = Vector3.zero;

        // DOTween 시퀀스 만들기
        Sequence seq = DOTween.Sequence();

        seq.Append(a.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutQuad)) // a가 x: -800 → 0
           .Append(b.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutQuad)) // b가 x: 800 → 0
           .Append(c.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)) // c가 (0,0,0) → (1,1,1)
                  .OnComplete(() =>
                  {
                      StartCoroutine(WaitFirstPanel());
                  });
    }

    IEnumerator WaitFirstPanel()
    {
        yield return new WaitForSeconds(1f);
        ImpactGame.SetActive(true);
        gameObject.SetActive(false);
    }
}
