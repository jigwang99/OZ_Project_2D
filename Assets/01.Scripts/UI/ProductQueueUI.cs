using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductQueueUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private List<ProductSlotUI> slots;
    [SerializeField] private Image progressFill;

    private ProductionBuilding current;

    private void Awake()
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].SetUp(this, i);
    }
    public void Show(ProductionBuilding building)
    {
        if(current != null)
            current.OnProductChanged -= RefreshQueue;

        current = building;
        current.OnProductChanged += RefreshQueue;
        panel.SetActive(true);
        RefreshQueue();
    }
    private void Update()
    {
        if (current == null)        
            return;
        progressFill.fillAmount = current.ProductProgress;
    }
    public void Clear()
    {
        if(current != null)
        {
            current.OnProductChanged -= RefreshQueue;
            current = null;
        }
        panel.SetActive(false);
    }
    private void RefreshQueue()
    {
        IReadOnlyList<UnitType> list = current.ProductList;

        for(int i = 0; i< slots.Count; i++)
        {
            if (i < list.Count)
                slots[i].Bind(UnitManager.instance.GetUnitStat(list[i]).Icon);
            else
                slots[i].Clear();
        }
        progressFill.gameObject.SetActive(list.Count > 0);
    }
        public void RequestCancel(int index)
        {
        current?.CancelProductAt(index);
    }
}
