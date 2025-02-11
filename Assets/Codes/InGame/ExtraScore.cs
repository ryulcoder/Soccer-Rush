using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraScore : MonoBehaviour
{
    public static ExtraScore instance;

    float totalScore;

    Player Player;
    BallMove BallMove;

    DefenderFootTrigger nowDefender;

    [Header("[ ExtraScore ]")]
    [SerializeField] float[] extraScore;
    [SerializeField] float[] moveExtra = new float[] { 0, 10 };
    [SerializeField] float[] skillExtra = new float[] { 15, 30 };

    [Header("[ ExtraScore UI]")]
    public Transform ExtraScoreTextParent;
    public GameObject ExtraScoreTextPrefab;
    string scoreText;

    [Header("[ Limit ]")]
    public float limitDis = 28;
    public float limitTime = 0.5f;

    bool isLimit, scoreCoroutine, isCoroutine;
    string scoreType;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Player = Player.Instance;
        BallMove = BallMove.instance;
    }

    void Update()
    {
        if (scoreCoroutine)
        {
            scoreCoroutine = false;
            StartCoroutine(CheckExtraScore(scoreType));
        }
        
    }

    // 태클 당할시 최종 추가 스코어 전달
    public void CheckEndScore()
    {
        Debug.LogWarning("최종스코어: " + totalScore);
    }

    // 플레이어 스킬 사용시 추가점수
    public void CheckStart(string scoreType)
    {
        if (scoreCoroutine) return;

        this.scoreType = scoreType;

        switch (scoreType)
        {
            case "AvoidMove":
                extraScore = moveExtra;
                scoreText = "무빙!\n";
                break;
            case "AvoidSkill":
                extraScore = skillExtra;
                scoreText = "회피!\n";
                break;
        }

        scoreCoroutine = true;
    }

    // 플레이어 스킬 사용시 추가점수 코루틴
    IEnumerator CheckExtraScore(string scoreType)
    {
        // 극한 회피 점수 확인
        if (isLimit)
        {
            // 좌우 회피 시 플레이어 태클과 라인 확인
            if (scoreType == "AvoidMove")
            {
                yield return new WaitForSeconds(0.2f);

                if (BallMove.instance.isTackled || Player.Instance.getTackled || nowDefender.LineCheck())
                {
                    yield break;
                }
            }

            totalScore += extraScore[1];
            CreateExtraScoreText("아슬아슬 "+scoreText, extraScore[1]);
        }
        else
        {
            if (extraScore[0] == 0) yield break;

            totalScore += extraScore[0];
            CreateExtraScoreText(scoreText, extraScore[0]);
        }

        yield break;
    }
    
    // limit 좌 우 회피 라인 확인
    public void LimitStart(DefenderFootTrigger nowDefender)
    {
        isLimit = true;
        this.nowDefender = nowDefender;
    }
    public void LimitEnd()
    {
        isLimit = false;
    }


    // 추가 점수 ui
    void CreateExtraScoreText(string text, float score)
    {
        GameObject ExtraScoreText = Instantiate(ExtraScoreTextPrefab, ExtraScoreTextParent);
        ExtraScoreText.GetComponent<ExtraScoreUI>().score = text + "  + " + score;
        ExtraScoreText.SetActive(true);

        Debug.LogWarning("추가점수 +" + score);
    }

}
