using UnityEngine;

public class UnitGatherState : UnitBaseState
{
    private Pawn pawn;
    public UnitGatherState(Unit unit) : base(unit)
    {
        pawn  = unit as Pawn;
    }
    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        Unit.Movement.Stop();
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        
    }
    private void MoveToResource()
    {
        if (pawn.Gather.TargetResource == null || pawn.Gather.TargetResource.IsDepleted)
            return;

        Unit.Movement.SetDestination(pawn.Gather.TargetResource.transform.position);
    }
}
