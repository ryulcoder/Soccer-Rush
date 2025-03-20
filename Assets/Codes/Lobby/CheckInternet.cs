using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CheckInternet : MonoBehaviour
{
    public static CheckInternet instance;

    public TextMeshProUGUI checkText;
    public GameObject RankingPanel;
    public GameObject LoadingImage;


    // ���ͳ� Ȯ�� �Լ�
    void Check(int num)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowInternetMessage();
        }
        else
        {
            switch (num)
            {
                case 0:
                    RankingPanel.SetActive(true);
                    break;
                case 1:
                    LoadingImage.SetActive(true);
                    break;
            }
        }
    }

    // 
    void ShowInternetMessage()
    {
        float duration = 3f; // �ִϸ��̼� ���� �ð�
        Vector3 moveOffset = new Vector3(0, 100, 0); // �̵� �Ÿ�

        checkText.gameObject.SetActive(true);
        checkText.color = new Color(checkText.color.r, checkText.color.g, checkText.color.b, 1); // ���İ� �ʱ�ȭ

        // ���� ��ġ���� moveOffset ��ŭ ���� �̵�
        checkText.rectTransform.anchoredPosition += new Vector2(0, -moveOffset.y);

        // DOTween �ִϸ��̼� ����
        checkText.rectTransform.DOAnchorPosY(checkText.rectTransform.anchoredPosition.y + moveOffset.y, duration);
        checkText.DOFade(0, duration).OnComplete(() =>
        {
            checkText.gameObject.SetActive(false); // �ִϸ��̼� �Ϸ� �� ��Ȱ��ȭ
        });
    }

    public void RankingInternetCheck()
    {
        Check(0);
    }

    public void PlayInternetCheck()
    {
        Check(1);
    }

}
