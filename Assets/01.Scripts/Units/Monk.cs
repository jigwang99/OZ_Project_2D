using UnityEngine;

public class Monk : Unit
{
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<MeleeAttack>();
    }
    protected void Start()
    {
        unitStat = UnitManager.instance.GetUnitStat(UnitType.Monk);
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Monk", this.gameObject);
    }
}
