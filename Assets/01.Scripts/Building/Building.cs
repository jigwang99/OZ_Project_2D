using System.Collections;
using UnityEngine;

public abstract class Building : MonoBehaviour, IPoolable
{
    protected BuildingStat buildingStat;
    [SerializeField] private Vector2 obstacleSize;

    public abstract BuildingType Type { get; }
    public BuildingStat BuildingStat => buildingStat;

    public int CurrentHp {  get; protected set; }
    public bool IsAlive { get; protected set; }
    public bool IsSelected { get; protected set; }

    public BuildingIdleState IdleState { get; protected set; }
    public BuildingBuildedState BuildedState { get; protected set; }
    public StateMachine StateMachine { get; protected set; }
    
    protected virtual void Awake()
    {
        StateMachine = new StateMachine();
        IdleState = new BuildingIdleState(this);
        BuildedState = new BuildingBuildedState(this);
    }
    protected virtual void OnEnable()
    {
        SetSelected(false);
        Init();
        StartCoroutine(RegisterObtacleNextFrame());
    }
    protected void Update() => StateMachine.Update();
    protected void FixedUpdate() => StateMachine.FixedUpdate();
    public void SetSelected(bool selected)
    {
        IsSelected = selected;
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
        ReturnToPool();
        GridManager.instance.UpdateArea(transform.position, obstacleSize);
    }
    private IEnumerator RegisterObtacleNextFrame()
    {
        yield return null;
        GridManager.instance.UpdateArea(transform.position, obstacleSize);
    }
    public virtual void Init()
    {
        if (buildingStat == null)
            buildingStat = BuildingManager.instance.GetBuildingStat(Type);
        CurrentHp = buildingStat.MaxHp;
        IsAlive = true;
        StateMachine.ChangeState(BuildedState);
    }
    public abstract void ReturnToPool();
}
