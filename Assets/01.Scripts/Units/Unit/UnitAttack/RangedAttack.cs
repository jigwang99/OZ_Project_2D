using UnityEngine;

public class RangedAttack : UnitAttack
{
    [SerializeField] private float offset = 0.3f;
    public override void Attack()
    {
        if (target == null || !target.IsAlive)
            return;

        Arrow arrow = ObjectPoolManager.instance.GetObject<Arrow>("Arrow");
        if (arrow == null)
            return;

        Vector2 origin = unit.transform.position;
        Vector2 dir = ((Vector2)target.transform.position - origin).normalized;
        if (dir == Vector2.zero)
            dir = Vector2.right;

        arrow.transform.position = origin + dir * offset;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        arrow.SetDamageAndLayer(unit.UnitStat.AttackDamage, unit.gameObject.layer);

        remainCooldown = unit.UnitStat.AttackCooldown;
    }
}
    