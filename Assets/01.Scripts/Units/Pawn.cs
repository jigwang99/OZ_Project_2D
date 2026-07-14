using UnityEngine;

public class Pawn : Unit
{
    public UnitGather Gather { get; private set; }
    public UnitGatherState GatherState { get;  private set; }
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<MeleeAttack>();
        Gather = GetComponent<UnitGather>();

        GatherState = new UnitGatherState(this);
    }
    protected void Start()
    {
        unitStat = UnitManager.instance.GetUnitStat(UnitType.Pawn);
    }
    public override void Init()
    {
        CurrentHp = unitStat.MaxHp;
        IsAlive = true;
        StateMachine.ChangeState(IdleState);
    }
    public override void ReturnToPool()
    {
        Gather.SetTargetResource(null);
        ObjectPoolManager.instance.ReturnObject("Pawn", this.gameObject);
    }
}
