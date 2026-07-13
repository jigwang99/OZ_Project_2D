using UnityEngine;

public class Gold : Resource
{
    private void Start()
    {
        resourceStat = ResourceManager.instance.GetResourceStat(ResourceType.Gold);
    }
    public override void Init()
    {
        remainAmount = resourceStat.MaxAmount;
    }

    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Gold", this.gameObject);
    }
}
