using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitStat
{
    [Header("Info")]
    [SerializeField] private UnitType unitType;

    [Header("Stats")]
    [SerializeField] private int maxHp;
    [SerializeField] private int attackDamage;
    [SerializeField] private int defense;
    [SerializeField] private int population;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float vision;

    [Header("Production")]
    [SerializeField] private int woodCost;
    [SerializeField] private int goldCost;
    [SerializeField] private float productTime;
    [SerializeField] private BuildingType requiredBuilding;

    [Header("Gather")]
    [SerializeField] private int gatherAmount;

    [Header("Icon")]
    [SerializeField] private Sprite icon;

    public UnitType UnitType => unitType;

    public int MaxHp => maxHp;
    public int AttackDamage => attackDamage;
    public int Defense => defense;
    public int Population => population;

    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public float Vision => vision;

    public int GoldCost => goldCost;
    public int WoodCost => woodCost;
    public float ProductTime => productTime;
    public BuildingType RequiredBuilding => requiredBuilding;

    public int GatherAmount => gatherAmount;

    public Sprite Icon => icon;
    public UnitStat(UnitType unitType, int maxHP, int attackDamage, int defense, int population, float moveSpeed, float attackRange, 
        float attackCooldown,float vision, int woodCost, int goldCost, float buildTime, BuildingType requiredBuilding, int gatherAmount, Sprite icon)
    {
        this.unitType = unitType;

        this.maxHp = maxHP;
        this.attackDamage = attackDamage;
        this.defense = defense;
        this.population = population;

        this.moveSpeed = moveSpeed;
        this.attackRange = attackRange;
        this.attackCooldown = attackCooldown;
        this.vision = vision;

        this.woodCost = woodCost;
        this.goldCost = goldCost;
        this.productTime = buildTime;
        this.requiredBuilding = requiredBuilding;

        this.gatherAmount = gatherAmount;
        this.icon = icon;
    }
    public UnitStat Clone()
    {
        return new UnitStat(unitType, maxHp, attackDamage, defense, population, moveSpeed, attackRange, attackCooldown, vision, woodCost, goldCost, productTime, requiredBuilding, gatherAmount, icon);
    }
}

[CreateAssetMenu(fileName = "UnitData", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    public List<UnitStat> unitList = new List<UnitStat>();
}