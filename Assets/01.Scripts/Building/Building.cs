using UnityEngine;

public abstract class Building : MonoBehaviour, IPoolable
{
    protected BuildingStat buildingStat;
    
    public BuildingStat BuildingStat => buildingStat;
    public int CurrentHp {  get; protected set; }
    public bool IsAlive { get; protected set; }

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
        Init();
    }
    protected void Update() => StateMachine.Update();
    protected void FixedUpdate() => StateMachine.FixedUpdate();
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
    }
    public abstract void Init();
    public abstract void ReturnToPool();
}
