using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUI : MonoBehaviour
{
    [Header("다중 선택")]
    [SerializeField] private List<UnitSlotUI> slots = new List<UnitSlotUI>();

    [Header("단일 선택")]
    [SerializeField] private GameObject info;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameTMP;
    [SerializeField] private TextMeshProUGUI hpTMP;
    [SerializeField] private TextMeshProUGUI attackTMP;
    [SerializeField] private TextMeshProUGUI defenseTMP;

    [Header("생산 큐")]
    [SerializeField] private ProductQueueUI productQueueUI; 

    private void Start()
    {
        foreach (UnitSlotUI slot in slots)
            slot.Clear();

        Player.instance.OnSelectionChanged += Refresh;
        Refresh();
    }
    private void OnDestroy()
    {
        if(Player.instance != null)
            Player.instance.OnSelectionChanged -= Refresh;
    }
    private void Update()
    {
        foreach(UnitSlotUI slot in slots)
            slot.UpdateHP();

        UpdateInfoHp();
    }
    private void Refresh()
    {
        IReadOnlyList<Unit> units = Player.instance.SelectUnitList;
        Building building = Player.instance.SelectBuilding;

        // 다중 선택
        if(units.Count > 1)
        {
            info.SetActive(false);
            productQueueUI.Clear();
            for(int i = 0; i < slots.Count; i++)
            {
                if (i < units.Count)
                    slots[i].Bind(units[i]);
                else
                    slots[i].Clear();
            }
            return;
        }

        foreach (UnitSlotUI slot in slots)
            slot.Clear();

        if (units.Count == 1)
        {
            productQueueUI.Clear();
            string label = units[0] is Monk ? "Heal" : "damage";
            ShowInfo(units[0].UnitStat.Icon, units[0].Type.ToString(), units[0].UnitStat.AttackDamage, units[0].UnitStat.Defense, label);
            return;
        }
        // 건물 선택
        if(building != null)
        {
            ShowInfo(building.BuildingStat.Icon, building.Type.ToString(), building.BuildingStat.AttackDamage, building.BuildingStat.Defense);

            if (building is ProductionBuilding pb)
                productQueueUI.Show(pb);
            else
                productQueueUI.Clear();
            return;
        }

        // 선택없음
        info.SetActive(false);
        productQueueUI.Clear();
    }
    private void ShowInfo(Sprite icon, string name, int attack, int defense, string label="damage")
    {
        info.SetActive(true);
        image.sprite = icon;
        nameTMP.text = name;
        defenseTMP.text = $"Armor : {defense}";
        
        attackTMP.gameObject.SetActive(attack > 0);
        if (attack > 0)
            attackTMP.text = $"{label} : {attack}";
    }
    private void UpdateInfoHp()
    {
        if (!info.activeSelf)
            return;

        IReadOnlyList<Unit> units = Player.instance.SelectUnitList;
        if(units.Count == 1)
        {
            hpTMP.text = $"{units[0].CurrentHp} / {units[0].UnitStat.MaxHp}";
            return;
        }

        Building building = Player.instance.SelectBuilding;
        if(building != null)
            hpTMP.text = $"{building.CurrentHp} / {building.BuildingStat.MaxHp}";
    }
}
