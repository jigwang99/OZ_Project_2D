using System.Collections.Generic;
using UnityEngine;

public class UnitDataLoader : DataLoader<UnitDataLoader, UnitType, UnitStat>
{
    [SerializeField] private UnitData unitData;

    protected override IReadOnlyList<UnitStat> StatList => unitData.unitList;
    protected override UnitType Key(UnitStat stat) => stat.UnitType;
    protected override UnitStat Clone(UnitStat stat) => stat.Clone();
}
