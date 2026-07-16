using UnityEngine;

public class BuildingBuildedState : BuildingBaseState
{
    private float timer;

    public BuildingBuildedState(Building building) : base(building)
    {
    }

    public override void Enter()
    {
        Building.ResetProgress();
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        if (Building.BuildProgress >= Building.BuildingStat.BuildTime)
        {
            Debug.Log("BuildFinish");
            Building.StateMachine.ChangeState(Building.IdleState);
        }
    }
}
