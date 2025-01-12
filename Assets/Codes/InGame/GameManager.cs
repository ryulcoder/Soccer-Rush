using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public float gameSpeed = 1;

    public float playerMoveSpeed = 0.5f;
    public float playerAniSpeed = 1;
    public float ballMoveSpeed = 280;

    public int count = 0;
    string nextTime;

    [Header("[ Code ]")]
    public Player Player;
    public BallMove BallMove;

    [Header("[ UI ]")]
    public Text Timer;

    [Header("[ Tile ]")]
    public Transform[] Tiles;

    [Header("[ Ani ]")]
    public Animator PlayerAnimator;
    public Animator TakleDefenderAnimator;
    public Animator SlidingDefenderAnimator;

    AnimatorStateInfo PlayerStateInfo;



    void Awake()
    {
        Player.speed = playerMoveSpeed;
        BallMove.speed = ballMoveSpeed;
        PlayerAnimator.speed = playerAniSpeed;

        Time.timeScale = 0;
    }

    void Update()
    {
        if (Time.timeScale > 0)
        {
            nextTime = TimeSpan.FromSeconds(Time.unscaledTime).ToString(@"mm\:ss").Replace(":", " : ");

            if (Timer.text != nextTime)
                Timer.text = nextTime;
        }
    }

    void FixedUpdate()
    {
        PlayerStateInfo = PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        if (PlayerStateInfo.IsName("Jump_Run"))
        {

        }

    }
    void LateUpdate()
    {
        if ((int)Time.unscaledTime / 10 - count >= 1)
        {
            count += 1;

            //GameSpeedUp();
        }
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
        Time.timeScale = gameSpeed;
        Player.PlayerStart();
    }

    public void GamePause()
    {
        Time.timeScale = 0;
    }
    public void GameReStart()
    {
        Time.timeScale = gameSpeed;
    }

}
