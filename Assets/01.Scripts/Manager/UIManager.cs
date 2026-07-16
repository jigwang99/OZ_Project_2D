using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI goldTMP;
    [SerializeField] private TextMeshProUGUI woodTMP;
    [SerializeField] private TextMeshProUGUI populationTMP;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        Player.instance.OnResourceChanged += UpdateResourceUI;
        Player.instance.OnPopulationChanged += UpdatePopulationUI;

        // 초기값
        UpdateResourceUI();
        UpdatePopulationUI();
    }
    private void UpdateResourceUI()
    {
        woodTMP.text = Player.instance.Wood.ToString();
        goldTMP.text = Player.instance.Gold.ToString();
    }
    private void UpdatePopulationUI()
    {
        populationTMP.text = $"{Player.instance.CurrentPopulation} / {Player.instance.MaxPopulation}";
    }
}
