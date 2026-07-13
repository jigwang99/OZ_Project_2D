using UnityEngine;
using System.Collections.Generic;

public enum ResourceType
{
    Wood,
    Gold,
}
public class ResourceStat
{
    [Header("Info")]
    [SerializeField] private ResourceType resourceType;

    [Header("Stat")]
    [SerializeField] private int maxAmount;

    public ResourceType ResourceType => resourceType;
    public int MaxAmount => maxAmount;

    public ResourceStat(ResourceType resourceType, int maxAmount)
    {
        this.resourceType = resourceType;
        this.maxAmount = maxAmount;
    }
    public ResourceStat Clone()
    {
        return new ResourceStat(resourceType, maxAmount);
    }
}

[CreateAssetMenu(fileName = "ResourceData", menuName = "RTS/Resource Data")]
public class ResourceData : ScriptableObject
{
    public List<ResourceStat> resourceList = new List<ResourceStat>();
}
