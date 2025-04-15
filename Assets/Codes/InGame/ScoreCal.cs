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
    public Slider impactSlider;         // 임팩트 존 슬라이더
    public TextMeshProUGUI checkText;

    public TextMeshProUGUI bTText;     // 수비수 제친 숫자
    string unityLeaderboard = "SoccerRushRanking";

    float scoreMulti;

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

    // 유니티 점수 전달
    public async void AddScore(string leaderboardId, int score)
    {
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(leaderboardId, score);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }

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

    // 업적 저장로그
    void SendAch()
    {
        int pastTD = PlayerPrefs.GetInt("TotalDistance");
        int pastBT = PlayerPrefs.GetInt("BreakThrough");
        PlayerPrefs.SetInt("TotalDistance", Distance + pastTD);
        PlayerPrefs.SetInt("BreakThrough", bTAmount + pastBT); 

        PlayerPrefs.Save();
    }
}
