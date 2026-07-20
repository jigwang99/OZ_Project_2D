using UnityEngine;

public class UnitAttackState : UnitBaseState
{
    public UnitAttackState(Unit unit) : base(unit)
    {
    }

    public override void Enter()
    {
    }
    public override void Exit()
    {
    }
    public override void FixedUpdate()
    {

    }
    public override void Update()
    {
        IDamageable target = Unit.Attack.GetTarget();

        if(target == null || !target.IsAlive)
        {
            Unit.Attack.SetTarget(null);
            Unit.StateMachine.ChangeState(Unit.IdleState);
            return;
        }
        if(!Unit.Attack.IsInRange())
        {
            Unit.StateMachine.ChangeState(Unit.ChaseState);
            return;
        }
        if (Unit.Attack.CanAttack())
        {
            Unit.FlipSprite(target.transform.position.x - Unit.transform.position.x);
            Unit.PlayAttackAnimation();
            Unit.Attack.Attack();
        }
    }
}
