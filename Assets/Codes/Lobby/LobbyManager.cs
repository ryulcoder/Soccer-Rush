using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public GameObject playbuttonUi;
    Button playbutton;

    void Awake()
    {
        playbutton = playbuttonUi.GetComponent<Button>();
    }

    void Start()
    {
        playbutton.onClick.AddListener(StartOffBgm);
        playbutton.onClick.AddListener(StartChectAch);
    }

    void StartChectAch()
    {
        StartManager.Instance.CheckAch();
    }

    void StartOffBgm()
    {
        LobbyAudioManager.instance.StopBGM();
    }
    
}
