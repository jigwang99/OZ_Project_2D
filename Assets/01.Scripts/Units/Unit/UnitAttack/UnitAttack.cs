using UnityEngine;

public abstract class UnitAttack : MonoBehaviour
{
    protected Unit unit;
    protected IDamageable target;
    protected float remainCooldown;

    [SerializeField] protected AudioClip attackClip;

    private Collider2D hit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }
    private void OnEnable()
    {
        remainCooldown = 0;
        target = null;
    }
    private void FixedUpdate()
    {
        if (remainCooldown > 0f)
            remainCooldown -= Time.fixedDeltaTime;
    }
    public bool CanAttack()
    {
        return remainCooldown <= 0;
    }
    public IDamageable GetTarget()
    {
        return target;
    }
    public void SetTarget(IDamageable target)
    {
        this.target = target;
    }
    public bool IsInRange()
    {
        if (target == null)
            return false;

        return RangeUtility.IsNear(unit.transform.position, target.transform, unit.UnitStat.AttackRange);   
    }
    public Unit FindTarget()
    {
        hit = Physics2D.OverlapCircle(transform.position, unit.UnitStat.Vision, unit.GetEnemyLayerMask());
        return hit != null ? hit.GetComponent<Unit>() : null;
    }
    protected void PlayAttackSound()
    {
        if (attackClip != null)
            AudioManager.instance?.PlaySFXAt(attackClip, unit.transform.position);
    }
    public abstract void Attack();
}