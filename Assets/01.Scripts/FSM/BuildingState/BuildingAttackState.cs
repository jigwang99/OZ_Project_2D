using UnityEngine;

public class BuildingAttackState : BuildingBaseState
{
    private Tower tower;
    private float remainCooldown;
    public BuildingAttackState(Building building) : base(building)
    {
        tower = building as Tower;
    }

    public override void Enter()
    {
        remainCooldown = 0f;
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
        if(remainCooldown > 0f)
            remainCooldown -= Time.fixedDeltaTime;
    }

    public override void Update()
    {
        IDamageable target = tower.GetTarget();

        if(target == null || !target.IsAlive || !tower.IsInRange(target))
        {
            tower.SetTarget(null);
            Building.StateMachine.ChangeState(Building.IdleState);
            return;
        }
        if(remainCooldown <= 0f)
        {
            tower.Fire(target);
            remainCooldown = Building.BuildingStat.AttackCooldown;
        }
    }
}
