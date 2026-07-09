using UnityEngine;

public abstract class BaseState : IState
{
    public Unit Unit {  get; private set; }
    public BaseState(Unit unit)
    {
        Unit = unit;
    }
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    public abstract void FixedUpdate();
}
