using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager instance;

    public BallAc[] ballAcs;

    void Awake()
    {
        instance = this;
        //SetPlayerPrefs();
        PlayerPrefs.SetInt("basic", 1);
        PlayerPrefs.Save();

    }


    // ∫º √º¿Œ¡ˆ∏¶ ¿ß«ÿ ui ≤®¡‹
    public void ChangeBall()
    {
        int backBall = PlayerPrefs.GetInt("EquippedBall");

        foreach (BallAc ballAc in ballAcs)
        {
            if (ballAc.ballData.ballNo == backBall)
            {
                ballAc.UnSelectBall();
            }
        }
    }

    void SetPlayerPrefs()
    {
        PlayerPrefs.SetInt("OrangeBall", 1);
        PlayerPrefs.SetInt("StarBall", 1);
        PlayerPrefs.Save();
    }

}
