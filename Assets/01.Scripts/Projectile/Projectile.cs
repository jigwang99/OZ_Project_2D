    using UnityEngine;

public abstract class Projectile : MonoBehaviour, IPoolable
{
    protected ProjectileStat projectileStat;
    protected float timer;
    protected Rigidbody2D rb;
    protected bool isReturned;
    public abstract ProjectileType Type { get; }
    public Vector2 Direction { get; protected set; }

    public int Damage { get; set; }
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    protected void OnEnable()
    {
        Init();
    }
    protected void Update()
    {
        if (projectileStat == null)
            return;

        if(timer > projectileStat.LifeTime)
            ReturnToPool();
    }
    protected void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        rb.linearVelocity = transform.right * projectileStat.Speed;
    }
    public void SetDamageAndLayer(int damage, int shooterLayer)
    {
        Damage = damage;

        gameObject.layer = shooterLayer == (int)Layer.Player || shooterLayer == (int)Layer.PlayerBuilding 
            ? (int)Layer.PlayerProjectile : (int)Layer.EnemyProjectile;
    }
    protected abstract void OnTriggerEnter2D(Collider2D collision);
    public virtual void Init()
    {
        if(projectileStat == null)
            projectileStat = ProjectileDataLoader.instance.GetProjectileStat(Type);
        timer = 0f;
        Direction = Vector2.zero;
        Damage = 0;
        isReturned = false;
    }
    public abstract void ReturnToPool();
}
