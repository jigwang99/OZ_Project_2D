using UnityEngine;

public interface IDamageable
{
    Transform transform { get; }
    bool IsAlive { get; }
    void TakeDamage(int attackDamage);
}
