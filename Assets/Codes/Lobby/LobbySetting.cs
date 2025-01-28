using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySetting : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    void OnEnable()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BgmVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
    }
}
