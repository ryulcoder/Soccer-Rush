using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameAudioManager : MonoBehaviour
{
    // 로비 오디오 매니저에서 이어져옴
    // Start is called before the first frame update
    void Start()
    {
        if (LobbyAudioManager.instance == null)
            return;

        LobbyAudioManager.instance.PlayBGMIngame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySfxButton()
    {
        if(LobbyAudioManager.instance == null)
            return;

        LobbyAudioManager.instance.PlaySfxButton();
    }

    public void PlaySfxKick()
    {
        if (LobbyAudioManager.instance == null)
            return;


        LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.kick);
    }
}
