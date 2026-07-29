using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
[Serializable]
public class ConstructionSetting
{
    public LayerMask obstacleLayerMask;
    public float minBuildDistance = 5f;
    public float maxBuildDistance = 30f;
    public int placeTryCount = 30;
    public int populationMargin = 4;
    public List<ConstructionRule> rules = new List<ConstructionRule>();
}
public class EnemyContructionModule
{
    private readonly EnemyContext enemyContext;
    private readonly ConstructionSetting setting;

    private Building currentConstructing;
    private Pawn builder;

    public EnemyContructionModule(EnemyContext enemyContext, ConstructionSetting setting)
    {
        this.enemyContext = enemyContext;
        this.setting = setting;
    }
    public void Update()
    {
        HandleConstruction();
    }
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

        foreach (ConstructionRule rule in setting.rules.OrderBy(r => r.priority))
        {
            if (!rule.phases.Contains(enemyContext.CurrentPhase))
                continue;
            if (enemyContext.Faction.CountBuildings(rule.buildingType) >= rule.maxCount)
                continue;
            if (rule.whenPopulationFull && enemyContext.Faction.MaxPopulation - enemyContext.Faction.CurrentPopulation > setting.populationMargin)
                continue;

            BuildingStat stat = BuildingDataLoader.instance.Get(rule.buildingType);

            if (stat == null)
                continue;

            if (enemyContext.Faction.Wood < stat.WoodCost || enemyContext.Faction.Gold < stat.GoldCost)
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
        if (!enemyContext.Faction.TryReduceResource(stat.WoodCost, stat.GoldCost))
            return null;

        Building building = ObjectPoolManager.instance.GetObject<Building>(type);
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
        Vector2 center = enemyContext.GetCenter();

        for (int i = 0; i < setting.placeTryCount; i++)
        {
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float distance = UnityEngine.Random.Range(setting.minBuildDistance, setting.maxBuildDistance);

            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector2 candidate = GridManager.instance.FitNode(center + dir * distance);

            if (!CanPlaceAt(candidate, size))
                continue;

            position = candidate;
            return true;
        }
        return false;
    }
    private bool CanPlaceAt(Vector2 center, Vector2 size)
    {
        if (!MapBounds.Contains(center, size))
            return false;
        if (Physics2D.OverlapBox(center, size, 0f, setting.obstacleLayerMask) != null)
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

        foreach (Unit unit in enemyContext.Faction.Units)
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
}
