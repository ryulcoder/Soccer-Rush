using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 로비에 남아서 오디오 조절 해주는 것
public class AudioManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    
    private void Start()
    {
        // Slider 값이 변경될 때 호출될 메서드 연결
        bgmSlider.onValueChanged.AddListener(LobbyAudioManager.instance.UpdateBgmVolume);
        sfxSlider.onValueChanged.AddListener(LobbyAudioManager.instance.UpdateSfxVolume);
        LobbyAudioManager.instance.PlayBGMLobby();
    }

    // 브금 수치 조정하기
    public void SetBgmVolume()
    {
        LobbyAudioManager.instance.bgmVolume = bgmSlider.value;
        PlayerPrefs.SetFloat("BgmVolume", LobbyAudioManager.instance.bgmVolume);
        PlayerPrefs.SetInt("isEditBgmVolume", 1);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume()
    {
        LobbyAudioManager.instance.sfxVolume = sfxSlider.value;
        PlayerPrefs.SetFloat("SfxVolume", LobbyAudioManager.instance.sfxVolume);
        PlayerPrefs.SetInt("isEditSfxVolume", 1);
        PlayerPrefs.Save();
    }

}
