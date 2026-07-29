using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingStat
{
    [Header("Info")]
    [SerializeField] private BuildingType buildingType;

    [Header("Stat")]
    [SerializeField] private int maxHp;
    [SerializeField] private int defense;

    [SerializeField] private int populationProvide;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    [Header("Production")]
    [SerializeField] private int woodCost;
    [SerializeField] private int goldCost;
    [SerializeField] private float buildTime;

    [Header("Icon")]
    [SerializeField] private Sprite icon;

    public BuildingType BuildingType => buildingType;
    public int MaxHp => maxHp;
    public int Defense => defense;
    public int PopulationProvide => populationProvide;
    public int AttackDamage => attackDamage;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public int WoodCost => woodCost;
    public int GoldCost => goldCost;
    public float BuildTime => buildTime;
    public Sprite Icon => icon;
    public BuildingStat(BuildingType buildingType, int maxHp, int defense, int populationProvide, int attackDamage, float attackRange, float attackCooldown, int woodCost, int goldCost, float buildTime, Sprite icon)
    {
        this.buildingType = buildingType;

        this.maxHp = maxHp;
        this.defense = defense;

        this.populationProvide = populationProvide;
        this.attackDamage = attackDamage;
        this.attackRange = attackRange;
        this.attackCooldown = attackCooldown;

        this.woodCost = woodCost;
        this.goldCost = goldCost;
        this.buildTime = buildTime;

        this.icon = icon;
    }
    public BuildingStat Clone()
    {
        return new BuildingStat(buildingType, maxHp, defense, populationProvide, attackDamage, attackRange, attackCooldown, woodCost, goldCost, buildTime, icon);
    }
}

[CreateAssetMenu(fileName = "BuildingData", menuName = "RTS/Building Data")]
public class BuildingData : ScriptableObject
{
    public List<BuildingStat> buildingList = new List<BuildingStat>();
}