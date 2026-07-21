using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum EnemyPhase
{
    Gather,
    Build,
    Combat,
}
[Serializable]
public class ProductionRule
{
    public UnitType unitType;
    public int maxCount;
    public int priority;
    public List<EnemyPhase> phases = new List<EnemyPhase>();
}
public class EnemyCommander : MonoBehaviour
{
    [Header("판단 주기")]
    [SerializeField] private float thinkInterval = 1.0f;

    [Header("자원")]
    [SerializeField] private LayerMask resourceLayerMask;
    [SerializeField] private float resourceSearchRadius = 30f;

    [Header("생산")]


    [Header("페이즈 분리")]
    [SerializeField] private int buildCount = 200;
    [SerializeField] private int combatCount = 15;

    [Header("생산 규칙")]
    [SerializeField] private List<ProductionRule> productionRules = new List<ProductionRule>();

    private EnemyPhase currentPhase;
    private Faction faction;
    private float thinkTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        faction = FactionManager.instance.Enemy;
        currentPhase = EnemyPhase.Gather;
    }

    // Update is called once per frame
    void Update()
    {
        thinkTimer -= Time.deltaTime;
        if (thinkTimer > 0f)
            return;
        thinkTimer = thinkInterval;

        currentPhase = UpdateEnemyPhase();

        PawnToGather();
        HandleProduction();
        // 전투
    }
    private EnemyPhase UpdateEnemyPhase()
    {
        int combatUnits = CountCombatUnits();

        if (combatUnits > combatCount)
            return EnemyPhase.Combat;

        if (faction.Wood >= buildCount)
            return EnemyPhase.Build;

        return EnemyPhase.Gather;
    }
    private int CountCombatUnits()
    {
        return faction.CountUnits(UnitType.Lancer) + faction.CountUnits(UnitType.Archer) + faction.CountUnits(UnitType.Monk) + faction.CountUnits(UnitType.Warrior);
    }
    //자원 채집
    private void PawnToGather()
    {
        int pawnIndex = 0;

        foreach(Unit unit in faction.Units)
        {
            if (!(unit is Pawn pawn) || !pawn.IsAlive)
                continue;

            if (pawn.StateMachine.CurrentState != pawn.IdleState)
            {
                pawnIndex++;
                continue;
            }

            ResourceType wantType = DecideResourceType(pawnIndex);
            pawnIndex++;

            Resource resource = FindNearestResource(pawn.transform.position, wantType);
            if (resource == null)
                continue;

            pawn.Gather.SetTargetResource(resource);
            pawn.Gather.SetReturnBuilding(Castle.FindNearestCastle(pawn.transform.position, FactionType.Enemy));
            pawn.StateMachine.ChangeState(pawn.GatherState);
        }
    }
    private ResourceType DecideResourceType(int pawnIndex)
    {
        switch(currentPhase)
        {
            case EnemyPhase.Gather:
                return ResourceType.Wood;
            case EnemyPhase.Build:
                return (pawnIndex % 4 == 3) ? ResourceType.Gold : ResourceType.Wood;
            case EnemyPhase.Combat:
                return (pawnIndex % 2 == 1) ? ResourceType.Gold : ResourceType.Wood;
            default: 
                return ResourceType.Wood;
        }
    }
    private Resource FindNearestResource(Vector2 from, ResourceType type)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(from, resourceSearchRadius, resourceLayerMask);

        Resource nearest = null;
        float minDistance = float.MaxValue;
        foreach(Collider2D hit in hits)
        {
            Resource res = hit.GetComponent<Resource>();
            if(res == null || res.IsDepleted)
                continue;
            if(res.Type != type)
                continue;

            float dist = Vector2.Distance(from, res.transform.position);
            if(dist < minDistance)
            {
                minDistance = dist;
                nearest = res;
            }
        }
        return nearest;
    }
    // 생산
    private void HandleProduction()
    {
        foreach(ProductionRule rule in productionRules.OrderBy(r => r.priority))
        {
            if (!rule.phases.Contains(currentPhase))
                continue;
            
            int current = faction.CountUnits(rule.unitType) + QueueCount(rule.unitType);
            if (current >= rule.maxCount)
                continue;

            ProductionBuilding building = FindProducer(rule.unitType);
            if (building == null)
                continue;

            building.EnqueueUnit(rule.unitType);
            break;
        }
    }
    private int QueueCount(UnitType type)  //생산큐에있는 유닛 수
    {
        int count = 0;
        foreach(Building building in faction.Buildings)
        {
            if(building is ProductionBuilding pb)
            {
                foreach(UnitType queued in pb.ProductList)
                    if(queued == type)
                        count++;
            }
        }
        return count;
    }
    private ProductionBuilding FindProducer(UnitType type)
    {
        foreach (Building building in faction.Buildings)
        {
            if (!(building is ProductionBuilding productionBuilding) || !productionBuilding.IsAlive)
                continue;
            if (productionBuilding.IsQueueFull)
                continue;
            if (!productionBuilding.CanProduceType(type))
                continue;
            return productionBuilding;
        }
        return null;
    }
    // 전투

}
