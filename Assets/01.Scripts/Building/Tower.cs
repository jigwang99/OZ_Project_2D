using UnityEngine;

public class Tower : Building
{
    public override BuildingType Type => BuildingType.Tower;

    [SerializeField] private LayerMask enemyLayerMask;
    private IDamageable target;

    Collider2D hit;
    public BuildingAttackState AttackState { get; private set; }

    [SerializeField] private AudioClip fireClip;

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
    public bool IsInRange(IDamageable target)
    {
        if (target == null)
            return false;

        return RangeUtility.IsNear(transform.position, target.transform, buildingStat.AttackRange);
    }
    public IDamageable FindTarget()
    {
        hit = Physics2D.OverlapCircle(transform.position, buildingStat.AttackRange, enemyLayerMask);
        return hit != null ? hit.GetComponent<IDamageable>() : null;
    }
    public void Fire(IDamageable target)
    {
        ArrowLauncher.Fire(transform.position, target, buildingStat.AttackDamage, gameObject.layer, 0.3f);
        if(fireClip != null)
            AudioManager.instance?.PlaySFXAt(fireClip, transform.position);
    }
    public override void Init()
    {
        base.Init();
        target = null;
    }
}
