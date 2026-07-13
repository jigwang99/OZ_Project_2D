public class UnitMoveState : UnitBaseState
{
    public UnitMoveState(Unit unit) : base(unit) { }

    public override void Enter()
    {
        Unit.Movement.SetMoveSpeed(Unit.UnitStat.MoveSpeed);
    }

    public override void Update()
    {
        // 애니메이션
    }

    public override void FixedUpdate()
    {
        Unit.Movement.Move();

        if (Unit.Movement.HasArrived)
        {
            Unit.StateMachine.ChangeState(Unit.IdleState);
        }
    }

    public override void Exit()
    {
        Unit.Movement.Stop();
    }
}