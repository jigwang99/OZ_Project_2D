using UnityEngine;

public class MeleeAttack : UnitAttack
{
    public override void Attack()
    {
        if (target == null || !target.IsAlive)
            return;
        target.TakeDamage(unit.UnitStat.AttackDamage);
        remainCooldown = unit.UnitStat.AttackCooldown;
        PlayAttackSound();
    }
}
