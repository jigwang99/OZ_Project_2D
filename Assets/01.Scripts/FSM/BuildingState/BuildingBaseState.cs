using UnityEngine;

public abstract class BuildingBaseState : IState
{
    public Building Building {  get; protected set; }
    public BuildingBaseState(Building building)
    {
        this.Building = building;
    }
    public abstract void Enter();
    public abstract void Exit();
    public abstract void FixedUpdate();
    public abstract void Update();
}

