using UnityEngine;

public class Wood : Resource
{
    public override ResourceType Type => ResourceType.Wood;

    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Wood", this.gameObject);
    }
}
