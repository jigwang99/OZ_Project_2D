using System;
public interface IPoolable
{
    Enum PoolKey { get; }
    void Init();
    void ReturnToPool();
}
