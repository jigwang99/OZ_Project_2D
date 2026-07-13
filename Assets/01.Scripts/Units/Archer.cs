using UnityEngine;

public class Archer : Unit
{
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<RangedAttack>();
    }
    protected void Start()
    {
        unitStat = UnitManager.instance.GetUnitStat(UnitType.Archer);
    }
    public override void Init()
    {
        CurrentHp = unitStat.MaxHp;
        IsAlive = true;
        StateMachine.ChangeState(IdleState);
    }
    public override void ReturnToPool()
    {
        
        ObjectPoolManager.instance.ReturnObject("Archer", this.gameObject);
    }
}
