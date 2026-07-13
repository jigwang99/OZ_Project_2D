using UnityEngine;


public abstract class Resource : MonoBehaviour, IPoolable
{
    protected ResourceStat resourceStat;
    protected int remainAmount;

    public ResourceType ResourceType
    {
        get { return resourceStat.ResourceType; }
    }
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
        remainAmount -= amount;

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
        ReturnToPool();
    }
    public abstract void Init();
    public abstract void ReturnToPool();
}
