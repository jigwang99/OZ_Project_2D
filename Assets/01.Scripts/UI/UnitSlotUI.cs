using UnityEngine;
using UnityEngine.UI;

public class UnitSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image hpBar;
    [SerializeField] private Button button;

    private Unit unit;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }
    public void Bind(Unit unit)
    {
        this.unit = unit;
        iconImage.sprite = unit.UnitStat.Icon;
        gameObject.SetActive(true);
        UpdateHP();
    }
    public void Clear()
    {
        unit = null;
        gameObject.SetActive(false);
    }
    public void UpdateHP()
    {
        if (unit == null)
            return;
        hpBar.fillAmount = (float) unit.CurrentHp / unit.UnitStat.MaxHp;
    }
    private void OnClick()
    {
        if(unit != null && unit.IsAlive)
            Player.instance.SelectSingleUnit(unit);
    }
}
