using UnityEngine;

public class Gold : Resource
{
    public override ResourceType Type => ResourceType.Gold;

    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Gold", this.gameObject);
    }
}
