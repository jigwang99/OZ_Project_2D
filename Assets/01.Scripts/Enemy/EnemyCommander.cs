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
public enum SquadOrder
{
    None,
    Defend,
    Attack,
    Regroup
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
    public bool whenPopulationFull;

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
    [SerializeField] private int populationMargin = 4;

    [Header("건설 규칙")]
    [SerializeField] private List<ConstructionRule> constructionRules = new List<ConstructionRule>();

    [Header("전투")]
    [SerializeField] private Transform rallyPoint;
    [SerializeField] private int attackSquadSize = 10;
    [SerializeField] private int regroupSquadSize = 3;
    [SerializeField] private float defenseRadius = 15f;
    [SerializeField] private float engageDistance = 10f;
    [SerializeField] private float rallyOffset = 6f;
    [SerializeField] private float squadSpacing = 1.1f;

    private List<Unit> squad = new List<Unit>();
    private IDamageable combatTarget;
    private Faction playerFaction;
    private bool hasOrder;

    private EnemyPhase currentPhase;
    private Faction faction;
    private float thinkTimer;

    private Building currentConstructing;
    private Pawn builder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        faction = FactionManager.instance.Enemy;
        playerFaction = FactionManager.instance.Player;
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
        HandleCombat();
    }
    private EnemyPhase UpdateEnemyPhase()
    {
        switch (currentPhase)
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
    #region gathering
    private void PawnToGather()
    {
        int pawnIndex = 0;

        foreach (IDamageable unit in faction.Units)
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
        switch (currentPhase)
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
        foreach (Collider2D hit in hits)
        {
            Resource res = hit.GetComponent<Resource>();
            if (res == null || res.IsDepleted)
                continue;
            if (res.Type != type)
                continue;

            float dist = Vector2.Distance(from, res.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = res;
            }
        }
        return nearest;
    }
    #endregion
    #region Production
    private void HandleProduction()
    {
        foreach (ProductionRule rule in productionRules.OrderBy(r => r.priority))
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
        foreach (Building building in faction.Buildings)
        {
            if (building is ProductionBuilding pb)
            {
                foreach (UnitType queued in pb.ProductList)
                    if (queued == type)
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

            if (productionBuilding.ProductList.Count < minQueue)
            {
                minQueue = productionBuilding.ProductList.Count;
                best = productionBuilding;
            }
        }
        return best;
    }
    #endregion
    #region Construction
    private void HandleConstruction()
    {
        if (currentConstructing != null)
        {
            if (currentConstructing.IsAlive && currentConstructing.IsConstruction)
            {
                AssignPawn(currentConstructing);
                return;
            }
            currentConstructing = null;
            builder = null;
        }

        foreach (ConstructionRule rule in constructionRules.OrderBy(r => r.priority))
        {
            if (!rule.phases.Contains(currentPhase))
                continue;
            if (faction.CountBuildings(rule.buildingType) >= rule.maxCount)
                continue;
            if (rule.whenPopulationFull && faction.MaxPopulation - faction.CurrentPopulation > populationMargin)
                continue;

            BuildingStat stat = BuildingDataLoader.instance.Get(rule.buildingType);

            if (stat == null)
                continue;

            if (faction.Wood < stat.WoodCost || faction.Gold < stat.GoldCost)
                continue;

            if (!TryFindPlacePosition(rule.size, out Vector2 position))
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
        if (building == null)
            return null;
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

        for (int i = 0; i < placeTryCount; i++)
        {
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float distance = UnityEngine.Random.Range(minBuildDistance, maxBuildDistance);

            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector2 candidate = GridManager.instance.FitNode(center + dir * distance);

            if (!CanPlaceAt(candidate, size))
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
        if (pawn.StateMachine.CurrentState != pawn.BuildState)
            return false;
        return pawn.Build.TargetBuilding == building;
    }
    private Pawn FindBuilder()
    {
        Pawn find = null;

        foreach (Unit unit in faction.Units)
        {
            if (!(unit is Pawn pawn) || !pawn.IsAlive)
                continue;
            if (pawn.StateMachine.CurrentState == pawn.IdleState)
                return pawn;
            if (find == null && pawn.StateMachine.CurrentState == pawn.GatherState)
                find = pawn;
        }
        return find;
    }
    #endregion
    #region Combat
    private SquadOrder currentOrder = SquadOrder.None;
    private IDamageable lastOrderTarget;
    private Vector2 orderDestination;

    private const float reorderDistance = 3f;
    private const float arriveDistance = 1.5f;

    private void HandleCombat()
    {
        CollectSquad();

        if (squad.Count == 0)
        {
            ClearOrder();
            return;
        }
        // 1순위 기지방어
        Unit intruder = FindIntruder();
        if (intruder != null)
        {
            combatTarget = intruder;
            IssueOrder(SquadOrder.Defend, intruder.transform.position, intruder);
            return;
        }

        // 2순위 병력이 모이고 combatPhase일 경우 공격
        if(currentPhase == EnemyPhase.Combat && squad.Count >= attackSquadSize)
        {
            if(!IsValidTarget(combatTarget))
                combatTarget = FindAttackTarget(GetSquadCenter());

            Vector2 destination = combatTarget != null ? (Vector2)combatTarget.transform.position : GetPlayerBasePosition();

            IssueOrder(SquadOrder.Attack, destination, combatTarget);
            return;
        }

        // 3순위 재집결
        combatTarget = null;

        if(squad.Count < regroupSquadSize)
        {
            ClearOrder();
            return;
        }

        IssueOrder(SquadOrder.Regroup, GetRallyPoint(), null);
    }
    private void ClearOrder()
    {
        combatTarget = null;
        lastOrderTarget = null;
        currentOrder = SquadOrder.None;
        hasOrder = false;
    }
    private void IssueOrder(SquadOrder order, Vector2 destination, IDamageable target)
    {
        bool issue = !hasOrder || order != currentOrder || !ReferenceEquals(target, lastOrderTarget) || Vector2.Distance(destination, orderDestination) > reorderDistance;

        currentOrder = order;
        lastOrderTarget = target;
        orderDestination = destination;
        hasOrder = true;

        CommandSquad(destination, target, issue);
    }
    private void CollectSquad()
    {
        squad.Clear();

        foreach(Unit unit in faction.Units)
        {
            if (unit == null || !unit.IsAlive || unit is Pawn)
                continue;
            if (!unit.gameObject.activeInHierarchy)
                continue;
            if (squad.Contains(unit))
                continue;
            squad.Add(unit);
        }
    }
    private bool IsValidTarget(IDamageable target)
    {
        if (target == null || !target.IsAlive)
            return false;

        GameObject targetObject = target.transform.gameObject;

        if (!targetObject.activeInHierarchy)
            return false;

        return targetObject.layer == (int)Layer.Player || targetObject.layer == (int)Layer.PlayerBuilding;
    }
    private Unit FindIntruder()
    {
        Unit nearest = null;
        float minDistance = defenseRadius;

        foreach(Unit unit in playerFaction.Units)
        {
            if (!IsValidTarget(unit))
                continue;

            float distance = DistanceToBase(unit.transform.position);
            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = unit;
            }
        }
        return nearest;
    }
    private float DistanceToBase(Vector2 position)
    {
        float minDistance = float.MaxValue;

        foreach(Building building in faction.Buildings)
        {
            if (building == null || !building.IsAlive)
                continue;

            float distance = Vector2.Distance(position, building.transform.position);
            if(distance < minDistance)
                minDistance = distance;
        }
        return minDistance;
    }
    private Vector2 GetSquadCenter()
    {
        if(squad.Count == 0)
            return GetCenter();

        Vector2 sum = Vector2.zero;
        foreach (Unit unit in squad)
            sum += (Vector2)unit.transform.position;
        return sum / squad.Count;
    }
    private IDamageable FindAttackTarget(Vector2 from)
    {
        // 교전거리 안의 유닛 
        Unit nearestUnit = null;
        float minUnitDistance = float.MaxValue;

        foreach(Unit unit in playerFaction.Units)
        {
            if (!IsValidTarget(unit))
                continue;

            float distance = Vector2.Distance(from, unit.transform.position);
            if(distance < minUnitDistance)
            {
                minUnitDistance = distance;
                nearestUnit = unit;
            }
        }
        if(nearestUnit != null && minUnitDistance <= engageDistance)
            return nearestUnit;

        // 적 건물
        Building nearestBuilding = null;
        float minBuildingDistance = float.MaxValue;

        foreach(Building building in playerFaction.Buildings)
        {
            if (!IsValidTarget(building))
                continue;

            float distance = Vector2.Distance(from, building.transform.position) - GetTargetBonus(building);
            if(distance < minBuildingDistance)
            {
                minBuildingDistance = distance;
                nearestBuilding = building;
            }    
        }
        if(nearestBuilding != null)
            return nearestBuilding;

        return nearestUnit;
    }
    private float GetTargetBonus(Building building)
    {
        if (building is Tower)
            return 12f;
        if (building is ProductionBuilding)
            return 8f;
        return 0f;
    }
    private Vector2 GetPlayerBasePosition()
    {
        Castle castle = Castle.FindNearestCastle(GetCenter(), FactionType.Player);
        if (IsValidTarget(castle))
            return castle.transform.position;

        foreach(Building building in playerFaction.Buildings)
            if(IsValidTarget(building))
                return building.transform.position;

        return GetCenter();
    }
    private Vector2 GetRallyPoint()
    {
        if (rallyPoint != null)
            return rallyPoint.position;

        Vector2 center = GetCenter();
        Vector2 dir = GetPlayerBasePosition() - center;

        return center + dir.normalized * rallyOffset;
    }
    private void CommandSquad(Vector2 destination, IDamageable target, bool issue)
    {
        for (int i = 0; i < squad.Count; i++)
        {
            CommandUnit(squad[i], destination + Formation.Offset(i, squad.Count, squadSpacing), target, issue);
        }

    }
    private void CommandUnit(Unit unit, Vector2 destination, IDamageable target, bool issue)
    {
        if(unit is Monk monk)
        {
            if (monk.StateMachine.CurrentState == monk.HealState)
                return;

            Unit sick = monk.Heal.FindTarget();
            if(sick != null && sick.IsAlive)
            {
                monk.Heal.SetTarget(sick);
                monk.StateMachine.ChangeState(monk.HealState);
                return;
            }
            MoveUnit(monk, destination, issue);
        }
        if(unit.Attack != null && IsValidTarget(target))
        {
            float distance = Vector2.Distance(unit.transform.position, target.transform.position);

            if(distance <= engageDistance)
            {
                if (!issue && ReferenceEquals(unit.Attack.GetTarget(), target) && IsEngaging(unit))
                    return;

                unit.Attack.SetTarget(target);
                unit.StateMachine.ChangeState(unit.ChaseState);
                return;
            }
        }

        if (!issue && unit.Attack != null && unit.Attack.GetTarget() != null && IsEngaging(unit))
            return;

        MoveUnit(unit, destination, issue);
    }
    private bool IsEngaging(Unit unit)
    {
        return unit.StateMachine.CurrentState == unit.ChaseState || unit.StateMachine.CurrentState == unit.AttackState;
    }
    private void MoveUnit(Unit unit, Vector2 destination, bool issue)
    {
        if (Vector2.Distance(unit.transform.position, destination) <= arriveDistance)
            return;
        if (!issue && unit.StateMachine.CurrentState == unit.MoveState && !unit.Movement.HasArrived)
            return;

        unit.Movement.SetDestination(destination);

        if(unit.StateMachine.CurrentState != unit.MoveState)
            unit.StateMachine.ChangeState(unit.MoveState);
    }
    #endregion
}
