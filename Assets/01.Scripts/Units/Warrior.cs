using UnityEngine;

public class Warrior : Unit
{
    protected override void Awake()
    {
        base.Awake();
        //Attack = GetComponent<MeleeAttack>();
    }
    protected void OnEnable()
    {
        unitStat = UnitManager.instance.GetUnitStat(UnitType.Warrior);
        CurrentHp = unitStat.MaxHp;
        IsAlive = true;
    }
}
