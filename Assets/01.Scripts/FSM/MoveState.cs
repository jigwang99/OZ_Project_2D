using UnityEngine;

public class MoveState : BaseState
{
    public MoveState(Unit unit) : base(unit)
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
        Unit.Movement.Move();
        if(Vector2.Distance(Unit.transform.position, Unit.Movement.GetDestination()) < 0.5f)
        {
            Unit.StateMachine.ChangeState(Unit.IdleState);
        }
    }
    public override void Update()
    {
        
    }
}
