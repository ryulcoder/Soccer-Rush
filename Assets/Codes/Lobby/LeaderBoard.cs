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
    //내 리더보드 목록중 랭킹이라는 이름의 리더보드를 바로 보여준다

    public void ShowLeaderboardUI() => Social.ShowLeaderboardUI();
    //내 리더보드 목록을 보여주고 그 중 선택할 수 있다.

    public void AddLeaderboard()//점수를 기록하는 함수
    => Social.ReportScore(int.Parse(score.text), GPGSIds.leaderboard_ranking, (bool success) => { });

    // 리더보드 부르기전에 접속
    void OnEnable()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log(Social.localUser.id);
                Debug.Log("연결됨");

                LoadTopScores();
            }
            else
            {
                Debug.Log("연결안됨");
            }
        });
    }

    public void LoadTopScores()
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_ranking, // 리더보드 ID
            LeaderboardStart.TopScores, // 상위 점수부터 시작
            10, // 불러올 점수의 개수
            LeaderboardCollection.Public, // 공개 리더보드
            LeaderboardTimeSpan.AllTime, // 전체 기간
            (LeaderboardScoreData data) => {
                if (data.Valid)
                {
                    Debug.Log("점수를 성공적으로 불러왔습니다.");
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
                                // 데이터를 처리하는 부분!
                                playerIdText[i].text = playerId;
                                playerScoreText[i].text = playerScore.ToString();
                            }
                        }
                    });
                }
            });
    }
}
//GPGSIds 스크립트는 static이어서 따로 참조할 필요가없다