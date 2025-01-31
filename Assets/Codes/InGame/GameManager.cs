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
    [Header("[ Game ]")]
    [SerializeField] int score = 0;
    [SerializeField] float distance = 0;

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
    public Player Player;
    public BallMove BallMove;

    [Header("[ UI ]")]
    public Text Timer;
    public GameObject SuccessPanel;
    public GameObject PausePanel;
    public GameObject FailPanel;

    [Header("[ Tile ]")]
    public Transform[] Tiles;

    [Header("[ Ani ]")]
    public Animator PlayerAnimator;
    public Animator TakleDefenderAnimator;
    public Animator SlidingDefenderAnimator;

    public int Score { get { return score; }}
    public int Distance { get { return (int)distance;}}
    public (float minGap, float maxGap) DefGap => (minGap, maxGap);
    public float[] DefPer { get { return defPer; } }

    int count = 0;
    string nextTime;
    bool coroutine;

    Stopwatch PlayTime = new();

    void Awake()
    {
        Player.speed = playerMoveSpeed;
        BallMove.speed = ballMoveSpeed;
        PlayerAnimator.speed = playerAniSpeed;

        Time.timeScale = 0;
    }

    void Update()
    {
        if (PlayTime.IsRunning)
        {
            nextTime = TimeSpan.FromSeconds(PlayTime.Elapsed.TotalSeconds).ToString(@"mm\:ss").Replace(":", " : ");

            if (Timer.text != nextTime)
            {
                score += 1;
                distance += 0.1f;
                Timer.text = nextTime;
            }
        }

        if (Player.getTackled && !coroutine)
        {
            coroutine = true;
            StartCoroutine(GameFail());
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


    IEnumerator GameFail()
    {
        PlayTime.Stop();

        yield return new WaitForSecondsRealtime(1.7f);

        FailPanel.SetActive(true);
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

}
