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
        resultTMP.text = result == GameManager.Result.Victory ? "Victory!!" : "Defeat";
    }
    private void OnClickMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
