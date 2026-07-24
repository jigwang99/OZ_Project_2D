using UnityEngine;

public abstract class UnitAttack : MonoBehaviour
{
    protected Unit unit;
    protected IDamageable target;
    protected float remainCooldown;

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
        if(target == null) return false;

        Collider2D col = target.transform.GetComponent<Collider2D>();
        Vector2 point = col != null ? col.ClosestPoint(unit.transform.position) : (Vector2)target.transform.position;

        return Vector2.Distance(unit.transform.position, point) <= unit.UnitStat.AttackRange;
    }
    public Unit FindTarget()
    {
        hit = Physics2D.OverlapCircle(transform.position, unit.UnitStat.Vision, unit.GetEnemyLayerMask());
        return hit != null ? hit.GetComponent<Unit>() : null;
    }
    public abstract void Attack();
}