using Unity.Collections;
using UnityEngine;

public class UnitGatherState : UnitBaseState
{
    public enum Phase
    {
        MoveToResource,
        Gathering,
        MoveToBuilding,
        Returning,
    }

    private Pawn pawn;
    private UnitGather gather;
    private Phase phase;
    private float timer;

    private const float range = 0.5f;
    private const float gatherDelay = 2f;
    private const float returnDelay = 1f;

    public UnitGatherState(Unit unit) : base(unit)
    {
        pawn = unit as Pawn;
        gather = pawn.Gather;
    }
    public override void Enter()
    {
        Unit.Movement.SetMoveSpeed(Unit.UnitStat.MoveSpeed);
        MoveToResource();
    }

    public override void Exit()
    {
        Unit.Movement.Stop();
        Unit.SetRunAnimation(false);
        pawn.StopInteractAnimation();
    }

    public override void FixedUpdate()
    {
        switch(phase)
        {
            case Phase.MoveToResource:
                Unit.Movement.Move();
                CheckArrivedAtResource();
                break;
            case Phase.MoveToBuilding:
                Unit.Movement.Move();
                CheckArrivedAtBuilding();
                break;
        }
    }

    public override void Update()
    {
        switch(phase)
        {
            case Phase.Gathering:
                Gathering();
                break;
            case Phase.Returning:
                Returning();
                break;
        }
    }
    public bool IsNear(Transform target)
    {
        Collider2D col = target.GetComponent<Collider2D>();
        Vector2 point = col != null ? col.ClosestPoint(Unit.transform.position) : (Vector2)target.position;

        return Vector2.Distance(Unit.transform.position, point) <= range;
    }

    private void MoveToResource()
    {
        if (gather.TargetResource == null || gather.TargetResource.IsDepleted)
            return;

        phase = Phase.MoveToResource;
        pawn.StopInteractAnimation();
        Unit.SetRunAnimation(true);
        Unit.Movement.SetDestinationNear(gather.TargetResource.transform);
    }
    private void CheckArrivedAtResource()
    {
        if (gather.TargetResource == null || gather.TargetResource.IsDepleted)
        {
            Unit.StateMachine.ChangeState(Unit.IdleState);
            return;
        }
        if(Unit.Movement.HasArrived || IsNear(gather.TargetResource.transform))
        {
            Unit.Movement.Stop();

            Unit.SetRunAnimation(false);
            PawnTool tool = gather.TargetResource.Type == ResourceType.Wood ? PawnTool.Axe : PawnTool.Pickaxe;
            pawn.SetInteractAnimation(tool);

            phase = Phase.Gathering;
            timer = gatherDelay;
        }
    }
    private void MoveToBuilding()
    {
        Castle castle = Castle.FindNearestCastle(Unit.transform.position, pawn.OwnerFaction.Type);
        gather.SetReturnBuilding(castle);

        if (castle == null)
        {
            Unit.StateMachine.ChangeState(Unit.IdleState);
            return;
        }

        pawn.StopInteractAnimation();
        pawn.SetCarryAnimation(gather.CarryResourceType, true);
        Unit.SetRunAnimation(true);

        phase = Phase.MoveToBuilding;
        Unit.Movement.SetDestinationNear(gather.ReturnBuilding.transform);
    }
    private void CheckArrivedAtBuilding()
    {
        if(Unit.Movement.HasArrived || IsNear(gather.ReturnBuilding.transform))
        {
            Unit.SetRunAnimation(false);
            phase = Phase.Returning;
            timer = returnDelay;
        }
    }
    private void Gathering()
    {
        if(gather.TargetResource == null || gather.TargetResource.IsDepleted)
        {
            Unit.StateMachine.ChangeState(Unit.IdleState);
            return;
        }

        // 
        timer -= Time.deltaTime;
        if (timer > 0f)
            return;

        gather.Gather();
        MoveToBuilding();
    }
    private void Returning()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        gather.ReturnResource();
        pawn.ClearCarryAnimation();

        if (gather.TargetResource != null && !gather.TargetResource.IsDepleted)
            MoveToResource();
        else
            Unit.StateMachine.ChangeState(Unit.IdleState);
    }
}
