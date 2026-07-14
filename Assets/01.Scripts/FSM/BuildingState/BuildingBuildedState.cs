using UnityEngine;

public class BuildingBuildedState : BuildingBaseState
{
    private float timer;

    public BuildingBuildedState(Building building) : base(building)
    {
    }

    public override void Enter()
    {
        timer = Building.BuildingStat.BuildTime;
        // 건설중
    }

    public override void Exit()
    {
        // 완성
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Building.StateMachine.ChangeState(Building.IdleState);
    }
}
