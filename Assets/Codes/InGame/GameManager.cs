using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("[ Game ]")]
    public GameObject continueButton;
    public GameObject ADBack;

    [Header("[ Ball Skins ]")]
    public GameObject[] ballSkins;

    [Header("[ Game Setting ]")]
    [SerializeField] float gameSpeed = 1;
    [SerializeField] float speedUp = 0.05f;
    [SerializeField] float playerMoveSpeed = 1;
    [SerializeField] float playerAniSpeed = 1.3f;
    [SerializeField] float ballMoveSpeed = 59;

    [Header("[ Def Setting ]")]
    [SerializeField] float minGap = 130;
    [SerializeField] float maxGap = 160;
    [SerializeField] float[] defPer = { 70, 30, 0, 0, 0, 0};

    [Header("[ Stamina ]")]
    public float totalStamina = 100;
    public float reGenRate = 0.05f;
    public Slider StaminaSlider;

    [Header("[ Code ]")]
    public GoogleAd googleAd;
    public Tutorial Tutorial;
    public PoolManager PoolManager;
    public ScoreCal ScoreCal;
    

    [Header("[ UI ]")]
    public GameObject GameEndBlurPanel;
    public GameObject PausePanel;
    public GameObject PauseBlurPanel;
    public GameObject GameEndPanel;

    [Header("[ Tile ]")]
    public Transform[] Tiles;

    [Header("[ Ani ]")]
    public Animator PlayerAnimator;

    public (float minGap, float maxGap) DefGap => (minGap, maxGap);
    public float[] DefPer { get { return defPer; } }

    Player Player;
    BallMove BallMove;

    int revive = 1;

    int count, reviveCount, revivePosition;
    bool coroutine, reviveSpeedUp;

    public bool aroundDefenderClear;
    bool GameEnd;

    static int ballSkinIdx;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 0;
#if (!UNITY_EDITOR)
        Shader.WarmupAllShaders();
