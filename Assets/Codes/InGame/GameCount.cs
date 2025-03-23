using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class GameCount : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // TextMeshProUGUI 연결
    float duration = 0.7f; // 각 숫자의 지속 시간
    public Vector3 startScale = Vector3.one; // 초기 크기
    public Vector3 bigScale = Vector3.one * 1.5f; // 확대 크기

    private void OnEnable()
    {
        StartCoroutine(CountdownRoutine());
    }

    private void OnDisable()
    {
        GameManager.Instance.GameResume();
    }

    IEnumerator CountdownRoutine()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.transform.localScale = startScale; // 즉시 크기 초기화
            countdownText.text = i.ToString();
            countdownText.transform.localScale = startScale;
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 1);

            countdownText.transform.DOScale(bigScale, duration / 2)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);

            // 페이드 아웃 애니메이션 (TimeScale 무시)
            //countdownText.DOFade(0, duration).SetEase(Ease.Linear).SetUpdate(true);

            yield return new WaitForSecondsRealtime(duration); // TimeScale 무시
        }

        countdownText.gameObject.SetActive(false); // 카운트 종료 후 비활성화
    }
}
