using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameAudioManager : MonoBehaviour
{
    // �κ� ����� �Ŵ������� �̾�����
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
