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
        IsDepleted = false;
    }

    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Wood", this.gameObject);
    }
}
