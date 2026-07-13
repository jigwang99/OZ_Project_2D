using UnityEngine;

public class UnitGather : MonoBehaviour
{
    private Unit unit;
    public Resource TargetResource {  get; private set; }
    public Building ReturnBuilding { get; private set; }

    public ResourceType CarryResourceType { get; private set; }
    public int CarryAmount { get; private set; }

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }
    public void SetTargetResource(Resource targetResource)
    {
        this.TargetResource = targetResource;
    }
    public void SetReturnBuilding(Building returnBuilding)
    {
        this.ReturnBuilding = returnBuilding;
    }

    public void Gather()
    {
        if (TargetResource == null)
            return;
        int gatherd = TargetResource.Gathered(unit.UnitStat.GatherAmount);

        if (gatherd == 0)
            return;

        CarryResourceType = TargetResource.ResourceType;
        CarryAmount = gatherd;
    }
    public void ReturnResource()
    {
        if(CarryAmount <= 0)
            return;

        // 플레이어 자원 상승
        CarryAmount = 0;
    }
}
