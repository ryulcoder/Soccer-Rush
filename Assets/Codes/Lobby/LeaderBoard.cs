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
    public TextMeshProUGUI messageText; // �α��� �ʿ� �޽���
    public GameObject LoadingPanel;

    string rankingId = "SoccerRushRanking";

    [Header("[MyScore]")]
    string myNickname;
    int myScore;
    int myRank;

    public void ShowLeaderboardUI_Ranking()
    => ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(GPGSIds.leaderboard_ranking);
    //�� �������� ����� ��ŷ�̶�� �̸��� �������带 �ٷ� �����ش�

    public void ShowLeaderboardUI() => Social.ShowLeaderboardUI();
    //�� �������� ����� �����ְ� �� �� ������ �� �ִ�.

    public void AddLeaderboard()//������ ����ϴ� �Լ�
    => Social.ReportScore(int.Parse(score.text), GPGSIds.leaderboard_ranking, (bool success) => { });

    private bool isAuthenticated = false; // ���� ���� ����


    //void AuthenticateUser()
    //{
    //    Social.localUser.Authenticate((bool success) =>
    //    {
    //        if (success)
    //        {
    //            Debug.Log(Social.localUser.id);
    //            Debug.Log("�����");
    //            isAuthenticated = true; // ���� �Ϸ� �÷��� ����
    //            LoadMyScores();
    //        }
    //        else
    //        {
    //            Debug.Log("����ȵ�");
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

    //// ž���ھ�� �θ���
    //public void LoadTopScores()
    //{
    //    PlayGamesPlatform.Instance.LoadScores(
    //        GPGSIds.leaderboard_ranking, // �������� ID
    //        LeaderboardStart.TopScores, // ���� �������� ����
    //        12, // �ҷ��� ������ ����
    //        LeaderboardCollection.Public, // ���� ��������
    //        LeaderboardTimeSpan.AllTime, // ��ü �Ⱓ
    //        (LeaderboardScoreData data) =>
    //        {
    //            if (data.Valid)
    //            {
    //                Debug.Log("������ ���������� �ҷ��Խ��ϴ�.");
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
    //                            // �����͸� ó���ϴ� �κ�!
    //                            playerIdText[i].text = playerName;
    //                            playerScoreText[i].text = playerScore.ToString();
    //                        }
    //                    }
    //                    LoadingPanel.SetActive(false);
    //                });
    //            }
    //        });
    //}

    // �α����� �������� �α����� �϶�� �޽���
    public void ShowLoginMessage()
    {
        float duration = 1.5f; // �ִϸ��̼� ���� �ð�
        Vector3 moveOffset = new Vector3(0, 100, 0); // �̵� �Ÿ�

        messageText.gameObject.SetActive(true);
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1); // ���İ� �ʱ�ȭ

        // ���� ��ġ���� moveOffset ��ŭ ���� �̵�
        messageText.rectTransform.anchoredPosition += new Vector2(0, -moveOffset.y);

        // DOTween �ִϸ��̼� ����
        messageText.rectTransform.DOAnchorPosY(messageText.rectTransform.anchoredPosition.y + moveOffset.y, duration);
        messageText.DOFade(0, duration).OnComplete(() =>
        {
            messageText.gameObject.SetActive(false); // �ִϸ��̼� �Ϸ� �� ��Ȱ��ȭ
        });
    }

    // ������ �θ���
    //public void LoadMyScores()
    //{
    //    PlayGamesPlatform.Instance.LoadScores(
    //        GPGSIds.leaderboard_ranking,
    //        LeaderboardStart.PlayerCentered, // �� ������ ��� �ֵ� �ҷ�����
    //        1, // �� ������ �ҷ�����
    //        LeaderboardCollection.Public,
    //        LeaderboardTimeSpan.AllTime,
    //        (LeaderboardScoreData myData) =>
    //        {
    //            if (myData.Valid && myData.PlayerScore != null)
    //            {
    //                string myName = Social.localUser.userName;
    //                long myScore = myData.PlayerScore.value;
    //                int myRank = myData.PlayerScore.rank; // �� ��� ��������

    //                Debug.Log(myRank);
    //                Debug.Log(myScore);
    //                Debug.Log(myName);
    //                if(myRank == -1)
    //                {
    //                    ShowLeaderboardUI_Ranking();
    //                }
    //                myPlayerIdText.text = myName;
    //                myPlayerScore.text = myScore.ToString();
    //                myPlayerRankText.text = myRank.ToString(); // ��� UI�� ǥ��
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

    // ����Ƽ ���� ��ũ ��������
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
        myPlayerRankText.text = (myRank + 1).ToString(); // ��� UI�� ǥ��
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
//GPGSIds ��ũ��Ʈ�� static�̾ ���� ������ �ʿ䰡����