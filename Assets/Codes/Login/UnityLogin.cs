using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SocialPlatforms;
using TMPro;


public class UnityLogin : MonoBehaviour
{
    string Token;
    public string Error;
    string rankingId = "SoccerRushRanking";
    public TextMeshProUGUI checkText;
    public LoadingLogin loadingLogin;

    async void Awake()
    {
        await TryInitializeUnityServices();
        //await InitializeUnityServices(); // Unity Services �ʱ�ȭ
        //InitializeGooglePlayGames(); // Google Play Games Ȱ��ȭ
    }

    // ���ͳ� üũ �� ��� 
    async Task TryInitializeUnityServices()
    {
        while (InternetConnect()) // ���ͳ��� ����� ������ ���
        {
            ShowInternetMessage();
            await Task.Delay(2000); // 3�� �� �ٽ� üũ
        }
        checkText.gameObject.SetActive(false);

        await InitializeUnityServices();
        InitializeGooglePlayGames(); // Google Play Games Ȱ��ȭ
    }

    async Task InitializeUnityServices()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            Debug.Log("Unity Services already initialized.");
            return;
        }

        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services Initialized");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Unity Services Initialization Failed: {e.Message}");
        }
    }

    void InitializeGooglePlayGames()
    {
        PlayGamesPlatform.Activate();
        LoginGooglePlayGames();
    }

    public void LoginGooglePlayGames()
    {
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play Games successful.");

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, async (code) =>
                {
                    Debug.Log("Authorization code: " + code);
                    Token = code;

                    // �񵿱� ������ ���� Task ���
                    await SignInWithGooglePlayGamesAsync(Token);
                    StartCoroutine(loadingLogin.WaitLoadingSecond());
                });
            }
            else
            {
                Error = "Failed to retrieve Google Play Games authorization code";
                Debug.LogError("Login Unsuccessful");
            }
        });
    }

    async Task SignInWithGooglePlayGamesAsync(string authCode)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            Debug.Log("Sign-in is successful.");

            // ���� �÷��� �г��� ��������
            //string googleNickname = GetGooglePlayNickname();
            string SetNickname = PlayerPrefs.GetString("nickname");
            Debug.Log($"Google Play Nickname: {SetNickname}");

            // Unity Authentication�� �г��� ������Ʈ
            await UpdatePlayerDisplayName(SetNickname);
            // ó�� ������ ���ھ� �ִ��� Ȯ��
            if (PlayerPrefs.GetInt("first") == 0)
            {
                GetPlayerScore(rankingId);
            }

        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"Authentication failed: {ex.Message}");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Request failed: {ex.Message}");
        }
    }

    async Task UpdatePlayerDisplayName(string nickname)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(nickname);
            Debug.Log($"Updated Player Display Name: {nickname}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to update player display name: {ex.Message}");
        }
    }

    string GetGooglePlayNickname()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            return PlayGamesPlatform.Instance.GetUserDisplayName();
        }
        return "UnknownPlayer"; // �α��� ���� �� �⺻ �г���
    }


    public async void GetPlayerScore(string leaderboardId)
    {
        var scoreResponse = await LeaderboardsService.Instance
            .GetPlayerScoreAsync(leaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        int myScore = (int)scoreResponse.Score;
        PlayerPrefs.SetInt("BestScore", myScore);
        PlayerPrefs.SetInt("first", 1);
        PlayerPrefs.Save();
    }

    // ���ͳ� ���� Ȯ���ϱ�
    void ShowInternetMessage()
    {
        checkText.gameObject.SetActive(true);
        checkText.color = new Color(checkText.color.r, checkText.color.g, checkText.color.b, 1); // ���İ� �ʱ�ȭ
    }

    bool InternetConnect()
    {
        return Application.internetReachability == NetworkReachability.NotReachable;
    }



    //public async void LoadPlayerScore(string leaderboardId)
    //{
    //    try
    //    {
    //        // ���� �α����� �÷��̾� ID ��������
    //        string playerId = AuthenticationService.Instance.PlayerId;
    //        Debug.Log($"Current Player ID: {playerId}");

    //        // �������忡�� ���� ��������
    //        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId);

    //        if (scoresResponse != null && scoresResponse.Results.Count > 0)
    //        {
    //            foreach (var score in scoresResponse.Results)
    //            {
    //                if (score.PlayerId == playerId) // ���� �÷��̾��� ������ ���͸�
    //                {
    //                    Debug.Log($"Player Score Found! Score: {score.Score}");
    //                    return;
    //                }
    //            }
    //            Debug.Log("No matching player score found.");
    //        }
    //        else
    //        {
    //            Debug.Log("No scores found in the leaderboard.");
    //        }
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.LogError($"Failed to load player score: {ex.Message}");
    //    }
    //}

    //public async void GetScoresByPlayerIds()
    //{
    //    var otherPlayerIds = new List<string> { "abc123", "abc456" };
    //    var scoresResponse = await LeaderboardsService.Instance
    //        .GetScoresByPlayerIdsAsync("SoccerRushRanking", otherPlayerIds);
    //    Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    //}
}
