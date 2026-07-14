using UnityEngine;

public class Tower : Building
{
    [SerializeField] private LayerMask enemyLayerMask;
    private Unit target;

    Collider2D hit;
    public BuildingAttackState AttackState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AttackState = new BuildingAttackState(this);
    }
    protected void Start()
    {
        buildingStat = BuildingManager.instance.GetBuildingStat(BuildingType.Tower);
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
        Transform arrow = ObjectPoolManager.instance.GetObject<Transform>("Arrow");
        arrow.position = transform.position;
        Vector2 dir = (target.transform.position - arrow.position).normalized;
        //arrow.rotation = Quaternion.
        arrow.GetComponent<Arrow>().Damage = buildingStat.AttackDamage;
    }
    public override void Init()
    {
        CurrentHp = buildingStat.MaxHp;
        IsAlive = true;
        target = null;
        StateMachine.ChangeState(BuildedState);
    }

    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Tower", this.gameObject);
    }
}
