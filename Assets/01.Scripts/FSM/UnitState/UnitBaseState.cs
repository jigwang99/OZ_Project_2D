public abstract class UnitBaseState : IState
{
    public Unit Unit {  get; protected set; }
    public UnitBaseState(Unit unit)
    {
        Unit = unit;
    }
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    public abstract void FixedUpdate();
}
