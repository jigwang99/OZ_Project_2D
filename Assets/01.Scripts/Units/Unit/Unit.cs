using System;
using UnityEngine;

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

    public Faction OwnerFaction { get; protected set; }

    private SelectCircle selectCircle;
    private MinimapMarker minimapMarker;

    public event Action<Unit> OnDied;

    public Enum PoolKey => Type;
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

        selectCircle = GetComponent<SelectCircle>();
        minimapMarker = GetComponent<MinimapMarker>();
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
        selectCircle?.SetVisible(selected);
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
        if (!IsAlive)
            return;
        IsAlive = false;

        OwnerFaction.UnregisterUnit(this);
        OwnerFaction.ReleasePopulation(unitStat.Population);

        OnDied?.Invoke(this);
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
        OwnerFaction = FactionManager.instance.FromLayer((int)layer);
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
        OwnerFaction.RegisterUnit(this);
        GetComponent<UnitVisual>()?.ApplyAnime();
        minimapMarker?.ApplyFaction((int)layer);
    }
    public virtual void Init()
    {
        if (unitStat == null)
            unitStat = UnitDataLoader.instance.Get(Type);
        SetSelected(false);
        CurrentHp = unitStat.MaxHp;
        IsAlive = true;

        if (gameObject.layer == (int)Layer.Player)
            enemyMask = (1 << (int)Layer.Enemy) | (1 << (int)Layer.EnemyBuilding);
        else
            enemyMask = (1 << (int)Layer.Player) | (1 << (int)Layer.PlayerBuilding);

        StateMachine.ChangeState(IdleState);
    }
    public virtual void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject(PoolKey, gameObject);
    }
}
