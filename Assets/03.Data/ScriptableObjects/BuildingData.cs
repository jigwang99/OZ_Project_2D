using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    Castle,
    House,
    Barracks,
    Archery,
    Monastery,
}
public class BuildingStat
{
    [Header("Info")]
    [SerializeField] private BuildingType buildingType;

    [Header("Stat")]
    [SerializeField] private int maxHp;
    [SerializeField] private int defense;

    [Header("Production")]
    [SerializeField] private int woodCost;
    [SerializeField] private int goldCost;
    [SerializeField] private float buildTime;

    public BuildingType BuildingType => buildingType;
    public int MaxHp => maxHp;
    public int Defense => defense;
    public int WoodCost => woodCost;
    public int GoldCost => goldCost;
    public float BuildTime => buildTime;

    public BuildingStat(BuildingType buildingType, int maxHp, int defense, int woodCost, int goldCost, float buildTime)
    {
        this.buildingType = buildingType;
        this.maxHp = maxHp;
        this.defense = defense;
        this.woodCost = woodCost;
        this.goldCost = goldCost;
        this.buildTime = buildTime;
    }
    public BuildingStat Clone()
    {
        return new BuildingStat(buildingType, maxHp, defense, woodCost, goldCost, buildTime);
    }
}

[CreateAssetMenu(fileName = "BuildingData", menuName = "RTS/Building Data")]
public class BuildingData : ScriptableObject
{
    public List<BuildingStat> buildingList = new List<BuildingStat>();
}