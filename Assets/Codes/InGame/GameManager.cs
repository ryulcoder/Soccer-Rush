using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("[ Game ]")]
    public GameObject continueButton;
    public GameObject ADBack;

    [Header("[ Ball Skins ]")]
    public GameObject[] ballSkins;
    static int ballSkinIdx;

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

    [Header("[ StartManager ]")]
    public float totalStamina = 100;
    public float regenTime = 2;
    public float scoreMulti = 1;

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

    int count, waitDisCount, stopPosition;
    bool coroutine, reSpeedUp, GameEnd;
    [SerializeField] bool isImpact;
    float impacDis = 200;

    public bool aroundDefenderClear, impactFail;

    static List<float[]> difficultyList = new List<float[]>
    {
        new float[] { 70, 30, 0, 0, 0, 0 },
        new float[] { 42.5f, 42.5f, 15, 0, 0, 0 },
        new float[] { 25, 25, 25, 25, 0, 0 },
        new float[] { 20, 20, 30, 30, 0, 0 },
        new float[] { 10, 20, 25, 45, 0, 0 },
        new float[] { 6, 17, 17, 40, 20, 0 },
        new float[] { 0, 15, 15, 40, 20, 10 },
        new float[] { 0, 15, 15, 50, 20, 10 },
        new float[] { 0, 15, 15, 40, 10, 20 },
        new float[] { 0, 15, 15, 45, 5, 20 },
        new float[] { 0, 15, 15, 35, 5, 30 }
    };

    public bool IsImpact {  
        get 
        {
            if (isImpact)
            {
                isImpact = false; 
                return true;
            }
            else 
                return false; 
        } 
    }

    void Awake()
    {
        Instance = this;

        Time.timeScale = 0;

        if (StartManager.Instance)
        {
            totalStamina = 100 + StartManager.Instance.staminaUse;
            regenTime = 2 - StartManager.Instance.staminaRegen;
            scoreMulti = StartManager.Instance.scoreMulti;

            ballSkinIdx = StartManager.Instance.ball;

            Debug.Log("스테미너 추가스텟 받음");
        }
        

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
        if ((Player.getTackled || impactFail) && !coroutine)
        {
            coroutine = true;

            Debug.LogWarning("최종스코어: " + ExtraScore.instance.CheckEndScore());

            StartCoroutine(GameOver());
        }

        if (!Player.getTackled && !impactFail && coroutine)
        {
            coroutine = false;
            aroundDefenderClear = false;
        }

        if (reSpeedUp && (ScoreCal.Distance - stopPosition) / 2 - waitDisCount >= 1)
        {
            waitDisCount++;

            Time.timeScale += (gameSpeed - 1) * 0.1f;

            if (Time.timeScale >= gameSpeed)
            {
                Time.timeScale = gameSpeed;
                reSpeedUp = false;
            }
                
        }

    }

    void LateUpdate()
    {
        if ((ScoreCal.Distance + 50) / impacDis - count >= 1)
        {
            count += 1;

            if (count >= 9)
            {
                impacDis = 800;
            }
            else if (count >= 6)
            {
                impacDis = 600;
            }
            else if (count >= 3)
            {
                impacDis = 400;
            }

            isImpact = true;
            IncreaseDifficulty();
        }
    }

    // 다시 게임으로 복귀할 때 호출
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !GameEnd)
        {
            GamePause();
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
            continueButton.SetActive(!impactFail);
        }

        PlayerDeathAd();

        if (Player.getTackled)
        {
            BallMove.gameObject.SetActive(false);
            Player.PlayerReset();
        }
       
    }


    public void BallReset()
    {
        BallMove.Reset();
    }

    public void GameSpeedUp()
    {
        gameSpeed += speedUp;

        Debug.LogWarning("스피드 업!");
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

    // 플레이어 살리기
    public void PlayerRevive()
    {
        GameEndPanel.SetActive(false);
        GameEndBlurPanel.SetActive(false);

        Player.GetComponent<Animator>().SetTrigger("ReStart");
        ReSpeedUp();
        Stamina.instance.RefillStamina();
        revive--;
        continueButton.SetActive(false);
    }


    public void ReSpeedUp()
    {
        reSpeedUp = true;
        stopPosition = ScoreCal.Distance;
    }

    public void InputOn()
    {
        StartCoroutine(InputOnDelay());
    }

    IEnumerator InputOnDelay()
    {
        yield return new WaitForSeconds(1);

        SwipInput.instance.Reset();
        Player.Instance.shootButton.gameObject.SetActive(true);
    }


    // 광고 뒷판 띄우기
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

    // 볼스킨 세팅
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

                break;

            case 1:
                minGap = 120;

                break;

            case 2:
                minGap = 110;

                break;

            case 3:
                minGap = 100;

                break;

            case 4:
                maxGap = 150;

                break;

            case 5:

                break;

            case 6:
                maxGap = 140;

                break;

            case 7:
                maxGap = 130;

                break;

            case 8:
                maxGap = 120;

                break;

            case 9:
                maxGap = 110;

                break;

            case 10:
                maxGap = 100;

                break;

            default:
                return;

        }

        defPer = difficultyList[count];

        Debug.LogWarning("난이도 업");
    }


}
