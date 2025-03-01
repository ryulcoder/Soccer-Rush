using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using UnityEngine.SceneManagement;


public class LoadingLogin : MonoBehaviour
{
    public GameObject loginButton;
    public GameObject guestLoginButton;
    public GameObject LoadingText;

    void Start()
    {
        if (PlayerPrefs.GetInt("Login") == 1)
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            SignIn(0);
            loginButton.SetActive(false);
            guestLoginButton.SetActive(false);
            LoadingText.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("FirstIn") == 0)
        {

        }
        else
        {
            loginButton.SetActive(false);
            guestLoginButton.SetActive(false);
            LoadingText.SetActive(true);
            StartCoroutine(WaitLobbyImage());
        }
    }

    // ���� �α��� ��ư
    public void SignGoogle()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        SignIn(1);
        loginButton.SetActive(false);
        guestLoginButton.SetActive(false);
        LoadingText.SetActive(true);
    }

    public void SignIn(int first)
    {
        if (first == 0)
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
        else
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthenticationFirstTime);
        }
    }



    // ��� �α��ν� ����
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            // Perfectly login success

            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

            //logText.text = "Success \n" + name;
            StartCoroutine(WaitLobbyImage());

            //Ŭ���� ���̺� �������� �ϴ� ����
            //DataConnectGP dataConnectGP = GetComponent<DataConnectGP>();
            //dataConnectGP.LoadData();
        }
        else
        {
            //logText.text = "Sign in Failed!";
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
            // Login failed
        }
    }

    // ù �α��ν� ����
    internal void ProcessAuthenticationFirstTime(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            // Perfectly login success

            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

            //logText.text = "Success \n" + name;
            PlayerPrefs.SetInt("Login", 1);
            PlayerPrefs.SetInt("FirstIn", 1);
            PlayerPrefs.Save();
            LoadHighScore();
            //Ŭ���� ���̺� �������� �ϴ� ����
            //DataConnectGP dataConnectGP = GetComponent<DataConnectGP>();
            //dataConnectGP.LoadData();
        }
        else
        {
            //logText.text = "Sign in Failed!";
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthenticationFirstTime);
            // Login failed
        }
    }

    // �α��ν� �ְ� ��� �ҷ�����
    void LoadHighScore()
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_ranking,
            LeaderboardStart.TopScores,
            1,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (LeaderboardScoreData data) =>
            {
                if (data.Valid)
                {
                    long highScore = data.PlayerScore.value;
                    if (highScore > PlayerPrefs.GetInt("BestScore"))
                    {
                        PlayerPrefs.SetInt("BestScore", (int)highScore);
                        PlayerPrefs.Save();
                    }
                    Debug.Log($"���̽��ھ� �ҷ����� ����: {highScore}");
                    StartCoroutine(WaitLobbyImage());
                }
                else
                {
                    Debug.Log("���̽��ھ� �ҷ����� ����");
                }
            });

    }

    // �κ� �̹����� �����ֱ� ���� ��� �ð�
    IEnumerator WaitLobbyImage()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Lobby");
    }
}
