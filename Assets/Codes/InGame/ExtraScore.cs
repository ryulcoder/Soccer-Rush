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

    // ��Ŭ ���ҽ� ���� �߰� ���ھ� ����
    public void CheckEndScore()
    {
        Debug.LogWarning("�������ھ�: " + totalScore);
    }

    // �÷��̾� ��ų ���� �߰�����
    public void CheckStart(string scoreType)
    {
        if (scoreCoroutine) return;

        this.scoreType = scoreType;

        switch (scoreType)
        {
            case "AvoidMove":
                extraScore = moveExtra;
                scoreText = "����!\n";
                break;
            case "AvoidSkill":
                extraScore = skillExtra;
                scoreText = "ȸ��!\n";
                break;
        }

        scoreCoroutine = true;
    }

    // �÷��̾� ��ų ���� �߰����� �ڷ�ƾ
    IEnumerator CheckExtraScore(string scoreType)
    {
        // ���� ȸ�� ���� Ȯ��
        if (isLimit)
        {
            // �¿� ȸ�� �� �÷��̾� ��Ŭ�� ���� Ȯ��
            if (scoreType == "AvoidMove")
            {
                yield return new WaitForSeconds(0.2f);

                if (BallMove.instance.isTackled || Player.Instance.getTackled || nowDefender.LineCheck())
                {
                    yield break;
                }
            }

            totalScore += extraScore[1];
            CreateExtraScoreText("�ƽ��ƽ� "+scoreText, extraScore[1]);
        }
        else
        {
            if (extraScore[0] == 0) yield break;

            totalScore += extraScore[0];
            CreateExtraScoreText(scoreText, extraScore[0]);
        }

        yield break;
    }
    
    // limit �� �� ȸ�� ���� Ȯ��
    public void LimitStart(DefenderFootTrigger nowDefender)
    {
        isLimit = true;
        this.nowDefender = nowDefender;
    }
    public void LimitEnd()
    {
        isLimit = false;
    }


    // �߰� ���� ui
    void CreateExtraScoreText(string text, float score)
    {
        GameObject ExtraScoreText = Instantiate(ExtraScoreTextPrefab, ExtraScoreTextParent);
        ExtraScoreText.GetComponent<ExtraScoreUI>().score = text + "  + " + score;
        ExtraScoreText.SetActive(true);

        Debug.LogWarning("�߰����� +" + score);
    }

}
