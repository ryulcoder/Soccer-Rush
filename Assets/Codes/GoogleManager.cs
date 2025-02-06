using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoogleManager : MonoBehaviour
{
    public Text logText;
    
    public void SignButton()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        SignIn();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            // Perfectly login success

            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

            logText.text = "Success \n" + name;
            PlayerPrefs.SetInt("Login", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Lobby");

            //클라우드 세이브 오류나서 일단 꺼둠
            //DataConnectGP dataConnectGP = GetComponent<DataConnectGP>();
            //dataConnectGP.LoadData();
        }
        else
        {
            logText.text = "Sign in Failed!";
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
            // Login failed
        }
    }
}