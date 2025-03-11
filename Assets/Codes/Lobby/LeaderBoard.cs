using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using System.Xml.Linq;
using UnityEngine.SocialPlatforms.Impl;
using System;
using Unity.Services.Leaderboards.Models;


public class LeaderBoard : MonoBehaviour
{
    public Text score;
    public Text[] playerIdText;
    public Text[] playerScoreText;
    public Text myPlayerIdText;
    public Text myPlayerScore;
    public Text myPlayerRankText;
    public GameObject setting;
    public TextMeshProUGUI messageText; // 로그인 필요 메시지
    public GameObject LoadingPanel;

    string rankingId = "SoccerRushRanking";

    [Header("[MyScore]")]
    string myNickname;
    int myScore;
    int myRank;

    public void ShowLeaderboardUI_Ranking()
    => ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(GPGSIds.leaderboard_ranking);
    //내 리더보드 목록중 랭킹이라는 이름의 리더보드를 바로 보여준다

    public void ShowLeaderboardUI() => Social.ShowLeaderboardUI();
    //내 리더보드 목록을 보여주고 그 중 선택할 수 있다.

    public void AddLeaderboard()//점수를 기록하는 함수
    => Social.ReportScore(int.Parse(score.text), GPGSIds.leaderboard_ranking, (bool success) => { });

    private bool isAuthenticated = false; // 인증 여부 저장


    //void AuthenticateUser()
    //{
    //    Social.localUser.Authenticate((bool success) =>
    //    {
    //        if (success)
    //        {
    //            Debug.Log(Social.localUser.id);
    //            Debug.Log("연결됨");
    //            isAuthenticated = true; // 인증 완료 플래그 설정
    //            LoadMyScores();
    //        }
    //        else
    //        {
    //            Debug.Log("연결안됨");
    //            setting.SetActive(true);
    //            ShowLoginMessage();
    //            gameObject.SetActive(false);
    //        }
    //    });
    //}


    void OnEnable()
    {
        GetPlayerScore(rankingId);
        GetTopPlayers(rankingId);
    }

    private void OnDisable()
    {
        LoadingPanel.SetActive(true);
    }

    //// 탑스코어로 부르기
    //public void LoadTopScores()
    //{
    //    PlayGamesPlatform.Instance.LoadScores(
    //        GPGSIds.leaderboard_ranking, // 리더보드 ID
    //        LeaderboardStart.TopScores, // 상위 점수부터 시작
    //        12, // 불러올 점수의 개수
    //        LeaderboardCollection.Public, // 공개 리더보드
    //        LeaderboardTimeSpan.AllTime, // 전체 기간
    //        (LeaderboardScoreData data) =>
    //        {
    //            if (data.Valid)
    //            {
    //                Debug.Log("점수를 성공적으로 불러왔습니다.");
    //                int numScores = data.Scores.Length;
    //                string[] userIds = new string[numScores];

    //                for (int i = 0; i < numScores; i++)
    //                {
    //                    string playerId = data.Scores[i].userID;
    //                    userIds[i] = playerId;
    //                }

    //                PlayGamesPlatform.Instance.LoadUsers(userIds, (IUserProfile[] users) =>
    //                {
    //                    if (users.Length > 0)
    //                    {
    //                        for (int i = 0; i < numScores; i++)
    //                        {
    //                            string playerId = data.Scores[i].userID;
    //                            long playerScore = data.Scores[i].value;
    //                            string playerName = "";

    //                            foreach (var user in users)
    //                            {
    //                                if (user.id == playerId)
    //                                {
    //                                    playerName = user.userName;
    //                                    Debug.Log(playerName);
    //                                    break;
    //                                }
    //                            }
    //                            // 데이터를 처리하는 부분!
    //                            playerIdText[i].text = playerName;
    //                            playerScoreText[i].text = playerScore.ToString();
    //                        }
    //                    }
    //                    LoadingPanel.SetActive(false);
    //                });
    //            }
    //        });
    //}

