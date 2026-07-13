using UnityEngine;

public class RangedAttack : UnitAttack
{
    [SerializeField] private GameObject arrow;
    public override void Attack()
    {
        //GameObject arrow = ObjectPoolManager.instance.GetObject("arrow");
        //arrow.transfrom.position = unit.transform.position;
        //arrow.transform.rotation = transform.rotation;
        //arrow.GetComponent<Arrow>().SetDamage(unit.UnitStat.AttackDamage);
    }
}
    