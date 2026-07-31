using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    public static OptionPanel instance;

    [Header("panel")]
    [SerializeField] private GameObject panel;

    [Header("mute")]
    [SerializeField] private Toggle muteToggle;

    [Header("Master Volume")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeTMP;

    [Header("BGM Volume")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmVolumeTMP;

    [Header("SFX Volume")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxVolumeTMP;

    [Header("Close")]
    [SerializeField] private Button closeButton;

    [Header("Open")]
    [SerializeField] private Button openButton;

    [Header("Menu")]
    [SerializeField] private Button menuButton;
    [SerializeField] private string menuSceneName = "StartScene";

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        if (muteToggle != null)
            muteToggle.onValueChanged.AddListener(OnMuteChanged);

        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(OnMasterChanged);
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
        if(openButton != null)
            openButton.onClick.AddListener(Open);
        if (menuButton != null)
            menuButton.onClick.AddListener(OnClickMenu);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
        InitFromManager();
        UpdateMenuButton(SceneManager.GetActiveScene().name);
    }
    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitFromManager();
        UpdateMenuButton(scene.name);
        Close();
    }
    private void UpdateMenuButton(string sceneName)
    {
        if (menuButton == null)
            return;
        menuButton.gameObject.SetActive(sceneName != menuSceneName);
    }
    private void InitFromManager()
    {
        if (AudioManager.instance == null)
            return;

        if(muteToggle != null)
            muteToggle.SetIsOnWithoutNotify(AudioManager.instance.IsMuted);

        if (masterSlider != null)
            masterSlider.SetValueWithoutNotify(AudioManager.instance.MasterVolume);
        if(bgmSlider != null)
            bgmSlider.SetValueWithoutNotify(AudioManager.instance.BgmVolume);
        if(sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(AudioManager.instance.SFXVolume);

        UpdateText(masterVolumeTMP, AudioManager.instance.MasterVolume);
        UpdateText(bgmVolumeTMP, AudioManager.instance.BgmVolume);
        UpdateText(sfxVolumeTMP, AudioManager.instance.SFXVolume);
    }
    public void Open()
    {
        InitFromManager();
        if(panel != null)
            panel.SetActive(true);
    }
    public void Close()
    {
        if(panel != null)
            panel.SetActive(false);
    }
    public void OnClickMenu()
    {
        Time.timeScale = 1.0f;
        Close();
        SceneManager.LoadScene(menuSceneName);
    }
    private void OnMuteChanged(bool mute)
    {
        AudioManager.instance?.SetMute(mute);
    }
    private void OnMasterChanged(float value)
    {
        AudioManager.instance?.SetMasterVolume(value);
        UpdateText(masterVolumeTMP, value);
    }
    private void OnBGMChanged(float value)
    {
        AudioManager.instance?.SetBGMVolume(value);
        UpdateText(bgmVolumeTMP, value);
    }
    private void OnSFXChanged(float value)
    {
        AudioManager.instance.SetSFXVolume(value);
        UpdateText(sfxVolumeTMP, value);
    }
    private void UpdateText(TextMeshProUGUI tmp, float value)
    {
        if (tmp == null)
            return;
        tmp.text = $"{Mathf.RoundToInt(value * 100f)}%";
    }
}
