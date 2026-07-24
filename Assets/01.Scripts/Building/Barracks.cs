using UnityEngine;

public class Barracks : ProductionBuilding
{
    public override BuildingType Type => BuildingType.Barracks;
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Barracks", this.gameObject);
    }
}
