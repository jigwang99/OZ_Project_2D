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

public abstract class Unit : MonoBehaviour
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
    public IdleState IdleState { get; protected set; }
    public MoveState MoveState { get; protected set; }
    public ChaseState ChaseState { get; protected set; }
    public AttackState AttackState { get; protected set; }
    protected virtual void Awake()
    {
        Movement = GetComponent<UnitMovement>();

        StateMachine = new StateMachine();
        IdleState = new IdleState(this);
        MoveState = new MoveState(this);
        ChaseState = new ChaseState(this);
        AttackState = new AttackState(this);
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
        gameObject.SetActive(false);
    }
}
