using System.Collections;
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
    protected virtual void Awake()
    {
        StateMachine = new StateMachine();
        IdleState = new BuildingIdleState(this);
        BuildedState = new BuildingBuildedState(this);

        selectCircle = GetComponent<SelectCircle>();
        minimapMarker = GetComponent<MinimapMarker>();
    }
    protected virtual void OnEnable()
    {
        SetSelected(false);
        StartCoroutine(RegisterObtacleNextFrame());
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
    }
    protected void Die()
    {
        IsAlive = false;
        UnregisterOwner();

        if(Player.instance.SelectBuilding == this)
            Player.instance.DeselectBuilding();

        WithdrawPopulation();
        ReturnToPool();
        GridManager.instance.UpdateArea(transform.position, obstacleSize);
    }
    private IEnumerator RegisterObtacleNextFrame()
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
        Debug.Log($"진행도 : {BuildProgress:F2} / {BuildingStat.BuildTime}");
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
        gameObject.layer = (int)(ownerLayer == Layer.Player || ownerLayer == Layer.PlayerBuilding
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
            buildingStat = BuildingDataLoader.instance.GetBuildingStat(Type);

        populationProvided = false;
        IsRegistered = false;
        CurrentHp = buildingStat.MaxHp;
        IsAlive = true;
        StateMachine.ChangeState(IsSkipBuilded ? IdleState : BuildedState);
        if(IsSkipBuilded)
            ProvidePopulation();
    }
    public abstract void ReturnToPool();
    protected virtual void OnDisable() { }
}
