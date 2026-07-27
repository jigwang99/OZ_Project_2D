using UnityEngine;
using System.Collections.Generic;

public abstract class DataLoader<TSelf, TKey, TStat> : MonoBehaviour where TSelf : DataLoader<TSelf, TKey, TStat>
{
    public static TSelf instance;

    private Dictionary<TKey, TStat> dictionary = new Dictionary<TKey, TStat>();

    protected abstract IReadOnlyList<TStat> StatList { get; }
    protected abstract TKey Key(TStat stat);
    protected abstract TStat Clone(TStat stat);

    private void Awake()
    {
        if (instance == null)
            instance = (TSelf)this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Load();
    }
    private void Load()
    {
        dictionary.Clear();
        for(int i = 0; i < StatList.Count; i++)
        {
            TStat clone = Clone(StatList[i]);
            dictionary[Key(clone)] = clone;
        }
    }
    public TStat Get(TKey key) => dictionary.GetValueOrDefault(key);
}
