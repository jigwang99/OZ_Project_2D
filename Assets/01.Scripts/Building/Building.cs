using System.Collections;
using System;
using UnityEngine;

public abstract class Building : MonoBehaviour, IPoolable, IDamageable
{
    protected BuildingStat buildingStat;
    [SerializeField] private Vector2 obstacleSize;

    private bool populationProvided;
    private bool IsRegistered;

    public abstract BuildingType Type { get; }
    public BuildingStat BuildingStat => buildingStat;

    public int CurrentHp {  get; protected set; }
    public bool IsAlive { get; protected set; }
    public bool IsSelected { get; protected set; }
    public bool IsSkipBuilded { get; protected set; }
    public float BuildProgress { get; protected set; }
    public bool IsConstruction => StateMachine.CurrentState == BuildedState;

    public BuildingIdleState IdleState { get; protected set; }
    public BuildingBuildedState BuildedState { get; protected set; }
    public StateMachine StateMachine { get; protected set; }
    
    public Faction OwnerFaction { get; private set; }

    private SelectCircle selectCircle;
    private MinimapMarker minimapMarker;

    private BuildingEffect effect;

    public Enum PoolKey => Type;

    public event Action<Building> OnDied;

    protected virtual void Awake()
    {
        StateMachine = new StateMachine();
        IdleState = new BuildingIdleState(this);
        BuildedState = new BuildingBuildedState(this);

        selectCircle = GetComponent<SelectCircle>();
        minimapMarker = GetComponent<MinimapMarker>();
        
        effect = GetComponent<BuildingEffect>();
    }
    protected virtual void OnEnable()
    {
        SetSelected(false);
        StartCoroutine(RegisterObstacleNextFrame());
    }
    protected void Update() => StateMachine.Update();
    protected void FixedUpdate() => StateMachine.FixedUpdate();
    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        selectCircle?.SetVisible(selected);
    }
    public void TakeDamage(int attackDamage)
    {
        int damage = Mathf.Max(1, attackDamage - buildingStat.Defense);

        CurrentHp -= damage;

        if(CurrentHp <= 0)
        {
            CurrentHp = 0;
            Die();
        }
        effect?.UpdateFire((float)CurrentHp / buildingStat.MaxHp);
    }
    protected void Die()
    {
        if (!IsAlive)
            return;

        IsAlive = false;
        UnregisterOwner();
        WithdrawPopulation();

        OnDied?.Invoke(this);

        effect?.PlayExplosion();
        ReturnToPool();
        GridManager.instance.UpdateArea(transform.position, obstacleSize);
    }
    private IEnumerator RegisterObstacleNextFrame()
    {
        yield return null;
        GridManager.instance.UpdateArea(transform.position, obstacleSize);
    }
    public void ResetProgress()
    {
        BuildProgress = 0;
    }
    public void Construct(float amount)
    {
        BuildProgress += amount;
    }
    public void ProvidePopulation()
    {
        RegisterOwner();
        if (populationProvided || buildingStat.PopulationProvide <= 0)
            return;

        populationProvided = true;
        OwnerFaction.AddMaxPopulation(buildingStat.PopulationProvide);
    }
    public void WithdrawPopulation()
    {
        if (!populationProvided)
            return;

        populationProvided = false;
        OwnerFaction.AddMaxPopulation(-buildingStat.PopulationProvide);
    }
    public void SetSkipBuilded(bool skip)
    {
        IsSkipBuilded = skip;
    }
    public void SetLayer(Layer ownerLayer)
    {
        gameObject.layer = (int)(LayerUtility.IsPlayerSide((int)ownerLayer)
            ? Layer.PlayerBuilding : Layer.EnemyBuilding);

        OwnerFaction = FactionManager.instance.FromLayer(gameObject.layer);
        GetComponent<BuildingVisual>()?.ApplySprite();
        minimapMarker?.ApplyFaction(gameObject.layer);
    }
    private void RegisterOwner()
    {
        if (IsRegistered)
            return;
        IsRegistered = true;
        OwnerFaction.RegisterBuilding(this);
    }
    private void UnregisterOwner()
    {
        if (!IsRegistered)
            return;
        IsRegistered = false;
        OwnerFaction.UnregisterBuilding(this);
    }
    public virtual void Init()
    {
        if (buildingStat == null)
            buildingStat = BuildingDataLoader.instance.Get(Type);

        populationProvided = false;
        IsRegistered = false;
        CurrentHp = buildingStat.MaxHp;
        IsAlive = true;
        StateMachine.ChangeState(IsSkipBuilded ? IdleState : BuildedState);
        if(IsSkipBuilded)
            ProvidePopulation();
        effect?.UpdateFire(1f);
    }
    public virtual void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject(PoolKey, gameObject);
    }
    protected virtual void OnDisable() { }
}
