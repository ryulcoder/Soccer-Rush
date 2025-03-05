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
        LoadMyScores();
    }

    // ž���ھ�� �θ���
    public void LoadTopScores()
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_ranking, // �������� ID
            LeaderboardStart.TopScores, // ���� �������� ����
            12, // �ҷ��� ������ ����
            LeaderboardCollection.Public, // ���� ��������
            LeaderboardTimeSpan.AllTime, // ��ü �Ⱓ
            (LeaderboardScoreData data) =>
            {
                if (data.Valid)
                {
                    Debug.Log("������ ���������� �ҷ��Խ��ϴ�.");
                    int numScores = data.Scores.Length;
                    string[] userIds = new string[numScores];

                    for (int i = 0; i < numScores; i++)
                    {
                        string playerId = data.Scores[i].userID;
                        userIds[i] = playerId;
                    }

                    PlayGamesPlatform.Instance.LoadUsers(userIds, (IUserProfile[] users) =>
                    {
                        if (users.Length > 0)
                        {
                            for (int i = 0; i < numScores; i++)
                            {
                                string playerId = data.Scores[i].userID;
                                long playerScore = data.Scores[i].value;
                                string playerName = "";

                                foreach (var user in users)
                                {
                                    if (user.id == playerId)
                                    {
                                        playerName = user.userName;
                                        break;
                                    }
                                }
                                // �����͸� ó���ϴ� �κ�!
                                playerIdText[i].text = playerName;
                                playerScoreText[i].text = playerScore.ToString();
                            }
                        }
                        LoadingPanel.SetActive(false);
                    });
                }
            });
    }

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
    public void LoadMyScores()
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_ranking,
            LeaderboardStart.PlayerCentered, // �� ������ ��� �ֵ� �ҷ�����
            1, // �� ������ �ҷ�����
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (LeaderboardScoreData myData) =>
            {
                if (myData.Valid && myData.PlayerScore != null)
                {
                    string myName = Social.localUser.userName;
                    long myScore = myData.PlayerScore.value;
                    int myRank = myData.PlayerScore.rank; // �� ��� ��������

                    myPlayerIdText.text = myName;
                    myPlayerScore.text = myScore.ToString();
                    myPlayerRankText.text = myRank.ToString(); // ��� UI�� ǥ��
                }
                else
                {
                    myPlayerIdText.text = "No data";
                    myPlayerScore.text = "0";
                    myPlayerRankText.text = "-";
                }
                LoadTopScores();
            });
    }

}
//GPGSIds ��ũ��Ʈ�� static�̾ ���� ������ �ʿ䰡����