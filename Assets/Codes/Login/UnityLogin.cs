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


public class UnityLogin : MonoBehaviour
{
    string Token;
    public string Error;

    async void Awake()
    {
        await InitializeUnityServices(); // Unity Services 초기화
        InitializeGooglePlayGames(); // Google Play Games 활성화
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

                    // 비동기 실행을 위해 Task 사용
                    await SignInWithGooglePlayGamesAsync(Token);
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

            // 구글 플레이 닉네임 가져오기
            string googleNickname = GetGooglePlayNickname();
            Debug.Log($"Google Play Nickname: {googleNickname}");

            // Unity Authentication에 닉네임 업데이트
            await UpdatePlayerDisplayName(googleNickname);
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
        return "UnknownPlayer"; // 로그인 실패 시 기본 닉네임
    }

    //public async void LoadPlayerScore(string leaderboardId)
    //{
    //    try
    //    {
    //        // 현재 로그인한 플레이어 ID 가져오기
    //        string playerId = AuthenticationService.Instance.PlayerId;
    //        Debug.Log($"Current Player ID: {playerId}");

    //        // 리더보드에서 점수 가져오기
    //        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId);

    //        if (scoresResponse != null && scoresResponse.Results.Count > 0)
    //        {
    //            foreach (var score in scoresResponse.Results)
    //            {
    //                if (score.PlayerId == playerId) // 현재 플레이어의 점수만 필터링
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
