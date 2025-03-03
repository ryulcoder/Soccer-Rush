using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySetting : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Awake()
    {
        if(PlayerPrefs.GetFloat("BgmSet") == 0)
        {
            PlayerPrefs.SetFloat("BgmVolume", 1);
            PlayerPrefs.SetFloat("SfxVolume", 1);
            PlayerPrefs.SetFloat("BgmSet", 1);
            PlayerPrefs.Save();
        }
    }
    void OnEnable()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BgmVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
    }
}
