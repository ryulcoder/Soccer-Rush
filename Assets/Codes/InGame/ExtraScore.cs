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
    [SerializeField] float[] shootExtra = new float[] { 100 };

    [Header("[ ExtraScore Particle ]")]
    [SerializeField] GameObject ExtraParticle;
    public GameObject MoveExtraParticle;
    public GameObject SpinExtraParticle;
    public GameObject JumpExtraParticle;

    [Header("[ Limit ]")]
    public float limitDis = 28;
    public float limitTime = 0.5f;

    public bool tutorial;

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
    public float CheckEndScore()
    {
        return totalScore;
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
                ExtraParticle = MoveExtraParticle;
                break;
            case "AvoidJump":
                extraScore = skillExtra;
                ExtraParticle = JumpExtraParticle;
                break;
            case "AvoidSpin":
                extraScore = skillExtra;
                ExtraParticle = SpinExtraParticle;
                break;
            case "HitShoot":
                extraScore = shootExtra;
                ExtraParticle = null;
                break;
        }

        scoreCoroutine = true;
    }

    // 플레이어 스킬 사용시 추가점수 코루틴
    IEnumerator CheckExtraScore(string scoreType)
    {
        if (scoreType == "HitShoot")
        {
            if (!tutorial)
                totalScore += extraScore[0];
            //Debug.LogWarning("추가점수 +" + extraScore[0]);

            yield break;
        }

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

            if (ExtraParticle)
            {
                if (ExtraParticle.activeSelf)
                    ExtraParticle.SetActive(false);

                ExtraParticle.SetActive(true);
                if (LobbyAudioManager.instance != null) 
                { 
                    LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.bonusPoint);
                }

            }

            if (!tutorial)
                totalScore += extraScore[1];
            //Debug.LogWarning("추가점수 +" + extraScore[1]);
        }
        else
        {
            if (extraScore[0] == 0) yield break;

            if (!tutorial)
                totalScore += extraScore[0];
            //Debug.LogWarning("추가점수 +" + extraScore[0]);
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


}
