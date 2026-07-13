using UnityEngine;

public abstract class UnitAttack : MonoBehaviour
{
    protected Unit unit;
    protected Unit target;
    protected float remainCooldown;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }
    private void OnEnable()
    {
        remainCooldown = 0;
    }
    private void Update()
    {
        if (remainCooldown > 0)
            remainCooldown -= Time.deltaTime;
    }
    public bool CanAttack()
    {
        return remainCooldown <= 0;
    }
    public Unit GetTarget()
    {
        return target;
    }
    public void SetTarget(Unit target)
    {
        this.target = target;
    }
    public bool IsInRange()
    {
        if(target == null) return false;
        return Vector2.Distance(unit.transform.position, target.transform.position) <= unit.UnitStat.AttackRange;
    }
    public abstract void Attack();
}