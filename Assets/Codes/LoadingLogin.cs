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
    float duration = 3f; // 3초 동안 채우기
    float elapsed = 0f;  // 경과 시간

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


    //자동로그인때문에 로그인버튼 함수 보류
    //// 구글 로그인 버튼
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



    // 통상 로그인시 과정
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

            //클라우드 세이브 오류나서 일단 꺼둠
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

    // 첫 로그인시 과정
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
            //클라우드 세이브 오류나서 일단 꺼둠
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

    // 첫 접속 하이스코어 찾아보기
    void LoadHighScore()
    {
        PlayGamesPlatform.Instance.LoadScores(
            GPGSIds.leaderboard_ranking,
            LeaderboardStart.PlayerCentered, // 내 점수를 기준으로 불러오기
            1,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (LeaderboardScoreData data) =>
            {
                if (data.Valid)
                {
                    long highScore = data.PlayerScore.value; // 이제 내 점수가 올바르게 들어옴
                    if (highScore > PlayerPrefs.GetInt("BestScore"))
                    {
                        PlayerPrefs.SetInt("BestScore", (int)highScore);
                        PlayerPrefs.Save();
                    }
                    Debug.Log($"내 하이스코어 불러오기 성공: {highScore}");
                    StartCoroutine(WaitLoadingSecond());
                }
                else
                {
                    Debug.Log("내 하이스코어 불러오기 실패");
                }
            });
    }

    // 로딩 슬라이더
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

    // 로딩 두번째
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


    //// 슬라이더 키기
    //void SliderOn(int first)
    //{
    //    slider.SetActive(true);
    //    loadingSlider = slider.GetComponent<Slider>();
    //    if(first == guest)  // 게스트일때
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
    //    Debug.Log("로그인 확인 중...");

    //    while (!PlayGamesPlatform.Instance.localUser.authenticated)
    //    {
    //        yield return new WaitForSeconds(0.5f);  // 0.5초마다 확인
    //    }

    //    Debug.Log("로그인 성공!");
    //}
}
