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


    // 인터넷 확인 함수
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
        float duration = 3f; // 애니메이션 지속 시간
        Vector3 moveOffset = new Vector3(0, 100, 0); // 이동 거리

        checkText.gameObject.SetActive(true);
        checkText.color = new Color(checkText.color.r, checkText.color.g, checkText.color.b, 1); // 알파값 초기화

        // 현재 위치에서 moveOffset 만큼 위로 이동
        checkText.rectTransform.anchoredPosition += new Vector2(0, -moveOffset.y);

        // DOTween 애니메이션 실행
        checkText.rectTransform.DOAnchorPosY(checkText.rectTransform.anchoredPosition.y + moveOffset.y, duration);
        checkText.DOFade(0, duration).OnComplete(() =>
        {
            checkText.gameObject.SetActive(false); // 애니메이션 완료 후 비활성화
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
