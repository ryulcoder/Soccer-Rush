using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LobbyAudioManager;

//인겜과 로비를 오가는 오디오 매니저
public class LobbyAudioManager : MonoBehaviour
{
    public static LobbyAudioManager instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public AudioClip ingameClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;
    bool needPlayBgm = true;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;
    float lowSfxVolume = 0.05f;
    public GameObject sfxObject;
    bool needPlaySfx;


    public enum Sfx {button, kick, shoot, shootHit, fallDown, death, bonusPoint, deathPunch, startWhistle, goalNet}

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        BgmInit();
        SfxInit();
    }

    private void Start()
    {
        GetSfxVolume();
        bgmPlayer.Play();
        // Slider 값이 변경될 때 호출될 메서드 연결
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
    public void UpdateBgmVolume(float value)
    {
        if (bgmPlayer != null)
        {
            bgmPlayer.volume = value;
        }
    }

    // sfx 값 조정
    public void UpdateSfxVolume(float value)
    {
        sfxVolume = value;
    }

    // 음악 가져오기
    void GetBgmVolume()
    {
        if(PlayerPrefs.GetInt("isEditBgmVolume") == 1)
            bgmVolume = PlayerPrefs.GetFloat("BgmVolume");
    }
    

    void GetSfxVolume()
    {
        if (PlayerPrefs.GetInt("isEditSfxVolume") == 1)
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume");
    }


    public void StopBGM()
    {
        if (bgmPlayer != null)
        {
            bgmPlayer.Stop();
        }
    }

    // 버튼음 출력
    public void PlaySfxButton()
    {
        PlaySfx(Sfx.button);
    }

    // 인겜 브금 틀어주기
    public void PlayBGMIngame()
    {
        bgmPlayer.clip = ingameClip;
        bgmPlayer.Play();
    }

    // 로비 브금 틀어주기
    public void PlayBGMLobby()
    {
        bgmPlayer.clip = bgmClip;
        bgmPlayer.Play();
    }
}
