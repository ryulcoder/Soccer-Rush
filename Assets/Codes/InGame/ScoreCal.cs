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

public class ScoreCal : MonoBehaviour
{
    public Transform player;
    float distance;
    float score;
    public Text distanceText;
    public Text scoreText;
    public Text distanceResult;
    public Text scoreResult;
    public GameObject BestScoreStamp;
    public GameObject lastBestScore;
    public GameObject settingButton;

    string unityLeaderboard = "SoccerRushRanking";


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
    }

    void Update()
    {
        score = player.position.z / 10 + ExtraScore.instance.CheckEndScore();
        distance = player.position.z / 20;
        SetScore();
    }

    void SetScore()
    {
        distanceText.text = Distance.ToString() + "m";
        scoreText.text = Score.ToString();
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
        AddScore(unityLeaderboard, Score);
        SceneManager.LoadScene("Lobby");
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
        // ������ ���� ����
        PlayGamesPlatform.Instance.ReportScore(Score, GPGSIds.leaderboard_ranking, success =>
        {
            if (success)
            {
                Debug.Log($"���� {Score} ���� ���� ����!");
                SceneManager.LoadScene("InGame");
            }
            else
            {
                Debug.Log("���� ���� ���� ����");
                SceneManager.LoadScene("InGame");
            }
        });
    }

    // ����Ƽ ���� ����
    public async void AddScore(string leaderboardId, int score)
    {
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(leaderboardId, score);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
}