    // 로그인을 안했을시 로그인을 하라는 메시지
    public void ShowLoginMessage()
    {
        float duration = 1.5f; // 애니메이션 지속 시간
        Vector3 moveOffset = new Vector3(0, 100, 0); // 이동 거리

        messageText.gameObject.SetActive(true);
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1); // 알파값 초기화

        // 현재 위치에서 moveOffset 만큼 위로 이동
        messageText.rectTransform.anchoredPosition += new Vector2(0, -moveOffset.y);

        // DOTween 애니메이션 실행
        messageText.rectTransform.DOAnchorPosY(messageText.rectTransform.anchoredPosition.y + moveOffset.y, duration);
        messageText.DOFade(0, duration).OnComplete(() =>
        {
            messageText.gameObject.SetActive(false); // 애니메이션 완료 후 비활성화
        });
    }

    // 내점수 부르기
    //public void LoadMyScores()
    //{
    //    PlayGamesPlatform.Instance.LoadScores(
    //        GPGSIds.leaderboard_ranking,
    //        LeaderboardStart.PlayerCentered, // 내 점수가 어디 있든 불러오기
    //        1, // 내 점수만 불러오기
    //        LeaderboardCollection.Public,
    //        LeaderboardTimeSpan.AllTime,
    //        (LeaderboardScoreData myData) =>
    //        {
    //            if (myData.Valid && myData.PlayerScore != null)
    //            {
    //                string myName = Social.localUser.userName;
    //                long myScore = myData.PlayerScore.value;
    //                int myRank = myData.PlayerScore.rank; // 내 등수 가져오기

    //                Debug.Log(myRank);
    //                Debug.Log(myScore);
    //                Debug.Log(myName);
    //                if(myRank == -1)
    //                {
    //                    ShowLeaderboardUI_Ranking();
    //                }
    //                myPlayerIdText.text = myName;
    //                myPlayerScore.text = myScore.ToString();
    //                myPlayerRankText.text = myRank.ToString(); // 등수 UI에 표시
    //            }
    //            else
    //            {
    //                myPlayerIdText.text = "No data";
    //                myPlayerScore.text = "0";
    //                myPlayerRankText.text = "-";
    //            }
    //            LoadTopScores();
    //        });
    //}

    // 유니티 나의 랭크 가져오기
    public async void GetPlayerScore(string leaderboardId)
    {
        var scoreResponse = await LeaderboardsService.Instance
            .GetPlayerScoreAsync(leaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        myScore = (int)scoreResponse.Score;
        myNickname = scoreResponse.PlayerName;
        myRank = scoreResponse.Rank;
        Debug.Log(myScore.ToString());
        Debug.Log(myNickname);
        myNickname = myNickname.Split('#')[0];

        myPlayerIdText.text = myNickname;
        myPlayerScore.text = myScore.ToString();
        myPlayerRankText.text = (myRank + 1).ToString(); // 등수 UI에 표시
    }

    public async void GetTopPlayers(string leaderboardId)
    {
        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, new GetScoresOptions { Limit = 12 });

            if (scoresResponse == null || scoresResponse.Results == null)
            {
                Debug.LogError("Failed to fetch leaderboard scores.");
                return;
            }

            int index = 0;

            foreach (var playerScore in scoresResponse.Results)
            {
                string playername = playerScore.PlayerName.Split('#')[0];
                playerIdText[index].text = playername;
                playerScoreText[index].text = playerScore.Score.ToString();
                index++;
            }
            LoadingPanel.SetActive(false);

            foreach (var playerScore in scoresResponse.Results)
            {
                Debug.Log($"Rank {playerScore.Rank}: {playerScore.PlayerName} - {playerScore.Score}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching leaderboard scores: {e.Message}");
        }
    }
}
//GPGSIds 스크립트는 static이어서 따로 참조할 필요가없다