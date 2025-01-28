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
        // Slider ���� ����� �� ȣ��� �޼��� ����
        bgmSlider.onValueChanged.AddListener(UpdateBgmVolume);
        sfxSlider.onValueChanged.AddListener(UpdateSfxVolume);
    }

    public void BgmInit()
    {
        //����� �÷��̾� �ʱ�ȭ
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
        // ȿ���� �÷��̾� �ʱ�ȭ
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
        Debug.Log("�����߻�");
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

    // Slider �� ���� �� AudioSource ���� ������Ʈ
    void UpdateBgmVolume(float value)
    {
        if (bgmPlayer != null)
        {
            bgmPlayer.volume = value;
        }
    } 
    
    // sfx �� ����
    void UpdateSfxVolume(float value)
    {
        sfxVolume = value;
    }

    // ���� ��������
    void GetBgmVolume()
    {
        if(PlayerPrefs.GetInt("isEditBgmVolume") == 1)
            bgmVolume = PlayerPrefs.GetFloat("BgmVolume");
    }
    
    // ��� ��ġ �����ϱ�
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


    // ��ư�� ���
    public void PlaySfxButton()
    {
        PlaySfx(Sfx.button);
    }
}
