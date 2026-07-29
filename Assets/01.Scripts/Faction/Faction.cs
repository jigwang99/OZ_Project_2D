using UnityEngine;
using System;
using System.Collections.Generic;
public class Faction
{
    private List<Unit> units = new List<Unit>();
    private List<Building> buildings = new List<Building>();

    public IReadOnlyList<Unit> Units => units;
    public IReadOnlyList<Building> Buildings => buildings;

    public FactionType Type {  get; private set; }

    public int Wood { get; private set; }
    public int Gold { get; private set; }

    private const int limitPopulation = 200;
    public int CurrentPopulation { get; private set; }
    public int MaxPopulation { get; private set; }

    public event Action OnResourceChanged;
    public event Action OnPopulationChanged;

    public Faction(FactionType type, int startWood, int startGold)
    {
        Type = type;
        Wood = startWood;
        Gold = startGold;
        MaxPopulation = 0;
    }
    // 유닛 등록
    public void RegisterUnit(Unit unit)
    {
        if (!units.Contains(unit))
            units.Add(unit);
    }
    public void UnregisterUnit(Unit unit)
    {
        if (units.Contains(unit))
            units.Remove(unit);
    }
    public int CountUnits(UnitType type)
    {
        int count = 0;
        for (int i = 0; i < units.Count; i++)
            if (units[i] != null && units[i].IsAlive && units[i].Type == type)
                count++;
        return count;
    }
    // 건물 등록
    public void RegisterBuilding(Building building)
    {
        if (!buildings.Contains(building))
            buildings.Add(building);
    }
    public void UnregisterBuilding(Building building)
    {
        if (buildings.Contains(building))
            buildings.Remove(building);
    }
    public bool HasBuilding(BuildingType type)
    {
        for (int i = 0; i < buildings.Count; i++)
            if (buildings[i] != null && buildings[i].IsAlive && buildings[i].Type == type)
                return true;
        return false;
    }
    public int CountBuildings(BuildingType type)
    {
        int count = 0;
        for (int i = 0; i < buildings.Count; i++)
            if (buildings[i] != null && Buildings[i].IsAlive && Buildings[i].Type == type)
                count++;
        return count;
    }
    // 자원
    public void AddResource(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Wood:
                AddWood(amount);
                break;
            case ResourceType.Gold:
                AddGold(amount);
                break;
        }
    }
    public void AddWood(int amount)
    {
        Wood += amount;
        OnResourceChanged?.Invoke();
    }
    public void AddGold(int amount)
    {
        Gold += amount;
        OnResourceChanged?.Invoke();
    }
    public bool TryReduceResource(int woodCost, int goldCost)
    {
        if (Wood < woodCost || Gold < goldCost)
            return false;

        Wood -= woodCost;
        Gold -= goldCost;
        OnResourceChanged?.Invoke();
        return true;
    }
    // 인구
    public bool TryIncreasePopulation(int amount)
    {
        if (CurrentPopulation + amount > MaxPopulation)
            return false;
        CurrentPopulation += amount;
        OnPopulationChanged?.Invoke();
        return true;
    }
    public void ReleasePopulation(int amount)
    {
        CurrentPopulation = Mathf.Max(0, CurrentPopulation - amount);
        OnPopulationChanged?.Invoke();
    }
    public void AddMaxPopulation(int amount)
    {
        MaxPopulation = Mathf.Max(0, MaxPopulation + amount);
        OnPopulationChanged?.Invoke();
    }
}
