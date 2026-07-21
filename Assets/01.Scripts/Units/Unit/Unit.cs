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
public abstract class Unit : MonoBehaviour, IPoolable, IDamageable
{
    protected UnitStat unitStat;
    protected Collider2D hit;
    protected int allianceMask;
    protected int enemyMask;

    public abstract UnitType Type { get; }
    public int CurrentHp { get; protected set; }
    public bool IsAlive { get; protected set; }
    public bool IsSelected { get; protected set; }
    public UnitStat UnitStat => unitStat;
    public UnitMovement Movement { get; protected set; }
    public UnitAttack Attack { get; protected set; }
    

    public StateMachine StateMachine { get; protected set; }
    public UnitIdleState IdleState { get; protected set; }
    public UnitMoveState MoveState { get; protected set; }
    public UnitChaseState ChaseState { get; protected set; }
    public UnitAttackState AttackState { get; protected set; }

    private SpriteRenderer spriteRenderer;

    protected Animator animator;
    private int isRun;
    protected int isAttack;
    protected virtual void Awake()
    {
        Movement = GetComponent<UnitMovement>();

        StateMachine = new StateMachine();
        IdleState = new UnitIdleState(this);
        MoveState = new UnitMoveState(this);
        ChaseState = new UnitChaseState(this);
        AttackState = new UnitAttackState(this);

        animator = GetComponent<Animator>();
        isRun = Animator.StringToHash("isRun");
        isAttack = Animator.StringToHash("isAttack");

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected void OnEnable()
    {
        Init();
    }
    // Update is called once per frame
    protected void Update() => StateMachine.Update();
    protected void FixedUpdate() => StateMachine.FixedUpdate();
    public void SetSelected(bool selected)
    {
        IsSelected = selected;
    }
    public void RestoreHP(int amount)
    {
        if (!IsAlive)
            return;
        CurrentHp = Mathf.Min(CurrentHp + amount, unitStat.MaxHp);
    }
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
        if(gameObject.layer == (int)Layer.Player)
        {
            Player.instance.DeselectUnit(this);
            Player.instance.ReleasePopulation(unitStat.Population);
        }
        ReturnToPool();
    }
    public void SetRunAnimation(bool isRun)
    {
        animator.SetBool(this.isRun, isRun);
    }
    public virtual void PlayAttackAnimation()
    {
        animator.SetTrigger(isAttack);
    }
    public void FlipSprite(float directionX)
    {
        spriteRenderer.flipX = directionX < 0;
    }
    public LayerMask GetEnemyLayerMask()
    {
        return enemyMask;
    }
    public void SetLayer(Layer layer)
    {
        gameObject.layer = (int)layer;

        if(layer == Layer.Player)
        {
            allianceMask = (1 << (int)Layer.Player) | (1 << (int)Layer.PlayerBuilding);
            enemyMask = (1 << (int)Layer.Enemy) | (1 << (int)Layer.EnemyBuilding);
        }
        else
        {
            allianceMask = (1 << (int)Layer.Enemy) | (1 << (int)Layer.EnemyBuilding);
            enemyMask = (1 << (int)Layer.Player) | (1 << (int)Layer.PlayerBuilding);
        }
        GetComponent<UnitVisual>()?.ApplyAnime();
    }
    public virtual void Init()
    {
        if (unitStat == null)
            unitStat = UnitDataLoader.instance.GetUnitStat(Type);
        SetSelected(false);
        CurrentHp = unitStat.MaxHp;
        IsAlive = true;

        if (gameObject.layer == (int)Layer.Player)
            enemyMask = (1 << (int)Layer.Enemy) | (1 << (int)Layer.EnemyBuilding);
        else
            enemyMask = (1 << (int)Layer.Player) | (1 << (int)Layer.PlayerBuilding);

        StateMachine.ChangeState(IdleState);
    }
    public abstract void ReturnToPool();
}
