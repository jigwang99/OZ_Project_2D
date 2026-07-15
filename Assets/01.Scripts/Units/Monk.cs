using UnityEngine;

public class Monk : Unit
{
    public override UnitType Type => UnitType.Monk;
    public UnitHeal Heal { get; private set; }
    public UnitHealState HealState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Heal = GetComponent<UnitHeal>();
        HealState = new UnitHealState(this);
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Monk", this.gameObject);
    }
}
