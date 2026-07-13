using UnityEngine;

public enum Layer
{
    Player = 6,
    Enemy = 7,
    PlayerProjectile = 8,
    EnemyProjectile = 9,
    PlayerBuilding = 10,
    EnemyBuilding = 11,
}

public abstract class Unit : MonoBehaviour, IPoolable
{
    protected UnitStat unitStat;
    protected Collider2D hit;
    protected int allianceMask;
    protected int enemyMask;

    public int CurrentHp { get; protected set; }
    public bool IsAlive { get; protected set; }
    public UnitStat UnitStat => unitStat;
    public UnitMovement Movement { get; protected set; }
    public UnitAttack Attack { get; protected set; }
    

    public StateMachine StateMachine { get; protected set; }
    public UnitIdleState IdleState { get; protected set; }
    public UnitMoveState MoveState { get; protected set; }
    public UnitChaseState ChaseState { get; protected set; }
    public UnitAttackState AttackState { get; protected set; }
    protected virtual void Awake()
    {
        Movement = GetComponent<UnitMovement>();

        StateMachine = new StateMachine();
        IdleState = new UnitIdleState(this);
        MoveState = new UnitMoveState(this);
        ChaseState = new UnitChaseState(this);
        AttackState = new UnitAttackState(this);
    }
    protected void OnEnable()
    {
        Init();
    }
    // Update is called once per frame
    protected void Update() => StateMachine.Update();
    protected void FixedUpdate() => StateMachine.FixedUpdate();
    public void TakeDamage(int attackDamage)
    {
        int damage = Mathf.Max(1, attackDamage - UnitStat.Defense);

        CurrentHp -= damage;

        if(CurrentHp <= 0)
        {
            CurrentHp = 0;
            Die();
        }
    }
    protected void Die()
    {
        IsAlive = false;
        ReturnToPool();
    }
    public abstract void Init();
    public abstract void ReturnToPool();
}
