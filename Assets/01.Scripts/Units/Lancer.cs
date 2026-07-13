using UnityEngine;

public class Lancer : Unit
{
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<MeleeAttack>();
    }
    protected void Start()
    {
        unitStat = UnitManager.instance.GetUnitStat(UnitType.Lancer);
    }
    public override void Init()
    {
        CurrentHp = unitStat.MaxHp;
        IsAlive = true;
        StateMachine.ChangeState(IdleState);
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Lancer", this.gameObject);
    }
}
