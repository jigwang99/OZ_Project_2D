using UnityEngine;

public class Warrior : Unit
{
    public override UnitType Type => UnitType.Warrior;

    private int attackIndex;
    private int AttackIndexHash;
    
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<MeleeAttack>();
        AttackIndexHash = Animator.StringToHash("AttackIndex");
    }
    public override void PlayAttackAnimation()
    {
        animator.SetInteger(AttackIndexHash, attackIndex);
        animator.SetTrigger(isAttack);
        attackIndex = (attackIndex + 1) % 2; // 0 - 1 - 0 - 1 반복
    }
    public override void Init()
    {
        base.Init();
        attackIndex = 0;
    }
}
