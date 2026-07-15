using UnityEngine;
using System;

public abstract class Resource : MonoBehaviour, IPoolable
{
    protected ResourceStat resourceStat;
    protected int remainAmount;

    public event Action OnDepleted;
    
    public abstract ResourceType Type { get; }
    public bool IsDepleted;
    protected void OnEnable()
    {
        Init();
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
    }
    public virtual void Init()
    {
        if (resourceStat == null)
        {
            if (ResourceManager.instance == null)
            {
                Debug.LogError("ResourceManager가 씬에 없거나 아직 초기화 전입니다.", this);
                return;
            }
            resourceStat = ResourceManager.instance.GetResourceStat(Type);
            if (resourceStat == null)
            {
                Debug.LogError($"{Type} 데이터가 ResourceData에 없습니다.", this);
                return;
            }
        }
        remainAmount = resourceStat.MaxAmount;
        IsDepleted = false;
    }
    public abstract void ReturnToPool();
}
