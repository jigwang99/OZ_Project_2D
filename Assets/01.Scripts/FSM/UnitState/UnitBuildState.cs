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
        Unit.SetRunAnimation(false);
        pawn.StopInteractAnimation();
        build.SetTarget(null);
    }

    public override void FixedUpdate()
    {
        if (build.TargetBuilding == null || build.IsInRange())
        {
            Unit.Movement.Stop();
            return;
        } 
        refindTimer -= Time.fixedDeltaTime;
        if(refindTimer <=0)
        {
            Unit.Movement.SetDestinationNear(build.TargetBuilding.transform);
            refindTimer = refindInterval;
        }
        Unit.Movement.Move();
    }

    public override void Update()
    {
        if (!build.HasValidTarget())
        {
            Unit.StateMachine.ChangeState(Unit.IdleState);
            return;
        }
        
        bool inRange = build.IsInRange();
        Unit.SetRunAnimation(!inRange);

        if (inRange)
        {
            pawn.SetInteractAnimation(PawnTool.Hammer);
            build.Construct();
        }
        else
            pawn.StopInteractAnimation();

        // test 
        if (build.TargetBuilding != null && Unit.Movement.HasArrived)
        {
            Collider2D col = build.TargetBuilding.GetComponent<Collider2D>();
            float dist = Vector2.Distance(Unit.transform.position, col.ClosestPoint(Unit.transform.position));

            Debug.Log($"{dist:F2}, range : {Unit.UnitStat.AttackRange}");
        }
    }
}
