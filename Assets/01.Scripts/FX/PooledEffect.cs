using UnityEngine;

public class PooldEffect : MonoBehaviour, IPoolable
{
    [SerializeField] private string poolKey;
    [SerializeField] private float liftTime = 1.0f;

    private float timer;

    public void Init()
    {
        timer = 0f;
    }
    private void OnEnable()
    {
        Init();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= liftTime)
            ReturnToPool();
    }
    public void ReturnToPool()
    {
        ObjectPoolManager.instance.ReturnObject(poolKey, gameObject);
    }
}
