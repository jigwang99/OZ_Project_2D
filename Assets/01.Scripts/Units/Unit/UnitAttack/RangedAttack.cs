using UnityEngine;

public class RangedAttack : UnitAttack
{
    public override void Attack()
    {
        Transform arrowTransform = ObjectPoolManager.instance.GetObject<Transform>("Arrow");
        arrowTransform.position = unit.transform.position;
        arrowTransform.rotation = unit.transform.rotation;
        arrowTransform.GetComponent<Arrow>().SetDamageAndLayer(unit.UnitStat.AttackDamage, unit.gameObject.layer);
        remainCooldown = unit.UnitStat.AttackCooldown;
    }
}
    