using UnityEngine;

public class Arrow : Projectile
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Unit>().TakeDamage(Damage);
        ReturnToPool();
    }
    public override void Init()
    {
        timer = 0f;
        Direction = Vector2.zero;
        Damage = 0;
    }
    public override void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject("Arrow", this.gameObject);
        transform.rotation = Quaternion.identity;
    }
}
