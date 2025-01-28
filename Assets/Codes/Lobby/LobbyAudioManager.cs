using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LobbyAudioManager;

public class LobbyAudioManager : MonoBehaviour
{
    public static LobbyAudioManager instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;
    bool needPlayBgm = true;
    public Slider bgmSlider;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;
    float lowSfxVolume = 0.05f;
    public GameObject sfxObject;
    bool needPlaySfx;
    public Slider sfxSlider;


    public enum Sfx {button}

    void Awake()
    {
        instance = this;
        BgmInit();
        SfxInit();
    }

    private void Start()
    {
        GetSfxVolume();
        bgmPlayer.Play();
        // Slider 값이 변경될 때 호출될 메서드 연결
        bgmSlider.onValueChanged.AddListener(UpdateBgmVolume);
        sfxSlider.onValueChanged.AddListener(UpdateSfxVolume);
    }

    public void BgmInit()
    {
        //배경은 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        GetBgmVolume();
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

    }

    public void SfxInit()
    {
        // 효과음 플레이어 초기화
        sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int i = 0; i < channels; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
            sfxPlayers[i].bypassEffects = true;

        }

        needPlaySfx = true;
    }


    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
            needPlayBgm = true;
        }
        else
        {
            bgmPlayer.Stop();
            needPlayBgm = false;
        }
    }

    public void OffSfx()
    {
        needPlaySfx = false;
        if (sfxObject != null)
            Destroy(sfxObject);
    }

    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        if (sfxObject == null)
            return;

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            sfxPlayers[loopIndex].volume = sfxVolume;
            break;
        }
    }

    public void SetIngameSound()
    {
        int bgmNeed = needPlayBgm == true ? 1 : 2;
        PlayerPrefs.SetInt("IngameBGM", bgmNeed);

        int sfxNeed = needPlaySfx == true ? 1 : 2;
        PlayerPrefs.SetInt("IngameSFX", sfxNeed);
        PlayerPrefs.Save();
        Debug.Log("설정했삼");
    }

    public void GetIngameSound()
    {
        if (PlayerPrefs.GetInt("IngameBGM") == 2)
        {
            Debug.Log(PlayerPrefs.GetInt("IngameBGM"));
            bgmPlayer.Stop();
            needPlayBgm = false;
        }

        if (PlayerPrefs.GetInt("IngameSFX") == 2)
        {
            OffSfx();
            needPlaySfx = false;
        }
    }

    // Slider 값 변경 시 AudioSource 볼륨 업데이트
    void UpdateBgmVolume(float value)
    {
        if (bgmPlayer != null)
        {
            bgmPlayer.volume = value;
        }
    } 
    
    // sfx 값 조정
    void UpdateSfxVolume(float value)
    {
        sfxVolume = value;
    }

    // 음악 가져오기
    void GetBgmVolume()
    {
        if(PlayerPrefs.GetInt("isEditBgmVolume") == 1)
            bgmVolume = PlayerPrefs.GetFloat("BgmVolume");
    }
    
    // 브금 수치 조정하기
    public void SetBgmVolume()
    {
        bgmVolume = bgmSlider.value;
        PlayerPrefs.SetFloat("BgmVolume", bgmVolume);
        PlayerPrefs.SetInt("isEditBgmVolume", 1 );
        PlayerPrefs.Save();
    }

    void GetSfxVolume()
    {
        if (PlayerPrefs.GetInt("isEditSfxVolume") == 1)
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume");
    }

    public void SetSfxVolume()
    {
        sfxVolume = sfxSlider.value;
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
        PlayerPrefs.SetInt("isEditSfxVolume", 1);
        PlayerPrefs.Save();
    }


    // 버튼음 출력
    public void PlaySfxButton()
    {
        PlaySfx(Sfx.button);
    }
}
