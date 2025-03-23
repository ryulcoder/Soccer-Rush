using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class GameCount : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // TextMeshProUGUI ����
    float duration = 0.7f; // �� ������ ���� �ð�
    public Vector3 startScale = Vector3.one; // �ʱ� ũ��
    public Vector3 bigScale = Vector3.one * 1.5f; // Ȯ�� ũ��

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
            countdownText.transform.localScale = startScale; // ��� ũ�� �ʱ�ȭ
            countdownText.text = i.ToString();
            countdownText.transform.localScale = startScale;
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 1);

            countdownText.transform.DOScale(bigScale, duration / 2)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);

            // ���̵� �ƿ� �ִϸ��̼� (TimeScale ����)
            //countdownText.DOFade(0, duration).SetEase(Ease.Linear).SetUpdate(true);

            yield return new WaitForSecondsRealtime(duration); // TimeScale ����
        }

        countdownText.gameObject.SetActive(false); // ī��Ʈ ���� �� ��Ȱ��ȭ
    }
}
