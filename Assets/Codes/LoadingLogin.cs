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
    public GameObject slider;
    public GameObject unityLogin;

    Slider loadingSlider;
    float duration = 3f; // 3�� ���� ä���
    float elapsed = 0f;  // ��� �ð�

    int loginGoogle = 1;
    int guest = 2;

    void Awake()
    {
        loadingSlider = slider.GetComponent<Slider>();

    }
    void Start()
    {
        StartCoroutine(WaitLoadingfirst());
    }


    //�ڵ��α��ζ����� �α��ι�ư �Լ� ����
    //// ���� �α��� ��ư
    //public void SignGoogle()
    //{
    //    PlayGamesPlatform.DebugLogEnabled = true;
    //    PlayGamesPlatform.Activate();
    //    loginButton.SetActive(false);
    //    guestLoginButton.SetActive(false);
    //    SliderOn(0);
    //}

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
            StartCoroutine(WaitLoadingSecond());

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
            PlayerPrefs.SetInt("first", 1);
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

    // ù ���� ���̽��ھ� ã�ƺ���
    void LoadHighScore()
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_ranking,
            LeaderboardStart.PlayerCentered, // �� ������ �������� �ҷ�����
            1,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (LeaderboardScoreData data) =>
            {
                if (data.Valid)
                {
                    long highScore = data.PlayerScore.value; // ���� �� ������ �ùٸ��� ����
                    if (highScore > PlayerPrefs.GetInt("BestScore"))
                    {
                        PlayerPrefs.SetInt("BestScore", (int)highScore);
                        PlayerPrefs.Save();
                    }
                    Debug.Log($"�� ���̽��ھ� �ҷ����� ����: {highScore}");
                    StartCoroutine(WaitLoadingSecond());
                }
                else
                {
                    Debug.Log("�� ���̽��ھ� �ҷ����� ����");
                }
            });
    }

    // �ε� �����̴�
    IEnumerator WaitLoadingfirst()
    {
        yield return new WaitForSeconds(1f);
        while (loadingSlider.value < 0.8f)
        {
            elapsed += Time.deltaTime;
            loadingSlider.value = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        if(PlayerPrefs.GetInt("first")== 0 )
        {
            SignIn(1);

        }
        else
        {
            SignIn(0);

        }
    }

    // �ε� �ι�°
    IEnumerator WaitLoadingSecond()
    {
        unityLogin.SetActive(true);
        while (loadingSlider.value < 1)
        {
            elapsed += Time.deltaTime;
            loadingSlider.value = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        SceneManager.LoadScene("Lobby");
    }


    //// �����̴� Ű��
    //void SliderOn(int first)
    //{
    //    slider.SetActive(true);
    //    loadingSlider = slider.GetComponent<Slider>();
    //    if(first == guest)  // �Խ�Ʈ�϶�
    //    {
    //        StartCoroutine(WaitLoadingSecond());

    //    }
    //    else
    //    {
    //        StartCoroutine(WaitLoadingfirst(first));
    //    }
    //}

    //public void GuestLogin()
    //{
    //    loginButton.SetActive(false);
    //    guestLoginButton.SetActive(false);
    //    PlayerPrefs.SetInt("FirstIn", 1);
    //    PlayerPrefs.Save();
    //    SliderOn(guest);
    //}

    //IEnumerator WaitForLogin()
    //{
    //    Debug.Log("�α��� Ȯ�� ��...");

    //    while (!PlayGamesPlatform.Instance.localUser.authenticated)
    //    {
    //        yield return new WaitForSeconds(0.5f);  // 0.5�ʸ��� Ȯ��
    //    }

    //    Debug.Log("�α��� ����!");
    //}
}
