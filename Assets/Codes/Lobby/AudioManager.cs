using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �κ� ���Ƽ� ����� ���� ���ִ� ��
public class AudioManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    
    private void Start()
    {
        // Slider ���� ����� �� ȣ��� �޼��� ����
        bgmSlider.onValueChanged.AddListener(LobbyAudioManager.instance.UpdateBgmVolume);
        sfxSlider.onValueChanged.AddListener(LobbyAudioManager.instance.UpdateSfxVolume);
        LobbyAudioManager.instance.PlayBGMLobby();
    }

    // ��� ��ġ �����ϱ�
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
