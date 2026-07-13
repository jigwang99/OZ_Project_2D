using UnityEngine;

public class Wood : Resource
{
    private void Start()
    {
        resourceStat = ResourceManager.instance.GetResourceStat(ResourceType.Wood);
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
