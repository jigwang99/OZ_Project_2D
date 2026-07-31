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

    [Header("location based reduction")]
    [SerializeField] private float minDistance = 6f;
    [SerializeField] private float maxDistance = 25f;

    private const string MasterKey = "vol_master";
    private const string BGMKey = "vol_bgm";
    private const string SFXKey = "vol_sfx";
    private const string MutKey = "vol_mute";

    private Camera listerCamera;

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
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null)
            return;
        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    public void PlaySFX(AudioClip clip, float scale = 1f)
    {
        if (sfxSource == null || clip == null)
            return;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(scale));
    }
    public void PlaySFXAt(AudioClip clip, Vector2 position, float scale = 1f)
    {
        if (clip == null)
            return;

        float reduce = DistanceReduce(position);

        if (reduce <= 0f)
            return;

        PlaySFX(clip, scale * reduce);
    }
    private float DistanceReduce(Vector2 position)
    {
        float dist = Vector2.Distance(ListnerPosition(), position);

        if (dist <= minDistance)
            return 1f;
        if (dist >= maxDistance)
            return 0f;
        return 1f - (dist - minDistance) / (maxDistance - minDistance);
    }
    private Vector2 ListnerPosition()
    {
        if(listerCamera == null)
            listerCamera = Camera.main;
        return listerCamera != null ? (Vector2)listerCamera.transform.position : Vector2.zero;
    }
}
