using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnemyCommander : MonoBehaviour
{
    [SerializeField] private LayerMask resourceLayerMask;
    [SerializeField] private float thinkInterval = 1.0f;
    [SerializeField] private float resourceSearchRadius = 30f;

    private Faction faction;
    private float thinkTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        faction = FactionManager.instance.Enemy;
    }

    // Update is called once per frame
    void Update()
    {
        thinkTimer -= Time.deltaTime;
        if (thinkTimer > 0f)
            return;
        thinkTimer = thinkInterval;
        PawnToGather();
    }
    private void PawnToGather()
    {
        foreach(Unit unit in faction.Units)
        {
            if (!(unit is Pawn pawn) || !pawn.IsAlive)
                continue;

            if (pawn.StateMachine.CurrentState != pawn.IdleState)
                continue;

            Resource resource = FindNearestResource(pawn.transform.position);
            if (resource == null)
                continue;

            pawn.Gather.SetTargetResource(resource);
            pawn.Gather.SetReturnBuilding(Castle.FindNearestCastle(pawn.transform.position, FactionType.Enemy));
            pawn.StateMachine.ChangeState(pawn.GatherState);
        }
    }
    private Resource FindNearestResource(Vector2 from)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(from, resourceSearchRadius, resourceLayerMask);

        Resource nearest = null;
        float minDistance = float.MaxValue;
        foreach(Collider2D hit in hits)
        {
            Resource res = hit.GetComponent<Resource>();
            if(res == null || res.IsDepleted)
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
}
