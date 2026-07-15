using UnityEngine;

public abstract class Projectile : MonoBehaviour, IPoolable
{
    protected ProjectileStat projectileStat;
    protected float timer;
    protected Rigidbody2D rb;

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
        if(timer > projectileStat.LifeTime)
            ReturnToPool();
    }
    protected void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        rb.linearVelocity = transform.right * projectileStat.Speed;
    }
    protected abstract void OnCollisionEnter2D(Collision2D collision);
    public virtual void Init()
    {
        if(projectileStat == null)
            projectileStat = ProjectileManager.instance.GetProjectileStat(Type);
        timer = 0f;
        Direction = Vector2.zero;
        Damage = 0;
    }
    public abstract void ReturnToPool();
}
