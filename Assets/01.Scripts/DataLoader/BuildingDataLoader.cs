using UnityEngine;
using System.Collections.Generic;
public class BuildingDataLoader : DataLoader<BuildingDataLoader, BuildingType, BuildingStat>
{

    [SerializeField] private BuildingData buildingData;

    protected override IReadOnlyList<BuildingStat> StatList => buildingData.buildingList;
    protected override BuildingType Key(BuildingStat stat) => stat.BuildingType;
    protected override BuildingStat Clone(BuildingStat stat) => stat.Clone();
}
