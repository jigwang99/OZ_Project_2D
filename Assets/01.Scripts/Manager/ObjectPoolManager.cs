using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    // Unit, Projectile, Building
    [SerializeField] private int poolSize = 5;
    [SerializeField] private List<GameObject> objList;
    

    private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        InitializePools();
    }
    private void InitializePools()
    {
        foreach (GameObject go in objList)
        {
            GameObject parentObject = new GameObject($"{go.name}_Pool");
            parentObject.transform.SetParent(transform);

            Pool pool = new Pool(go, parentObject.transform, poolSize);

            pools.Add(go.name, pool);
        }
    }
    public T GetObject<T>(string key) where T : Component
    {
        if(!pools.ContainsKey(key))
            return null;
        return pools[key].GetObject<T>();
    }
    public void ReturnObject(string key, GameObject go)
    {
        if(!pools.ContainsKey(key))
        {
            Destroy(go);
            return;
        }
        pools[key].ReturnObject(go);
    }
}
