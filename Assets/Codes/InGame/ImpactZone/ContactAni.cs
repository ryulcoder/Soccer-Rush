using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContactAni : MonoBehaviour
{
    public RectTransform a;     // left
    public RectTransform b;     // right
    public RectTransform c;     // vs image

    public GameObject background;
    public GameObject vsImage;
    public GameObject IZLabel;
    public ImpactGame impactGame;

    int goalkeeperStage;
    public TextMeshProUGUI goalkeeperName;
    public TextMeshProUGUI playerName;

    public string[] goalkeeperNames;
 


    void Awake()
    {
        if (PlayerPrefs.HasKey("nickname"))
        {
            playerName.text = PlayerPrefs.GetString("nickname");
        }
        
    }
    void OnEnable()
    {
        IZLabel.SetActive(true);
        PlaySequence();
        impactGame.goalkeeperStage = goalkeeperStage;
        goalkeeperName.text = goalkeeperNames[goalkeeperStage];
        if(goalkeeperStage < goalkeeperNames.Length - 1)
        {
            goalkeeperStage++;
        }
    }

    void PlaySequence()
    {
        // 초기 위치 및 스케일 설정
        a.anchoredPosition = new Vector2(-800f, 0);
        b.anchoredPosition = new Vector2(800f, 0);
        c.localScale = Vector3.zero;

        // DOTween 시퀀스 만들기
        Sequence seq = DOTween.Sequence();

        seq.Append(a.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutQuad))
           .Append(b.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutQuad))
           .Append(c.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack))
           .OnComplete(() =>
           {
               StartCoroutine(WaitFirstPanel());
           });
    }

    IEnumerator WaitFirstPanel()
    {
        yield return new WaitForSeconds(1f);
        vsImage.SetActive(false);
        background.SetActive(false);
        IZLabel.SetActive(false);

        Sequence seq2 = DOTween.Sequence();
        seq2.Append(a.DOAnchorPosY(-570f, 0.5f).SetEase(Ease.OutQuad))
            .Join(b.DOAnchorPosY(570f, 0.5f).SetEase(Ease.OutQuad))
            .OnComplete(() =>
            {
                StartCoroutine(WaitMovePanel());
            });
    }

    IEnumerator WaitMovePanel()
    {
        yield return new WaitForSeconds(1f);
        impactGame.StartGame();
    }

    private void OnDisable()
    {
        vsImage.SetActive(true);
        background.SetActive(true);

    }
}