#endif

        if (ballSkinIdx != PlayerPrefs.GetInt("BallSkin", 0))
            ballSkinIdx = PlayerPrefs.GetInt("BallSkin", 0);

    }

    private void Start()
    {
        Player = Player.Instance;
        BallMove = BallMove.instance;

        SetBallSkin();

        Tutorial.gameSpeed = gameSpeed;

        Player.speed = playerMoveSpeed;
        BallMove.speed = ballMoveSpeed;
        PlayerAnimator.speed = playerAniSpeed;

        GameStart();

        if (LobbyAudioManager.instance != null)
        {
            LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.startWhistle);
        }

    }

    void Update()
    {
        if (Player.getTackled && !coroutine)
        {
            coroutine = true;

            Debug.LogWarning("최종스코어: " + ExtraScore.instance.CheckEndScore());

            StartCoroutine(GameOver());
        }

        if (!Player.getTackled && coroutine)
        {
            coroutine = false;
            aroundDefenderClear = false;
        }

        if (reviveSpeedUp && (ScoreCal.Distance - revivePosition) / 2 - reviveCount >= 1)
        {
            reviveCount++;

            Time.timeScale += (gameSpeed - 1) * 0.1f;

            if (Time.timeScale >= gameSpeed)
            {
                Time.timeScale = gameSpeed;
                reviveSpeedUp = false;
            }
                
        }

    }

    void LateUpdate()
    {
        if (!reviveSpeedUp && ScoreCal.Distance / 50 - count >= 1)
        {
            count += 1;

            GameSpeedUp();
            IncreaseDifficulty();
        }
    }

    IEnumerator GameOver()
    {
        Time.timeScale = 1;
        GameEnd = true;
        yield return new WaitForSecondsRealtime(1.7f);

        aroundDefenderClear = true;

        GameEndPanel.SetActive(true);
        GameEndBlurPanel.SetActive(true);
        ScoreCal.SetResult();
        if(revive > 0)
        {
            continueButton.SetActive(true);
        }
        PlayerDeathAd();

        BallMove.gameObject.SetActive(false);
        Player.PlayerReset();
    }


    public void BallReset()
    {
        BallMove.Reset();
    }

    public void GameSpeedUp()
    {
        Time.timeScale = gameSpeed += speedUp;

        Debug.Log("속도업!!!");
    }


    public void GameStart()
    {
        Player.PlayerStart();
        Time.timeScale = gameSpeed;
    }

    public void GamePause()
    {
        PausePanel.SetActive(true);
        PauseBlurPanel.SetActive(true);

        Time.timeScale = 0;
    }
    public void GameResume()
    {
        Time.timeScale = gameSpeed; 
        PausePanel.SetActive(false);
        PauseBlurPanel.SetActive(false);
    }

    public void GameReStart()
    {
        SceneManager.LoadScene("InGame");
    }

    public void GoLobby()
    {
        SceneManager.LoadScene("Lobby");
        Time.timeScale = 1;
    }

    // 플레이어 죽으면 광고 띄우기
    public void PlayerDeathAd()
    {
        ADBackOn();
        googleAd.ShowInterstitialAd();
    }

    // 부활 광고 띄우기
    public void ReviveAd()
    {
        ADBackOn();
        googleAd.ShowRewardedAd();
    }

    // 다시 게임으로 복귀할 때 호출
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !GameEnd)
        {
            GamePause();
        }
    }

    public void PlayerRevive()
    {
        GameEndPanel.SetActive(false);
        GameEndBlurPanel.SetActive(false);

        Player.GetComponent<Animator>().SetTrigger("ReStart");

        revive--;
        continueButton.SetActive(false);

        revivePosition = ScoreCal.Distance;
        reviveSpeedUp = true;
    }

    void ADBackOn()
    {
        ADBack.SetActive(true);
        StartCoroutine(ADBackOffCoroutine());
    }

    IEnumerator ADBackOffCoroutine()
    { 
        yield return new WaitForSeconds(0.2f);

        ADBack.SetActive(false);

        yield break;
    }


    void SetBallSkin()
    {
        if (ballSkinIdx == 0) return;

        BallMove.GetComponent<MeshFilter>().sharedMesh = ballSkins[ballSkinIdx].GetComponent<MeshFilter>().sharedMesh;
        BallMove.GetComponent<MeshRenderer>().sharedMaterials = ballSkins[ballSkinIdx].GetComponent<MeshRenderer>().sharedMaterials;
    }


    void IncreaseDifficulty()
    {

        // Def [ stand, sliding, anomaly, 2_def, 3_def, 3_def_anomaly]
        switch (count)
        {
            case 0: // 첫 설정값
                minGap = 130;
                maxGap = 160;

                defPer = new float[] { 70, 30, 0, 0, 0, 0 };
                break;

            case 1:
                minGap = 120;
                defPer = new float[] { 42.5f, 42.5f, 15, 0, 0, 0 };

                break;

            case 2:
                minGap = 110;
                defPer = new float[] { 25, 25, 25, 25, 0, 0 };

                break;

            case 3:
                minGap = 100;
                defPer = new float[] { 20, 20, 30, 30, 0, 0 };

                break;

            case 4:
                maxGap = 150;
                defPer = new float[] { 10, 20, 25, 45, 0, 0 };

                break;

            case 5:
                defPer = new float[] { 6, 17, 17, 30, 30, 0 };

                break;

            case 6:
                maxGap = 140;
                defPer = new float[] { 0, 15, 15, 40, 25, 5 };

                break;

            case 7:
                maxGap = 130;
                defPer = new float[] { 0, 15, 15, 50, 20, 10 };

                break;

            case 8:
                maxGap = 120;
                defPer = new float[] { 0, 15, 15, 40, 10, 20 };

                break;

            case 9:
                maxGap = 110;
                defPer = new float[] { 0, 15, 15, 40, 5, 25 };

                break;

            case 10:
                maxGap = 100;
                defPer = new float[] { 0, 15, 15, 35, 5, 30 };

                break;

            default:
                return;

        }
    }


}
