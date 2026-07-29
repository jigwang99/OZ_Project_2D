using UnityEngine;

public class Arrow : Projectile
{
    public override ProjectileType Type => ProjectileType.Arrow;

    [SerializeField] private AudioClip hitClip;

    protected override void Start()
    {
        base.Start();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        { 
            damageable.TakeDamage(Damage);
            if(hitClip != null)
                AudioManager.instance?.PlaySFXAt(hitClip, transform.position);
        }
        ReturnToPool();
    }
    public override void ReturnToPool()
    {
        if (isReturned)
            return;
        isReturned = true;
        ObjectPoolManager.instance.ReturnObject(PoolKey, gameObject);
    }
}