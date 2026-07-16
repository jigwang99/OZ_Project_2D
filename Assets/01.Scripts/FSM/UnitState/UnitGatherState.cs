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

    private const float range = 1.5f;
    private const float Delay = 0.3f;
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
            phase = Phase.Gathering;
            timer = Delay;
        }
    }
    private void MoveToBuilding()
    {
        Castle castle = Castle.FindNearestCastle(Unit.transform.position);
        gather.SetReturnBuilding(castle);

        if (castle == null)
        {
            Unit.StateMachine.ChangeState(Unit.IdleState);
            return;
        }
        phase = Phase.MoveToBuilding;
        Unit.Movement.SetDestinationNear(gather.ReturnBuilding.transform);
    }
    private void CheckArrivedAtBuilding()
    {
        if(Unit.Movement.HasArrived || IsNear(gather.ReturnBuilding.transform))
        {
            phase = Phase.Returning;
            timer = Delay;
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

        if (gather.TargetResource != null && !gather.TargetResource.IsDepleted)
            MoveToResource();
        else
            Unit.StateMachine.ChangeState(Unit.IdleState);
    }
}
