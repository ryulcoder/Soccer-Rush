using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using TMPro;
using DG.Tweening;


public class ScoreCal : MonoBehaviour
{
    public Transform player;
    float distance;
    float score;
    public int bTAmount;
    public Text distanceText;
    public Text scoreText;
    public Text distanceResult;
    public Text scoreResult;
    public GameObject BestScoreStamp;
    public GameObject lastBestScore;
    public GameObject settingButton;
    public Slider impactSlider;         // ����Ʈ �� �����̴�
    public TextMeshProUGUI checkText;

    public TextMeshProUGUI bTText;     // ����� ��ģ ����
    string unityLeaderboard = "SoccerRushRanking";

    float scoreMulti;

    public int Distance { get { return (int)distance; } }
    public int Score { get { return (int)score; } }

    void Start()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            Debug.Log("PlayGamesPlatform �ٽ� Ȱ��ȭ");
            PlayGamesPlatform.Activate();
            Social.localUser.Authenticate(success =>
            {
                if (success)
                    Debug.Log("��α��� ����!");
                else
                    Debug.Log("��α��� ����");
            });
        }
        else
        {
            Debug.Log("�̹� �α��εǾ� ����");
        }

        scoreMulti = GameManager.Instance.scoreMulti;
    }

    void Update()
    {
        score = player.position.z / 10 * scoreMulti + ExtraScore.instance.CheckEndScore();
        distance = player.position.z / 20;
        SetScore();
    }

    void SetScore()
    {
        distanceText.text = Distance.ToString() + "m";
        scoreText.text = Score.ToString();
        impactSlider.value = (distance % 200)/ 200;
        bTText.text = bTAmount.ToString();
    }
    
    // ���â �����ֱ�
    public void SetResult()
    {
        distanceResult.text = "Distance: " + Distance.ToString() + "m";
        scoreResult.text = Score.ToString();

        int bestScore = PlayerPrefs.GetInt("BestScore");
        if (score > bestScore) 
        {
            PlayerPrefs.SetInt("BestScore", (int)score);
            PlayerPrefs.Save();
            BestScoreStamp.SetActive(true);
        }
        else
        {
            lastBestScore.SetActive(true);
            lastBestScore.GetComponent<Text>().text = "Best Score : <color=#FCC062>"+ bestScore + "</color>";
        }

        //distanceText.gameObject.SetActive(false);
        //scoreText.gameObject.SetActive(false);
        //settingButton.SetActive(false);
    }

    // ���ھ� ����
    public void SaveScoreAndQuit()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowInternetMessage();
        }
        else
        {
            SendAch();
            AddScore(unityLeaderboard, Score);
            SceneManager.LoadScene("Lobby");
        }
        //// ������ ���� ����
        //PlayGamesPlatform.Instance.ReportScore(Score, GPGSIds.leaderboard_ranking, success =>
        //{
        //    if (success)
        //    {
        //        Debug.Log($"���� {Score} ���� ���� ����!");
        //        SceneManager.LoadScene("Lobby");
        //    }
        //    else
        //    {
        //        Debug.Log("���� ���� ���� ����");
        //        SceneManager.LoadScene("Lobby");
        //    }
        //});
    }

    public void SaveScoreAndRetry()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowInternetMessage();
        }
        else
        {
            SendAch();
            AddScore(unityLeaderboard, Score);
            SceneManager.LoadScene("InGame");
        }
    }

    // ����Ƽ ���� ����
    public async void AddScore(string leaderboardId, int score)
    {
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(leaderboardId, score);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }

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

    // ���� ����α�
    void SendAch()
    {
        int pastTD = PlayerPrefs.GetInt("TotalDistance");
        int pastBT = PlayerPrefs.GetInt("BreakThrough");
        PlayerPrefs.SetInt("TotalDistance", Distance + pastTD);
        PlayerPrefs.SetInt("BreakThrough", bTAmount + pastBT); 

        PlayerPrefs.Save();
    }
}
