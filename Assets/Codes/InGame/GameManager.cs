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

    [Header("[ Game Setting ]")]
    [SerializeField] float gameSpeed = 1;
    [SerializeField] float playerMoveSpeed = 1;
    [SerializeField] float playerAniSpeed = 1.3f;
    [SerializeField] float ballMoveSpeed = 60;

    [Header("[ Def Setting ]")]
    [SerializeField] float minGap = 80;
    [SerializeField] float maxGap = 100;
    [SerializeField] float[] defPer = { 25, 25, 25, 15, 10};


    [Header("[ Code ]")]
    public PoolManager PoolManager;
    public ScoreCal ScoreCal;
    Player Player;
    BallMove BallMove;

    [Header("[ UI ]")]
    public GameObject GameEndBlurPanel;
    public GameObject PausePanel;
    public GameObject GameEndPanel;

    [Header("[ Tile ]")]
    public Transform[] Tiles;

    [Header("[ Ani ]")]
    public Animator PlayerAnimator;
    public Animator TakleDefenderAnimator;
    public Animator SlidingDefenderAnimator;

    public (float minGap, float maxGap) DefGap => (minGap, maxGap);
    public float[] DefPer { get { return defPer; } }

    int count = 0;
    string nextTime;
    bool coroutine;

    Stopwatch PlayTime = new();

    void Awake()
    {
        Instance = this;
        Time.timeScale = 0;
    }

    private void Start()
    {
        Player = Player.Instance;
        BallMove = BallMove.instance;

        Player.speed = playerMoveSpeed;
        BallMove.speed = ballMoveSpeed;
        PlayerAnimator.speed = playerAniSpeed;
        GameStart();
    }

    void Update()
    {
        if (Player.getTackled && !coroutine)
        {
            coroutine = true;

            ExtraScore.instance.CheckEndScore();

            StartCoroutine(GameEnd());
            
        }
    }

    void LateUpdate()
    {
        if ((int)PlayTime.Elapsed.TotalSeconds / 10 - count >= 1)
        {
            count += 1;

            //GameSpeedUp();
        }
    }


    IEnumerator GameEnd()
    {
        PlayTime.Stop();

        yield return new WaitForSecondsRealtime(1.7f);
        
        GameEndPanel.SetActive(true);
        GameEndBlurPanel.SetActive(true);
        ScoreCal.SaveScore();
    }

    public void BallReset()
    {
        BallMove.Reset();
    }

    public void GameSpeedUp()
    {
        Time.timeScale *= 1.1f;

        Debug.Log("¼Óµµ¾÷!!!");
    }


    public void GameStart()
    {
        Player.PlayerStart();
        Time.timeScale = gameSpeed;

        PlayTime.Start();
    }

    public void GamePause()
    {
        PlayTime.Stop();
        Time.timeScale = 0;
    }
    public void GameResume()
    {
        PlayTime.Start();
        Time.timeScale = gameSpeed;
    }

    public void GameReStart()
    {
        SceneManager.LoadScene("InGame");
    }

    public void GoLobby()
    {
        SceneManager.LoadScene("Lobby");

    }
}
