using UnityEngine;

public class House : Building
{
    public override BuildingType Type => BuildingType.House;

    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("House", this.gameObject);
    }
}
