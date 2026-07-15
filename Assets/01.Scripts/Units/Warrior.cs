using UnityEngine;

public class Warrior : Unit
{
    public override UnitType Type => UnitType.Warrior;
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<MeleeAttack>();
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Warrior", this.gameObject);
    }
}
