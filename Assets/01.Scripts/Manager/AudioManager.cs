using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Souce")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Clip")]
    [SerializeField] private AudioClip bgmClip;

    [Header("default Volume")]
    [SerializeField, Range(0f, 1f)] private float defaultMaster = 1f;
    [SerializeField, Range(0f, 1f)] private float defaultBGM = 1f;
    [SerializeField, Range(0f, 1f)] private float defaultSFX = 1f;

    private const string MasterKey = "vol_master";
    private const string BGMKey = "vol_bgm";
    private const string SFXKey = "vol_sfx";
    private const string MutKey = "vol_mute";

    public event Action OnVolumeChanged;

    public float  MasterVolume {  get; private set; }
    public float BgmVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public bool IsMuted { get; private set; }

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Load();
    }
    private void Load()
    {
        MasterVolume = PlayerPrefs.GetFloat(MasterKey, defaultMaster);
        BgmVolume = PlayerPrefs.GetFloat(BGMKey, defaultBGM);
        SFXVolume = PlayerPrefs.GetFloat(SFXKey, defaultSFX);
        IsMuted = PlayerPrefs.GetInt(MutKey, 0) == 1;
        ApplyVolume();
    }
    private void ApplyVolume()
    {
        float master = IsMuted ? 0f : MasterVolume;

        if (bgmSource != null)
            bgmSource.volume = master * BgmVolume;
        if(sfxSource != null)
            sfxSource.volume = master * SFXVolume;

        OnVolumeChanged?.Invoke();
    }
    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MasterKey, MasterVolume);
        ApplyVolume();
    }
    public void SetBGMVolume(float value)
    {
        BgmVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(BGMKey, BgmVolume);
        ApplyVolume();
    }
    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SFXKey, SFXVolume);
        ApplyVolume();
    }
    public void SetMute(bool mute)
    {
        IsMuted = mute;
        PlayerPrefs.SetInt(MutKey, mute ? 1 : 0);
        ApplyVolume();
    }
    public void PlayBGM()
    {
        bgmSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;
        sfxSource.PlayOneShot(clip);
    }
}
