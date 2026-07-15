using UnityEngine;

public class Pawn : Unit
{
    public override UnitType Type => UnitType.Pawn;
    public UnitGather Gather { get; private set; }
    public UnitBuild Build { get; private set; }
    public UnitGatherState GatherState { get;  private set; }
    public UnitBuildState BuildState { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<MeleeAttack>();
        Gather = GetComponent<UnitGather>();
        Build = GetComponent<UnitBuild>();

        GatherState = new UnitGatherState(this);
        BuildState = new UnitBuildState(this);
    }
    public override void ReturnToPool()
    {
        Gather.SetTargetResource(null);
        Build.SetTarget(null);
        ObjectPoolManager.instance.ReturnObject("Pawn", this.gameObject);
    }
}
