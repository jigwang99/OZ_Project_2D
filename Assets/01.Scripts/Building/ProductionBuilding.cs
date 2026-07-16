using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class ProductionBuilding : Building
{
    [SerializeField] private List<UnitType> producibleUnits;

    [SerializeField] private List<UnitType> productList = new List<UnitType>();

    public BuildingProductState ProductState { get; protected set; }
    public bool HasList => productList.Count > 0;
    public float CurrentProductTime => UnitManager.instance.GetUnitStat(productList.FirstOrDefault()).ProductTime;

    [SerializeField] private Transform spawnPosition;

    protected override void Awake()
    {
        base.Awake();
        ProductState = new BuildingProductState(this);
    }
    public bool EnqueueUnit(UnitType unitType)
    {
        if (!producibleUnits.Contains(unitType))
            return false;

        UnitStat unitStat = UnitManager.instance.GetUnitStat(unitType);

        if (!Player.instance.TryIncreasePopulation(unitStat.Population))
            return false;

        if (!Player.instance.TryReduceResource(unitStat.WoodCost, unitStat.GoldCost))
        {
            Player.instance.ReleasePopulation(unitStat.Population);
            return false;
        }

        productList.Add(unitType);

        if (StateMachine.CurrentState == IdleState)
            StateMachine.ChangeState(ProductState);
        return true;
    }
    public bool EnqueueUnitByIndex(int index)
    {
        if (index < 0 || index >= producibleUnits.Count)
            return false;

        return EnqueueUnit(producibleUnits[index]);
    }
    public void CancelLastProduct()
    {
        if (productList.Count == 0)
            return;

        int lastIndex = productList.Count - 1;
        Refund(productList[lastIndex]);
        productList.RemoveAt(lastIndex);
    }
    public void CompleteProduction()
    {
        // 첫번 째 유닛 리스트에서 제거
        UnitType unitType = productList.FirstOrDefault();
        productList.RemoveAt(0);

        //유닛스폰
        Unit unit = ObjectPoolManager.instance.GetObject<Unit>(unitType.ToString());
        unit.transform.position = spawnPosition.position;
    }
    private void Refund(UnitType unitType)
    {
        UnitStat unitStat = UnitManager.instance.GetUnitStat(unitType);
        Player.instance.AddWood(unitStat.WoodCost);
        Player.instance.AddGold(unitStat.GoldCost);
        Player.instance.ReleasePopulation(unitStat.Population);
    }
}
