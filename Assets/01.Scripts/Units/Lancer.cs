using UnityEngine;

public class Lancer : Unit
{
    public override UnitType Type => UnitType.Lancer;
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<MeleeAttack>();
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Lancer", this.gameObject);
    }
}
