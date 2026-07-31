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
        // Monk 힐 탐지
        if(Unit is Monk monk)
        {
            Unit sick = monk.Heal.FindTarget();

            if(sick != null && sick.IsAlive)
            {
                monk.Heal.SetTarget(sick);
                monk.StateMachine.ChangeState(monk.HealState);
            }
            return;
        }
        // 대기 상태시 적 탐지
        if(!(Unit is Pawn))
        {
            IDamageable target = Unit.Attack.FindTarget();

            if(target != null && target.IsAlive)
            {
                Unit.Attack.SetTarget(target);
                Unit.StateMachine.ChangeState(Unit.AttackState);
            }    
        }
    }
}
