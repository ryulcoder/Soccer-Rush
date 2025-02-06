using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

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
    
    public int Distance { get { return (int)distance; } }
    public int Score { get { return (int)score; } }

    void Update()
    {
        score = player.position.z / 10;
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

        distanceText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        settingButton.SetActive(false);
    }

    // 스코어 저장
    public void SaveScore()
    {
        int highScore = PlayerPrefs.GetInt("BestScore", 0);

        if (Score > highScore)
        {
            PlayerPrefs.SetInt("BestScore", Score);
            PlayerPrefs.Save();
            Debug.Log($"새로운 하이스코어 저장: {Score}");


            // 서버에 점수 제출
            Social.ReportScore(Score, GPGSIds.leaderboard_ranking, success =>
            {
                if (success)
                {
                    Debug.Log($"점수 {Score} 서버 전송 성공!");
                    SetResult();
                }
                else
                {
                    Debug.Log("점수 서버 전송 실패");
                }
            });
        }
    }
}
