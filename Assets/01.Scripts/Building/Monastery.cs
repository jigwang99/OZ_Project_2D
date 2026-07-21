using UnityEngine;

public class Monastery : Building
{
    public override BuildingType Type => BuildingType.Monastery;

    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Monastery", this.gameObject);
    }
}
