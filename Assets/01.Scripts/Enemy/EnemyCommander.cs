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
[Serializable]
public class ConstructionRule
{
    public BuildingType buildingType;
    public Vector2 size;
    public int maxCount;
    public int priority;
    public List<EnemyPhase> phases = new List<EnemyPhase>();
}
public class EnemyCommander : MonoBehaviour
{
    [Header("판단 주기")]
    [SerializeField] private float thinkInterval = 1.0f;

    [Header("페이즈 분리")]
    [SerializeField] private int buildCount = 200;
    [SerializeField] private int combatCount = 15;

    [Header("자원")]
    [SerializeField] private LayerMask resourceLayerMask;
    [SerializeField] private float resourceSearchRadius = 30f;

    
    [Header("생산 규칙")]
    [SerializeField] private List<ProductionRule> productionRules = new List<ProductionRule>();

    [Header("건설")]
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float minBuildDistance = 5f;
    [SerializeField] private float maxBuildDistance = 30f;
    [SerializeField] private int placeTryCount = 30;

    [Header("건설 규칙")]
    [SerializeField] private List<ConstructionRule> constructionRules = new List<ConstructionRule>();

    private EnemyPhase currentPhase;
    private Faction faction;
    private float thinkTimer;

    private Building currentConstructing;
    private Pawn builder;
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

        HandleConstruction();
        PawnToGather();
        HandleProduction();
        // 전투

        Debug.Log($"phase = {currentPhase}, wood = {faction.Wood}, gold = {faction.Gold}, pop = {faction.CurrentPopulation}, cons = {(currentConstructing != null ? currentConstructing.Type.ToString() : "none")}");
    }
    private EnemyPhase UpdateEnemyPhase()
    {
        switch(currentPhase)
        {
            case EnemyPhase.Gather:
                return faction.Wood >= buildCount ? EnemyPhase.Build : EnemyPhase.Gather;
            case EnemyPhase.Build:
                return CountCombatUnits() > combatCount ? EnemyPhase.Combat : EnemyPhase.Build;
            default:
                return currentPhase;
        }
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
        ProductionBuilding best = null;
        int minQueue = int.MaxValue;

        foreach (Building building in faction.Buildings)
        {
            if (!(building is ProductionBuilding productionBuilding) || !productionBuilding.IsAlive)
                continue;
            if (productionBuilding.IsQueueFull)
                continue;
            if (!productionBuilding.CanProduceType(type))
                continue;
            
            if(productionBuilding.ProductList.Count < minQueue)
            {
                minQueue = productionBuilding.ProductList.Count;
                best = productionBuilding;
            }
        }
        return best;
    }
    // 건물 건설
    private void HandleConstruction()
    {
        if(currentConstructing != null)
        {
            if(currentConstructing.IsAlive && currentConstructing.IsConstruction)
            {
                AssignPawn(currentConstructing);
                return;
            }
            currentConstructing = null;
            builder = null;
        }

        foreach(ConstructionRule rule in constructionRules.OrderBy(r => r.priority))
        {
            if (!rule.phases.Contains(currentPhase))
                continue;
            if (faction.CountBuildings(rule.buildingType) >= rule.maxCount)
                continue;

            BuildingStat stat = BuildingDataLoader.instance.GetBuildingStat(rule.buildingType);

            if (stat == null)
                continue;

            if (faction.Wood < stat.WoodCost || faction.Gold < stat.GoldCost)
                continue;
            
            if(!TryFindPlacePosition(rule.size, out Vector2 position))
                continue;

            Building building = PlaceBuilding(rule.buildingType, position, stat);
            if (building == null)
                continue;

            currentConstructing = building;
            AssignPawn(building);
            break;
        }
    }
    private Building PlaceBuilding(BuildingType type, Vector2 position, BuildingStat stat)
    {
        if (!faction.TryReduceResource(stat.WoodCost, stat.GoldCost))
            return null;

        Building building = ObjectPoolManager.instance.GetObject<Building>(type.ToString());
        if(building == null)
        {
            // 새거 만들기
        }

        building.transform.position = position;
        building.SetLayer(Layer.Enemy);
        building.SetSkipBuilded(false);
        building.Init();
        return building;
    }
    private bool TryFindPlacePosition(Vector2 size, out Vector2 position)
    {
        position = Vector2.zero;
        Vector2 center = GetCenter();

        for(int i = 0; i < placeTryCount; i++)
        {
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float distance = UnityEngine.Random.Range(minBuildDistance, maxBuildDistance);

            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector2 candidate = GridManager.instance.FitNode(center + dir * distance);

            if(!CanPlaceAt(candidate, size))
                continue;

            position = candidate;
            return true;
        }
        return false;
    }
    private Vector2 GetCenter()
    {
        Castle castle = Castle.FindNearestCastle(transform.position, FactionType.Enemy);
        return castle != null ? (Vector2)castle.transform.position : (Vector2)transform.position;
    }
    private bool CanPlaceAt(Vector2 center, Vector2 size)
    {
        if (Physics2D.OverlapBox(center, size, 0f, obstacleLayerMask) != null)
            return false;
        return GridManager.instance.IsAreaWalkable(center, size);
    }
    private void AssignPawn(Building building)
    {
        if (IsValidBuilder(builder, building))
            return;

        builder = FindBuilder();
        if (builder == null)
            return;

        builder.Build.SetTarget(building);
        builder.StateMachine.ChangeState(builder.BuildState);
        
    }
    private bool IsValidBuilder(Pawn pawn, Building building)
    {
        if (pawn == null || !pawn.IsAlive)
            return false;
        if (!pawn.gameObject.activeInHierarchy)
            return false;
        if(pawn.StateMachine.CurrentState != pawn.BuildState)
            return false;
        return pawn.Build.TargetBuilding == building;
    }
    private Pawn FindBuilder()
    {
        Pawn find = null;

        foreach(Unit unit in faction.Units)
        {
            if (!(unit is Pawn pawn) || !pawn.IsAlive)
                continue;
            if (pawn.StateMachine.CurrentState == pawn.IdleState)
                return pawn;
            if(find == null && pawn.StateMachine.CurrentState == pawn.GatherState)
                find = pawn;
        }
        return find;
    }
    // 전투

}
