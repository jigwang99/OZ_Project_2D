using UnityEngine;

public class Arrow : Projectile
{
    public override ProjectileType Type => ProjectileType.Arrow;
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Unit damageable = collision.gameObject.GetComponent<Unit>();
        if (damageable != null)
            damageable.TakeDamage(Damage);
        ReturnToPool();
    }
    public override void ReturnToPool()
    {
        if (isReturned)
            return;
        isReturned = true;
        ObjectPoolManager.instance.ReturnObject(Type.ToString(), gameObject);
    }
}