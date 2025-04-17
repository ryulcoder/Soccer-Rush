using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BallData;

public class BallAc : MonoBehaviour
{
    public BallData ballData;
    public GameObject EquipButton;
    public GameObject UnlockButton;
    public GameObject EquippedButton;

    void OnEnable()
    {
        if (PlayerPrefs.HasKey(ballData.ballName))
        {
            UnlockButton.SetActive(false);
            EquipButton.SetActive(true);

            // 공이 장착되어있을시
            if (PlayerPrefs.GetInt("EquippedBall") == ballData.ballNo)
            {
                int ball = PlayerPrefs.GetInt("EquippedBall");
                Debug.Log(ball);
                EquipButton.SetActive(false);
                EquippedButton.SetActive(true);
            }
        }

        // 기본 공 설정
        if (!PlayerPrefs.HasKey("EquippedBall"))
        {
            if (ballData.ballNo == 0)
            {
                EquipButton.SetActive(false);
                EquippedButton.SetActive(true);
            }
        }
    }

    public void SelectBall()
    {
        BallManager.instance.ChangeBall();
        PlayerPrefs.SetInt("EquippedBall", ballData.ballNo);
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetInt("EquippedBall"));

        EquipButton.SetActive(false);
        EquippedButton.SetActive(true);
    }

    public void UnSelectBall()
    {
        EquipButton.SetActive(true);
        EquippedButton.SetActive(false);
    }

}
