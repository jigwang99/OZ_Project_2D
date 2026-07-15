using UnityEngine;

public class UnitBuildState : UnitBaseState
{
    private Pawn pawn;
    private UnitBuild build;
    private float refindTimer;
    private const float refindInterval = 0.25f;

    public UnitBuildState(Unit unit) : base(unit)
    {
        pawn = unit as Pawn;
        build = pawn.Build;
    }
    public override void Enter()
    {
        Unit.Movement.SetMoveSpeed(Unit.UnitStat.MoveSpeed);
        refindTimer = 0;
    }

    public override void Exit()
    {
        Unit.Movement.Stop();
        build.SetTarget(null);
    }

    public override void FixedUpdate()
    {
        if (build.TargetBuilding == null || build.IsInRange())
            return;
        refindTimer -= Time.fixedDeltaTime;
        if(refindTimer <=0)
        {
            Unit.Movement.SetDestination(build.TargetBuilding.transform.position);
            refindTimer = refindInterval;
        }
        Unit.Movement.Move();
    }

    public override void Update()
    {
        if(!build.HasValidTarget())
        {
            Unit.StateMachine.ChangeState(Unit.IdleState);
            return;
        }
        if (build.IsInRange())
            build.Construct();
    }
}
