using UnityEngine;
using System.Collections.Generic;
public class ProjectileDataLoader : DataLoader<ProjectileDataLoader, ProjectileType, ProjectileStat>
{
    [SerializeField] private ProjectileData projectileData;

    protected override IReadOnlyList<ProjectileStat> StatList => projectileData.projectileList;
    protected override ProjectileType Key(ProjectileStat stat) => stat.ProjectileType;
    protected override ProjectileStat Clone(ProjectileStat stat) => stat.Clone();
}
