using UnityEngine;

public class UnitIdleState : UnitBaseState
{
    public UnitIdleState(Unit unit) : base(unit)
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
        // 대기 상태시 적 탐지
        if(!(Unit is Pawn))
        {
            Unit target = Unit.Attack.FindTarget();

            if(target != null && target.IsAlive)
            {
                Unit.Attack.SetTarget(target);
                Unit.StateMachine.ChangeState(Unit.AttackState);
            }    
        }
    }
}
