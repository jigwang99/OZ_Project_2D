using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected UnitData data;

    public Unit Target {  get; protected set; }
    public UnitData Data => data;
    public UnitMovement Movement { get; protected set; }
    public UnitAttack Attack { get; protected set; }
    public int CurrentHp { get; protected set; }

    public StateMachine StateMachine { get; protected set; }
    public IdleState IdleState { get; protected set; }
    public MoveState MoveState { get; protected set; }
    public AttackState AttackState { get; protected set; }
    protected void Awake()
    {
        
        Movement = GetComponent<UnitMovement>();
        Attack = GetComponent<UnitAttack>();

        StateMachine = new StateMachine();
        IdleState = new IdleState(this);
        MoveState = new MoveState(this);
        AttackState = new AttackState(this);
    }
    protected void OnEnable()
    {
        CurrentHp = Data.MaxHp;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        StateMachine.Update();
    }
    protected void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }
    public void TakeDamage(int damage)
    {
        CurrentHp -= damage - Data.Defense;

        if(CurrentHp <= 0)
        {
            CurrentHp = 0;
            Die();
        }
    }
    protected void Die()
    {

    }
}
