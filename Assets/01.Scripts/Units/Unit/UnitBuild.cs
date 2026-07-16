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
        
        Collider2D col = TargetBuilding.GetComponent<Collider2D>();
        Vector2 point = col != null ? col.ClosestPoint(unit.transform.position) : (Vector2)TargetBuilding.transform.position;

        return Vector2.Distance(unit.transform.position, point) <= unit.UnitStat.AttackRange;
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
