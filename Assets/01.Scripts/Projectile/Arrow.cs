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
        collision.gameObject.GetComponent<Unit>().TakeDamage(Damage);
        ReturnToPool();
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Arrow", this.gameObject);
        transform.rotation = Quaternion.identity;
    }
}