using UnityEngine;

public class Pawn : Unit
{
    public override UnitType Type => UnitType.Pawn;
    public UnitGather Gather { get; private set; }
    public UnitGatherState GatherState { get;  private set; }
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<MeleeAttack>();
        Gather = GetComponent<UnitGather>();

        GatherState = new UnitGatherState(this);
    }
    public override void ReturnToPool()
    {
        Gather.SetTargetResource(null);
        ObjectPoolManager.instance.ReturnObject("Pawn", this.gameObject);
    }
}
