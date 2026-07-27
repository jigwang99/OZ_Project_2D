using UnityEngine;

public class Tower : Building
{
    public override BuildingType Type => BuildingType.Tower;

    [SerializeField] private LayerMask enemyLayerMask;
    private IDamageable target;

    Collider2D hit;
    public BuildingAttackState AttackState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AttackState = new BuildingAttackState(this);
    }
    public IDamageable GetTarget()
    {
        return target;
    }
    public void SetTarget(IDamageable target)
    {
        this.target = target;
    }
    public bool IsInRange(IDamageable targete)
    {
        return Vector2.Distance(transform.position, target.transform.position) <= BuildingStat.AttackRange;
    }
    public Unit FindTarget()
    {
        hit = Physics2D.OverlapCircle(transform.position, buildingStat.AttackRange, enemyLayerMask);
        return hit != null ? hit.GetComponent<Unit>() : null;
    }
    public void Fire(IDamageable target)
    {
        ArrowLauncher.Fire(transform.position, target, buildingStat.AttackDamage, gameObject.layer, 0.3f);
    }
    public override void Init()
    {
        base.Init();
        target = null;
    }
}
