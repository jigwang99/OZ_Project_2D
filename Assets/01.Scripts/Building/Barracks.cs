using UnityEngine;
using UnityEngine.InputSystem;

public class Barracks : ProductionBuilding
{
    public override BuildingType Type => BuildingType.Barracks;
    public override void Init()
    {
        if(buildingStat == null)
            buildingStat = BuildingDataLoader.instance.GetBuildingStat(BuildingType.Barracks);
        CurrentHp = buildingStat.MaxHp;
        IsAlive = true;
        StateMachine.ChangeState(BuildedState);
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Barracks", this.gameObject);
    }
}
