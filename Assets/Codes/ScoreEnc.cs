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

    // ����Ƽ�� ���̺귯������ secretKey ��������
    [DllImport("secretKeyLib")]
    private static extern IntPtr Java_com_mycompany_mygame_SecretKeyProvider_getSecretKey();

    static ScoreEnc()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // ����Ƽ�� �ڵ忡�� Secret Key ��������
            secretKey = Marshal.PtrToStringAnsi(Java_com_mycompany_mygame_SecretKeyProvider_getSecretKey());
        }
        else
        {
            secretKey = "FallbackSecretKey123"; // ������ �� iOS��
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

        string data = scoreStr + ":" + hash; // ������ �ؽø� �Բ� ����

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
                            Debug.Log(" Ŭ���忡 ���� ���� ����!");
                        }
                        else
                        {
                            Debug.LogError(" Ŭ���� ���� ����! ���� ����: " + commitStatus);
                        }
                    });
                }
                else
                {
                    Debug.LogError(" Ŭ���� ���� ���� ����!");
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
                                    Debug.Log($"Ŭ���忡�� �ҷ��� ����: {score}");
                                    onScoreLoaded?.Invoke(score);
                                }
                                else
                                {
                                    Debug.LogWarning("��ŷ ����! Ŭ���� ���� ��ȿȭ.");
                                    onScoreLoaded?.Invoke(0);
                                }
                            }
                            else
                            {
                                Debug.LogError(" Ŭ���� ������ �ջ�!");
                                onScoreLoaded?.Invoke(0);
                            }
                        }
                        else
                        {
                            Debug.LogError("Ŭ���� ������ �б� ����!");
                            onScoreLoaded?.Invoke(0);
                        }
                    });
                }
                else
                {
                    Debug.LogError(" Ŭ���� ���� ���� ����!");
                    onScoreLoaded?.Invoke(0);
                }
            });
    }

    public static string GetSecretKey()
    {
        return secretKey;
    }

}
