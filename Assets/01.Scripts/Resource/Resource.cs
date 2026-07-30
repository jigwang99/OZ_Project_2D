using UnityEngine;
using System;
using System.Collections;

public abstract class Resource : MonoBehaviour, IPoolable
{
    protected ResourceStat resourceStat;
    protected int remainAmount;

    [SerializeField] private Vector2 obstacleSize;

    public event Action OnDepleted;
    
    public abstract ResourceType Type { get; }
    public bool IsDepleted;

    public Enum PoolKey => Type;

    protected void OnEnable()
    {
        Init();
        StartCoroutine(RegisterObstacleNextFrame());
    }
    public int Gathered(int amount)
    {
        if (IsDepleted)
            return 0;

        int gathered = Mathf.Min(amount, remainAmount);
        remainAmount -= gathered;

        if(remainAmount <= 0 )
        {
            remainAmount = 0;
            Deplete();
        }

        return gathered;
    }
    private void Deplete()
    {
        IsDepleted = true;
        OnDepleted?.Invoke();
        OnDepleted = null;
        ReturnToPool();
        GridManager.instance.UpdateArea(transform.position, obstacleSize);
    }
    private IEnumerator RegisterObstacleNextFrame()
    {
        yield return null;
        GridManager.instance.UpdateArea(transform.position, obstacleSize);
    }
    public virtual void Init()
    {
        if (resourceStat == null)
        {
            resourceStat = ResourceDataLoader.instance.Get(Type);
        }
        remainAmount = resourceStat.MaxAmount;
        IsDepleted = false;
    }
    public virtual void ReturnToPool()
    {
        if (!IsDepleted)
            return;
        ObjectPoolManager.instance.ReturnObject(PoolKey, gameObject);
    }
}
