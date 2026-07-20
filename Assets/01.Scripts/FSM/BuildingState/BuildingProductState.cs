using UnityEngine;

public class BuildingProductState : BuildingBaseState
{
    private ProductionBuilding productionBuilding;
    private float timer;
    public float RemainTimer => timer;
    public BuildingProductState(Building building) : base(building)
    { 
        productionBuilding = building as ProductionBuilding;
    }
    public override void Enter()
    {
        timer = productionBuilding.CurrentProductTime;
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        if(!productionBuilding.HasList)
        {
            Building.StateMachine.ChangeState(Building.IdleState);
            return;
        }

        timer -= Time.deltaTime;
        if(timer <= 0f)
        {
            productionBuilding.CompleteProduction();

            if (productionBuilding.HasList)
                timer = productionBuilding.CurrentProductTime;
            else
                Building.StateMachine.ChangeState(Building.IdleState);
        }
    }
}
