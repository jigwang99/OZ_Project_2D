using UnityEngine;

public class Arrow : Projectile
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Unit>().TakeDamage(Damage);
        ReturnToPool();
    }
    protected override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Arrow", this.gameObject);
        transform.rotation = Quaternion.identity;
    }
}
