using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI resultTMP;
    [SerializeField] private Button menuButton;
    [SerializeField] private string menuSceneName = "StartScene";

    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip defeatClip;

    private void Awake()
    {
        panel.SetActive(false);
        if(menuButton != null)
            menuButton.onClick.AddListener(OnClickMenu);
    }
    private void Start()
    {
        GameManager.instance.OnGameOver += Show;
    }
    private void OnDestroy()
    {
        if(GameManager.instance != null)
            GameManager.instance.OnGameOver -= Show;
    }
    private void Show(GameManager.Result result)
    {
        panel.SetActive(true);

        bool win = result == GameManager.Result.Victory;
        resultTMP.text = win ? "Victory!!" : "Defeat";

        AudioClip clip = win ? victoryClip : defeatClip;
        if(clip != null)
            AudioManager.instance?.PlaySFX(clip);
    }
    private void OnClickMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
