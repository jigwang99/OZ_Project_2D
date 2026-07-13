using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Pawn,
    Warrior,
    Archer,
    Lancer,
    Monk,
}
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
    [SerializeField] private float buildTime;

    [Header("Gather")]
    [SerializeField] private int gatherAmount;
    [SerializeField] private float gatherInterval;

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
    public float BuildTime => buildTime;

    public int GatherAmount => gatherAmount;
    public float GatherInterval => gatherInterval;

    public UnitStat(UnitType unitType, int maxHP, int attackDamage, int defense, int population, float moveSpeed, float attackRange, 
        float attackCooldown,float vision, int woodCost, int goldCost, float buildTime, int gatherAmount, float gatherInterval)
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
        this.buildTime = buildTime;

        this.gatherAmount = gatherAmount;
        this.gatherInterval = gatherInterval;
    }
    public UnitStat Clone()
    {
        return new UnitStat(unitType, maxHp, attackDamage, defense, population, moveSpeed, attackRange, attackCooldown, vision, woodCost, goldCost, buildTime, gatherAmount, gatherInterval);
    }
}

[CreateAssetMenu(fileName = "UnitData", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    public List<UnitStat> unitList = new List<UnitStat>();
}