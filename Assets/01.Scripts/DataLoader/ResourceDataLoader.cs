using System.Collections.Generic;
using UnityEngine;

public class ResourceDataLoader : DataLoader<ResourceDataLoader, ResourceType, ResourceStat>
{
    [SerializeField] private ResourceData resourceData;

    protected override IReadOnlyList<ResourceStat> StatList => resourceData.resourceList;
    protected override ResourceType Key(ResourceStat stat) => stat.ResourceType;
    protected override ResourceStat Clone(ResourceStat stat) => stat.Clone();
}
