using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class ProductionBuilding : Building
{
    [SerializeField] private List<UnitType> producibleUnits;
    [SerializeField] private List<UnitType> productList = new List<UnitType>();

    public const int MaxProductList = 7;
    public bool IsQueueFull => productList.Count >= MaxProductList;

    public BuildingProductState ProductState { get; protected set; }
    public bool HasList => productList.Count > 0;
    public float CurrentProductTime => UnitDataLoader.instance.GetUnitStat(productList.FirstOrDefault()).ProductTime;

    [SerializeField] private Transform spawnPosition;

    public IReadOnlyList<UnitType> ProductList => productList;
    public event Action OnProductChanged;

    public float ProductProgress
    {
        get
        {
            if (!HasList || StateMachine.CurrentState != ProductState)
                return 0f;
            return 1f - ProductState.RemainTimer / CurrentProductTime;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        ProductState = new BuildingProductState(this);
    }
    public bool EnqueueUnit(UnitType unitType)
    {
        if (productList.Count >= MaxProductList)
            return false;

        if (!producibleUnits.Contains(unitType))
            return false;

        if (!CanProduce(unitType))
            return false;

        UnitStat unitStat = UnitDataLoader.instance.GetUnitStat(unitType);

        if (!OwnerFaction.TryIncreasePopulation(unitStat.Population))
            return false;

        if (!OwnerFaction.TryReduceResource(unitStat.WoodCost, unitStat.GoldCost))
        {
            OwnerFaction.ReleasePopulation(unitStat.Population);
            return false;
        }

        productList.Add(unitType);
        OnProductChanged?.Invoke();

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
        CancelProductAt(productList.Count - 1);
    }
    public void CancelProductAt(int index)
    {
        if (index < 0 || index >= productList.Count)
            return;

        Refund(productList[index]);
        productList.RemoveAt(index);

        if (productList.Count == 0)
        {
            if (StateMachine.CurrentState == ProductState)
                StateMachine.ChangeState(IdleState);
        }
        else if(index == 0 && StateMachine.CurrentState == ProductState)
        {
            StateMachine.ChangeState(ProductState);
        }
        OnProductChanged?.Invoke();
    }
    public void CompleteProduction()
    {
        if(productList.Count == 0)
            return;

        // 첫번 째 유닛 리스트에서 제거
        UnitType unitType = productList.FirstOrDefault();
        productList.RemoveAt(0);
        OnProductChanged?.Invoke();

        //유닛스폰
        Unit unit = ObjectPoolManager.instance.GetObject<Unit>(unitType.ToString());
        unit.SetLayer(gameObject.layer == (int)Layer.PlayerBuilding ? Layer.Player : Layer.Enemy);
        unit.transform.position = spawnPosition.position;
    }
    private void Refund(UnitType unitType)
    {
        UnitStat unitStat = UnitDataLoader.instance.GetUnitStat(unitType);
        OwnerFaction.AddWood(unitStat.WoodCost);
        OwnerFaction.AddGold(unitStat.GoldCost);
        OwnerFaction.ReleasePopulation(unitStat.Population);
    }
    public bool CanProduce(UnitType unitType)
    {
        UnitStat stat = UnitDataLoader.instance.GetUnitStat(unitType);
        if (stat == null)
            return false;
        if (stat.RequiredBuilding == BuildingType.None)
            return true;
        return OwnerFaction.HasBuilding(stat.RequiredBuilding);
    }
    public bool CanProduceType(UnitType unitType)
    {
        if(!producibleUnits.Contains(unitType)) // 생산 종류 검사
            return false;
        return CanProduce(unitType);
    }
    protected override void OnDisable()
    {
        for(int i = productList.Count - 1; i >= 0; i++)
        {
            Refund(productList[i]);
            productList.RemoveAt(i);
        }
        OnProductChanged?.Invoke();
    }
}
