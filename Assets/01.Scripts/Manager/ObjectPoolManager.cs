using UnityEngine;
using System.Collections.Generic;
using System;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    // Unit, Projectile, Building
    [SerializeField] private int poolSize;
    [SerializeField] private List<GameObject> objList;
    
    private Dictionary<Enum, Pool> pools = new Dictionary<Enum, Pool>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        InitializePools();
    }
    private void InitializePools()
    {
        foreach (GameObject go in objList)
        {
            IPoolable poolable = go.GetComponent<IPoolable>();
            if (poolable == null)
                continue;

            GameObject parentObject = new GameObject($"{go.name}_Pool");
            parentObject.transform.SetParent(transform);

            pools.Add(poolable.PoolKey, new Pool(go, parentObject.transform, poolSize));
        }
    }
    public T GetObject<T>(Enum key) where T : Component
    {
        if (!pools.TryGetValue(key, out Pool pool))
            return null;
        return pool.GetObject<T>();
    }
    public void ReturnObject(Enum key, GameObject go)
    {
        if(!pools.TryGetValue(key, out Pool pool))
        {
            Destroy(go);
            return;
        }
        pool.ReturnObject(go);
    }
}
