using UnityEngine;

public class Tower : Building
{
    public override BuildingType Type => BuildingType.Tower;

    [SerializeField] private LayerMask enemyLayerMask;
    private Unit target;

    Collider2D hit;
    public BuildingAttackState AttackState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AttackState = new BuildingAttackState(this);
    }
    public Unit GetTarget()
    {
        return target;
    }
    public void SetTarget(Unit target)
    {
        this.target = target;
    }
    public bool IsInRange(Unit unit)
    {
        return Vector2.Distance(transform.position, unit.transform.position) <= BuildingStat.AttackRange;
    }
    public Unit FindTarget()
    {
        hit = Physics2D.OverlapCircle(transform.position, buildingStat.AttackRange, enemyLayerMask);
        return hit != null ? hit.GetComponent<Unit>() : null;
    }
    public void Fire(Unit target)
    {
        if(target == null || !target.IsAlive)
            return;

        Arrow arrow = ObjectPoolManager.instance.GetObject<Arrow>("Arrow");
        if (arrow == null)
            return;

        Vector2 origin = transform.position;
        Vector2 dir = ((Vector2)target.transform.position - origin).normalized;
        if (dir == Vector2.zero)
            dir = Vector2.right;

        arrow.transform.position = origin + dir * 0.3f;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        arrow.SetDamageAndLayer(buildingStat.AttackDamage, gameObject.layer);
    }
    public override void Init()
    {
        base.Init();
        target = null;
    }

    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Tower", this.gameObject);
    }
}
