using System.Runtime.CompilerServices;
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
        if(TargetResource != null)
            TargetResource.OnDepleted -= HandleTargetDepleted;

        this.TargetResource = targetResource;
        
        if(TargetResource != null)
            TargetResource.OnDepleted += HandleTargetDepleted;
    }
    public void SetReturnBuilding(Building returnBuilding)
    {
        this.ReturnBuilding = returnBuilding;
    }

    public void Gather()
    {
        Resource resource = TargetResource;

        if (resource == null)
            return;

        ResourceType type = resource.Type;

        int gatherd = resource.Gathered(unit.UnitStat.GatherAmount);

        if (gatherd == 0)
            return;

        CarryResourceType = type;
        CarryAmount = gatherd;
    }
    public void ReturnResource()
    {
        if(CarryAmount <= 0)
            return;

        unit.OwnerFaction.AddResource(CarryResourceType, CarryAmount);
        CarryAmount = 0;
    }
    private void HandleTargetDepleted()
    {
        TargetResource = null;
    }
}
