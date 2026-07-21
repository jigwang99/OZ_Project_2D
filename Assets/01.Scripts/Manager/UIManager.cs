using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI goldTMP;
    [SerializeField] private TextMeshProUGUI woodTMP;
    [SerializeField] private TextMeshProUGUI populationTMP;

    private Faction faction;

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
        faction = FactionManager.instance.Player;  // Player 진영

        faction.OnResourceChanged += UpdateResourceUI;
        faction.OnPopulationChanged += UpdatePopulationUI;

        // 초기값
        UpdateResourceUI();
        UpdatePopulationUI();
    }
    private void UpdateResourceUI()
    {
        woodTMP.text = faction.Wood.ToString();
        goldTMP.text = faction.Gold.ToString();
    }
    private void UpdatePopulationUI()
    {
        populationTMP.text = $"{faction.CurrentPopulation} / {faction.MaxPopulation}";
    }
}
