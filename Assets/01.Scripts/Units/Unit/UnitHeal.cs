using UnityEngine;

public class UnitHeal : MonoBehaviour
{
    [SerializeField] private LayerMask allyLayerMask;

    [Header("Audio")]
    [SerializeField] private AudioClip healClip;

    private Unit unit;
    private Unit target;
    private float remainCooldown;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }
    private void OnEnable()
    {
        remainCooldown = 0f;
        target = null;
    }
    private void FixedUpdate()
    {
        if(remainCooldown > 0f)
            remainCooldown -= Time.fixedDeltaTime;
    }
    public bool CanHeal()
    {
        return remainCooldown <= 0f;
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
        if(target == null)
            return false;

        return RangeUtility.IsNear(unit.transform.position, target.transform, unit.UnitStat.AttackRange);
    }
    public Unit FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, unit.UnitStat.Vision, allyLayerMask);

        Unit mostSick = null;
        float lowestRatio = 1f;

        foreach(Collider2D hit in hits)
        {
            Unit ally = hit.GetComponent<Unit>();

            if (ally == null || !ally.IsAlive || ally == unit)
                continue;
            if (ally.CurrentHp == ally.UnitStat.MaxHp)
                continue;

            float ratio = (float)ally.CurrentHp / ally.UnitStat.MaxHp;
            if(ratio < lowestRatio)
            {
                lowestRatio = ratio;
                mostSick = ally;
            }
        }
        return mostSick;
    }
    public void Heal()
    {
        if (target == null || !target.IsAlive)
            return;
        if(healClip != null)
            AudioManager.instance?.PlaySFXAt(healClip, transform.position);
        target.RestoreHP(unit.UnitStat.AttackDamage);
        remainCooldown = unit.UnitStat.AttackCooldown;
    }
}
