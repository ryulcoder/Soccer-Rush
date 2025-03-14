using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("[ Game ]")]
    public int score = 0;
    //[SerializeField] int distance = 0;
    int revive = 1;
    public GameObject continueButton;

    [Header("[ Game Setting ]")]
    [SerializeField] float gameSpeed = 1;
    [SerializeField] float playerMoveSpeed = 1;
    [SerializeField] float playerAniSpeed = 1.3f;
    [SerializeField] float ballMoveSpeed = 59;

    [Header("[ Def Setting ]")]
    [SerializeField] float minGap = 100;
    [SerializeField] float maxGap = 130;
    [SerializeField] float[] defPer = { 25, 25, 25, 15, 10};


    [Header("[ Code ]")]
    public Tutorial Tutorial;
    public PoolManager PoolManager;
    public ScoreCal ScoreCal;
    Player Player;
    BallMove BallMove;
    public GoogleAd googleAd;

    [Header("[ UI ]")]
    public GameObject GameEndBlurPanel;
    public GameObject PausePanel;
    public GameObject GameEndPanel;

    [Header("[ Tile ]")]
    public Transform[] Tiles;

    [Header("[ Ani ]")]
    public Animator PlayerAnimator;

    public (float minGap, float maxGap) DefGap => (minGap, maxGap);
    public float[] DefPer { get { return defPer; } }

    int count, reviveCount, revivePosition;
    bool coroutine, reviveSpeedUp;

    public bool aroundDefenderClear;


    void Awake()
    {
        Instance = this;
        Time.timeScale = 0;
    }

    private void Start()
    {
        Player = Player.Instance;
        BallMove = BallMove.instance;

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
        }
    }


    IEnumerator GameOver()
    {
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
        Time.timeScale = gameSpeed += 0.05f;

        Debug.Log("속도업!!!");
    }


    public void GameStart()
    {
        Player.PlayerStart();
        Time.timeScale = gameSpeed;
    }

    public void GamePause()
    {
        Time.timeScale = 0;
    }
    public void GameResume()
    {
        Time.timeScale = gameSpeed;
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
        googleAd.ShowInterstitialAd();
    }

    // 부활 광고 띄우기
    public void ReviveAd()
    {
        googleAd.ShowRewardedAd();
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
}
