using UnityEngine;

public class Monk : Unit
{
    public UnitHeal Heal { get; private set; }
    public UnitHealState HealState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Heal = GetComponent<UnitHeal>();
        HealState = new UnitHealState(this);
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
