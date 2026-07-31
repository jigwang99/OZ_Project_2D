using UnityEngine;

public class RangedAttack : UnitAttack
{
    [SerializeField] private float offset = 0.3f;
    public override void Attack()
    {
        if (ArrowLauncher.Fire(unit.transform.position, target, unit.UnitStat.AttackDamage, unit.gameObject.layer, offset))
        {
            remainCooldown = unit.UnitStat.AttackCooldown;
            PlayAttackSound();
        }
    }
}
    