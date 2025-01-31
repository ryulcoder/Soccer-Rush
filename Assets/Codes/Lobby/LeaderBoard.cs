using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public Text score;
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
            }
        });
    }
}
//GPGSIds ��ũ��Ʈ�� static�̾ ���� ������ �ʿ䰡����