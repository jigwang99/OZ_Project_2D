using UnityEngine;
using System;

public enum FactionType
{
    Player,
    Enemy,
}

public class Faction
{
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
