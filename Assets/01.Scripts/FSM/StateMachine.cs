using UnityEngine;

public interface IState
{
    public void Enter();
    public void Exit();
    public void Update();
    public void FixedUpdate();
}
public class StateMachine
{
    public IState CurrentState { get; private set; }
    public void ChangeState(IState state)
    {
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }
    public void Update() => CurrentState?.Update();

    public void FixedUpdate() => CurrentState?.FixedUpdate();
}
