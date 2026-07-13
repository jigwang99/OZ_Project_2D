using UnityEngine;

public class ChaseState : BaseState
{
    private float scout;
    private const float repathInterval = 0.25f;

    public ChaseState(Unit unit) : base(unit)
    {
    }

    public override void Enter()
    {
        Unit.Movement.SetMoveSpeed(Unit.UnitStat.MoveSpeed);
        scout = 0f;
    }

    public override void Exit()
    {
        Unit.Movement.Stop();
    }

    public override void FixedUpdate()
    {
        Unit target = Unit.Attack.GetTarget();
        if (target == null)
            return;

        scout -= Time.fixedDeltaTime;
        if (scout <= 0f)
        {
            Unit.Movement.SetDestination(target.transform.position);
            scout = repathInterval;
        }
        Unit.Movement.Move();
    }

    public override void Update()
    {
        Unit target = Unit.Attack.GetTarget();

        if(target == null || !target.IsAlive)
        {
            Unit.Attack.SetTarget(null);
            Unit.StateMachine.ChangeState(Unit.IdleState);
            return;
        }

        if (Unit.Attack.IsInRange())
            Unit.StateMachine.ChangeState(Unit.AttackState);
    }
}
