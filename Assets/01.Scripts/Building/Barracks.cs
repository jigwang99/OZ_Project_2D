using UnityEngine;
using UnityEngine.InputSystem;

public class Barracks : ProductionBuilding
{
    private void Start()
    {
        buildingStat = BuildingManager.instance.GetBuildingStat(BuildingType.Barracks);
    }
    public override void Init()
    {
        CurrentHp = buildingStat.MaxHp;
        IsAlive = true;
        StateMachine.ChangeState(BuildedState);
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Barracks", this.gameObject);
    }
}
