using UnityEngine;

public class Archery : Building
{
    public override BuildingType Type => BuildingType.Archery;

    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Archery", this.gameObject);
    }
}
