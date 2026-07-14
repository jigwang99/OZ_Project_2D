using System;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    Castle,
    House,
    Barracks,
    Archery,
    Monastery,
    Tower,
}
[Serializable]
public class BuildingStat
{
    [Header("Info")]
    [SerializeField] private BuildingType buildingType;

    [Header("Stat")]
    [SerializeField] private int maxHp;
    [SerializeField] private int defense;

    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    [Header("Production")]
    [SerializeField] private int woodCost;
    [SerializeField] private int goldCost;
    [SerializeField] private float buildTime;

    public BuildingType BuildingType => buildingType;
    public int MaxHp => maxHp;
    public int Defense => defense;
    public int AttackDamage => attackDamage;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public int WoodCost => woodCost;
    public int GoldCost => goldCost;
    public float BuildTime => buildTime;

    public BuildingStat(BuildingType buildingType, int maxHp, int defense, int attackDamage, float attackRange, float attackCooldown, int woodCost, int goldCost, float buildTime)
    {
        this.buildingType = buildingType;

        this.maxHp = maxHp;
        this.defense = defense;

        this.attackDamage = attackDamage;
        this.attackRange = attackRange;
        this.attackCooldown = attackCooldown;

        this.woodCost = woodCost;
        this.goldCost = goldCost;
        this.buildTime = buildTime;
    }
    public BuildingStat Clone()
    {
        return new BuildingStat(buildingType, maxHp, defense, attackDamage, attackRange, attackCooldown, woodCost, goldCost, buildTime);
    }
}

[CreateAssetMenu(fileName = "BuildingData", menuName = "RTS/Building Data")]
public class BuildingData : ScriptableObject
{
    public List<BuildingStat> buildingList = new List<BuildingStat>();
}