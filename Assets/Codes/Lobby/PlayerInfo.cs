using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI score;

    private void Start()
    {
        nickname.text = PlayerPrefs.GetString("nickname");
        score.text = PlayerPrefs.GetInt("BestScore").ToString();
    }
}
