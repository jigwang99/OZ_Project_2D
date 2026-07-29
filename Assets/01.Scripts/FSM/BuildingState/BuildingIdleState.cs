public class BuildingIdleState : BuildingBaseState
{
    public BuildingIdleState(Building building) : base(building)
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
        if(Building is Tower tower)
        {
            IDamageable target = tower.FindTarget();
            if(target != null && target.IsAlive)
            {
                tower.SetTarget(target);
                Building.StateMachine.ChangeState(tower.AttackState);
            }
            return;
        }
        if (Building is ProductionBuilding productionBuilding && productionBuilding.HasList)
            Building.StateMachine.ChangeState(productionBuilding.ProductState);
    }
}
