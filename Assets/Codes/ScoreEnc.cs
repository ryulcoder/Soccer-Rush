using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;

public class ScoreEnc : MonoBehaviour
{
    private const string SaveFileName = "player_score";
    private static string secretKey;

    // 네이티브 라이브러리에서 secretKey 가져오기
    [DllImport("secretKeyLib")]
    private static extern IntPtr Java_com_mycompany_mygame_SecretKeyProvider_getSecretKey();

    static ScoreEnc()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // 네이티브 코드에서 Secret Key 가져오기
            secretKey = Marshal.PtrToStringAnsi(Java_com_mycompany_mygame_SecretKeyProvider_getSecretKey());
        }
        else
        {
            secretKey = "FallbackSecretKey123"; // 에디터 및 iOS용
        }
    }

    private static string ComputeHMAC(string value)
    {
        using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hash);
        }
    }

    public static void SaveScoreToCloud(int score)
    {
        string scoreStr = score.ToString();
        string hash = ComputeHMAC(scoreStr);

        string data = scoreStr + ":" + hash; // 점수와 해시를 함께 저장

        PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
            SaveFileName,
            GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
                    PlayGamesPlatform.Instance.SavedGame.CommitUpdate(game, update, bytes, (commitStatus, updatedGame) =>
                    {
                        if (commitStatus == SavedGameRequestStatus.Success)
                        {
                            Debug.Log(" 클라우드에 점수 저장 성공!");
                        }
                        else
                        {
                            Debug.LogError(" 클라우드 저장 실패! 오류 상태: " + commitStatus);
                        }
                    });
                }
                else
                {
                    Debug.LogError(" 클라우드 저장 열기 실패!");
                }
            });
    }

    public static void LoadScoreFromCloud(Action<int> onScoreLoaded)
    {
        PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
            SaveFileName,
            GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, (readStatus, data) =>
                    {
                        if (readStatus == SavedGameRequestStatus.Success && data.Length > 0)
                        {
                            string savedData = Encoding.UTF8.GetString(data);
                            string[] parts = savedData.Split(':');

                            if (parts.Length == 2)
                            {
                                string savedScore = parts[0];
                                string savedHash = parts[1];

                                string computedHash = ComputeHMAC(savedScore);
                                if (savedHash == computedHash)
                                {
                                    int score = int.Parse(savedScore);
                                    Debug.Log($"클라우드에서 불러온 점수: {score}");
                                    onScoreLoaded?.Invoke(score);
                                }
                                else
                                {
                                    Debug.LogWarning("해킹 감지! 클라우드 저장 무효화.");
                                    onScoreLoaded?.Invoke(0);
                                }
                            }
                            else
                            {
                                Debug.LogError(" 클라우드 데이터 손상!");
                                onScoreLoaded?.Invoke(0);
                            }
                        }
                        else
                        {
                            Debug.LogError("클라우드 데이터 읽기 실패!");
                            onScoreLoaded?.Invoke(0);
                        }
                    });
                }
                else
                {
                    Debug.LogError(" 클라우드 저장 열기 실패!");
                    onScoreLoaded?.Invoke(0);
                }
            });
    }

    public static string GetSecretKey()
    {
        return secretKey;
    }

}
