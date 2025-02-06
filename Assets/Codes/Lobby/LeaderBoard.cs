using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public Text score;
    public Text[] playerIdText;
    public Text[] playerScoreText;
    public void ShowLeaderboardUI_Ranking()
    => ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(GPGSIds.leaderboard_ranking);
    //�� �������� ����� ��ŷ�̶�� �̸��� �������带 �ٷ� �����ش�

    public void ShowLeaderboardUI() => Social.ShowLeaderboardUI();
    //�� �������� ����� �����ְ� �� �� ������ �� �ִ�.

    public void AddLeaderboard()//������ ����ϴ� �Լ�
    => Social.ReportScore(int.Parse(score.text), GPGSIds.leaderboard_ranking, (bool success) => { });

    // �������� �θ������� ����
    void OnEnable()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log(Social.localUser.id);
                Debug.Log("�����");

                LoadTopScores();
            }
            else
            {
                Debug.Log("����ȵ�");
            }
        });
    }

    public void LoadTopScores()
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_ranking, // �������� ID
            LeaderboardStart.TopScores, // ���� �������� ����
            10, // �ҷ��� ������ ����
            LeaderboardCollection.Public, // ���� ��������
            LeaderboardTimeSpan.AllTime, // ��ü �Ⱓ
            (LeaderboardScoreData data) => {
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

                    PlayGamesPlatform.Instance.LoadUsers(userIds, (IUserProfile[] users) => {
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
                                playerIdText[i].text = playerId;
                                playerScoreText[i].text = playerScore.ToString();
                            }
                        }
                    });
                }
            });
    }
}
//GPGSIds ��ũ��Ʈ�� static�̾ ���� ������ �ʿ䰡����