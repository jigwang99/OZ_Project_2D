using UnityEngine;

public class Monk : Unit
{
    public override UnitType Type => UnitType.Monk;
    public UnitHeal Heal { get; private set; }
    public UnitHealState HealState { get; private set; }

    private int IsHeal;
    protected override void Awake()
    {
        base.Awake();
        Heal = GetComponent<UnitHeal>();
        HealState = new UnitHealState(this);

        IsHeal = Animator.StringToHash("isHeal");
    }
    public void PlayHealAnimation()
    {
        animator.SetTrigger(IsHeal);
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Monk", this.gameObject);
    }
}
