using System;
using System.Collections.Generic;
using UnityEngine;

public enum SquadOrder
{
    None,
    Defend,
    Attack,
    Regroup
}
[Serializable]
public class CombatSetting
{
    public Transform rallyPoint;
    public int attackSquadSize = 10;
    public int regroupSquadSize = 3;
    public float defenseRadius = 15f;
    public float engageDistance = 10f;
    public float rallyOffset = 6f;
    public float squadSpacing = 1.1f;
}
public class EnemyCombatModule : IEnemyModule
{
    private readonly EnemyContext enemyContext;
    private readonly CombatSetting setting;

    private List<Unit> squad = new List<Unit>();
    private IDamageable combatTarget;
    private bool hasOrder;

    private SquadOrder currentOrder = SquadOrder.None;
    private IDamageable lastOrderTarget;
    private Vector2 orderDestination;

    private const float reorderDistance = 3f;
    private const float arriveDistance = 1.5f;

    public EnemyCombatModule(EnemyContext enemyContext, CombatSetting setting)
    {
        this.enemyContext = enemyContext;
        this.setting = setting;
    }
    public void Update()
    {
        HandleCombat();
    }
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
        if (enemyContext.CurrentPhase == EnemyPhase.Combat && squad.Count >= setting.attackSquadSize)
        {
            if (!IsValidTarget(combatTarget))
                combatTarget = FindAttackTarget(GetSquadCenter());

            Vector2 destination = combatTarget != null ? (Vector2)combatTarget.transform.position : GetPlayerBasePosition();

            IssueOrder(SquadOrder.Attack, destination, combatTarget);
            return;
        }

        // 3순위 재집결
        combatTarget = null;

        if (squad.Count < setting.regroupSquadSize)
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

        foreach (Unit unit in enemyContext.Faction.Units)
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
        float minDistance = setting.defenseRadius;

        foreach (Unit unit in enemyContext.PlayerFaction.Units)
        {
            if (!IsValidTarget(unit))
                continue;

            float distance = DistanceToBase(unit.transform.position);
            if (distance < minDistance)
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

        foreach (Building building in enemyContext.Faction.Buildings)
        {
            if (building == null || !building.IsAlive)
                continue;

            float distance = Vector2.Distance(position, building.transform.position);
            if (distance < minDistance)
                minDistance = distance;
        }
        return minDistance;
    }
    private Vector2 GetSquadCenter()
    {
        if (squad.Count == 0)
            return enemyContext.GetCenter();

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

        foreach (Unit unit in enemyContext.PlayerFaction.Units)
        {
            if (!IsValidTarget(unit))
                continue;

            float distance = Vector2.Distance(from, unit.transform.position);
            if (distance < minUnitDistance)
            {
                minUnitDistance = distance;
                nearestUnit = unit;
            }
        }
        if (nearestUnit != null && minUnitDistance <= setting.engageDistance)
            return nearestUnit;

        // 적 건물
        Building nearestBuilding = null;
        float minBuildingDistance = float.MaxValue;

        foreach (Building building in enemyContext.PlayerFaction.Buildings)
        {
            if (!IsValidTarget(building))
                continue;

            float distance = Vector2.Distance(from, building.transform.position) - GetTargetBonus(building);
            if (distance < minBuildingDistance)
            {
                minBuildingDistance = distance;
                nearestBuilding = building;
            }
        }
        if (nearestBuilding != null)
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
        Castle castle = Castle.FindNearestCastle(enemyContext.GetCenter(), FactionType.Player);
        if (IsValidTarget(castle))
            return castle.transform.position;

        foreach (Building building in enemyContext.PlayerFaction.Buildings)
            if (IsValidTarget(building))
                return building.transform.position;

        return enemyContext.GetCenter();
    }
    private Vector2 GetRallyPoint()
    {
        if (setting.rallyPoint != null)
            return setting.rallyPoint.position;

        Vector2 center = enemyContext.GetCenter();
        Vector2 dir = GetPlayerBasePosition() - center;

        return center + dir.normalized * setting.rallyOffset;
    }
    private void CommandSquad(Vector2 destination, IDamageable target, bool issue)
    {
        for (int i = 0; i < squad.Count; i++)
        {
            CommandUnit(squad[i], destination + Formation.Offset(i, squad.Count, setting.squadSpacing), target, issue);
        }

    }
    private void CommandUnit(Unit unit, Vector2 destination, IDamageable target, bool issue)
    {
        if (unit is Monk monk)
        {
            if (monk.StateMachine.CurrentState == monk.HealState)
                return;

            Unit sick = monk.Heal.FindTarget();
            if (sick != null && sick.IsAlive)
            {
                monk.Heal.SetTarget(sick);
                monk.StateMachine.ChangeState(monk.HealState);
                return;
            }
            MoveUnit(monk, destination, issue);
        }
        if (unit.Attack != null && IsValidTarget(target))
        {
            float distance = Vector2.Distance(unit.transform.position, target.transform.position);

            if (distance <= setting.engageDistance)
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

        if (unit.StateMachine.CurrentState != unit.MoveState)
            unit.StateMachine.ChangeState(unit.MoveState);
    }
}
