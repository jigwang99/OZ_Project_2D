using System;
using UnityEngine;

[Serializable]
public class GatherSetting
{
    public LayerMask resourceLayerMask;
    public float resourceSearchRadius = 30f;
}
public class EnemyGatherModule : IEnemyModule
{
    private readonly EnemyContext enemyContext;
    private readonly GatherSetting setting;

    public EnemyGatherModule(EnemyContext enemyContext, GatherSetting setting)
    {
        this.enemyContext = enemyContext;
        this.setting = setting;
    }
    public void Update()
    {
        PawnToGather();
    }
    private void PawnToGather()
    {
        int pawnIndex = 0;

        foreach (IDamageable unit in enemyContext.Faction.Units)
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
        switch (enemyContext.CurrentPhase)
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(from, setting.resourceSearchRadius, setting.resourceLayerMask);

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
}
