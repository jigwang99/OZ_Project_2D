using UnityEngine;

public class UnitHealState : UnitBaseState
{
    private Monk monk;
    private UnitHeal heal;
    private float refindTimer;
    private const float refindInterval = 0.25f;

    public UnitHealState(Unit unit) : base(unit)
    {
        monk = unit as Monk;
        heal = monk.Heal;
    }
    public override void Enter()
    {
        Unit.Movement.SetMoveSpeed(Unit.UnitStat.MoveSpeed);
        refindTimer = 0f;
    }

    public override void Exit()
    {
        Unit.Movement.Stop();
    }

    public override void FixedUpdate()
    {
        Unit target = heal.GetTarget();
        if (target == null)
            return;

        if (heal.IsInRange())
            return;

        refindTimer -= Time.fixedDeltaTime;
        if(refindTimer <= 0f)
        {
            Unit.Movement.SetDestination(target.transform.position);
            refindTimer = refindInterval;
        }
        Unit.Movement.Move();
    }

    public override void Update()
    {
        Unit target = heal.GetTarget();

        if (target == null || !target.IsAlive || target.CurrentHp >= target.UnitStat.MaxHp)
        {
            heal.SetTarget(null);
            Unit.StateMachine.ChangeState(Unit.IdleState);
            return;
        }

        if (heal.IsInRange() && heal.CanHeal())
        {
            monk.PlayHealAnimation();
            heal.Heal();
        }
        Unit.SetRunAnimation(!heal.IsInRange());
    }
}
