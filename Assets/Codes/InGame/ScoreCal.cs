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
            Debug.Log("PlayGamesPlatform 다시 활성화");
            PlayGamesPlatform.Activate();
            Social.localUser.Authenticate(success =>
            {
                if (success)
                    Debug.Log("재로그인 성공!");
                else
                    Debug.Log("재로그인 실패");
            });
        }
        else
        {
            Debug.Log("이미 로그인되어 있음");
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
    
    // 결과창 보여주기
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

    // 스코어 저장
    public void SaveScoreAndQuit()
    {
        AddScore(unityLeaderboard, Score);
        SceneManager.LoadScene("Lobby");
        //// 서버에 점수 제출
        //PlayGamesPlatform.Instance.ReportScore(Score, GPGSIds.leaderboard_ranking, success =>
        //{
        //    if (success)
        //    {
        //        Debug.Log($"점수 {Score} 서버 전송 성공!");
        //        SceneManager.LoadScene("Lobby");
        //    }
        //    else
        //    {
        //        Debug.Log("점수 서버 전송 실패");
        //        SceneManager.LoadScene("Lobby");
        //    }
        //});
    }

    public void SaveScoreAndRetry()
    {
        // 서버에 점수 제출
        PlayGamesPlatform.Instance.ReportScore(Score, GPGSIds.leaderboard_ranking, success =>
        {
            if (success)
            {
                Debug.Log($"점수 {Score} 서버 전송 성공!");
                SceneManager.LoadScene("InGame");
            }
            else
            {
                Debug.Log("점수 서버 전송 실패");
                SceneManager.LoadScene("InGame");
            }
        });
    }

    // 유니티 점수 전달
    public async void AddScore(string leaderboardId, int score)
    {
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(leaderboardId, score);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
}
