using UnityEngine;

public class UnitBuild : MonoBehaviour
{
    private Unit unit;
    
    public Building TargetBuilding {  get; private set; }

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }
    private void OnEnable()
    {
        TargetBuilding = null;
    }
    public void SetTarget(Building building)
    {
        TargetBuilding = building;
    }
    public bool IsInRange()
    {
        if (TargetBuilding == null)
            return false;

        return RangeUtility.IsNear(unit.transform.position, TargetBuilding.transform, unit.UnitStat.AttackRange);
    }
    public bool HasValidTarget()
    {
        return TargetBuilding != null && TargetBuilding.IsAlive && TargetBuilding.IsConstruction;
    }
    public void Construct()
    {
        TargetBuilding.Construct(Time.deltaTime);
    }
}
