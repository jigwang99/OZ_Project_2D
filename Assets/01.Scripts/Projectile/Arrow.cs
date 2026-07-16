using UnityEngine;

public class Arrow : Projectile
{
    public override ProjectileType Type => ProjectileType.Arrow;
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(Damage);
        ReturnToPool();
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Arrow", this.gameObject);
        transform.rotation = Quaternion.identity;
    }
}