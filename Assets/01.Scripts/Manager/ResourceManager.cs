using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    [SerializeField] private ResourceData resourceData;
    private Dictionary<ResourceType, ResourceStat> resourceDictionary = new Dictionary<ResourceType, ResourceStat>();
    private void Awake()
    {
        if(instance == null)
            instance= this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadResourceData();
    }
    private void LoadResourceData()
    {
        resourceDictionary.Clear();
        for(int i = 0; i < resourceData.resourceList.Count; i++)
        {
            ResourceStat resourceStat = resourceData.resourceList[i].Clone();
            resourceDictionary[resourceStat.ResourceType] = resourceStat;   
        }
    }
    public ResourceStat GetResourceStat(ResourceType type)
    {
        return resourceDictionary.GetValueOrDefault(type);
    }
}
