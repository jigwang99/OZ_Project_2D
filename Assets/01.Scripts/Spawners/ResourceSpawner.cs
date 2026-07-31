using System.Collections.Generic;
using System;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Serializable]
    private struct SpawnResource
    {
        public ResourceType resourceType;
        public Transform spawnPoint;
    }

    [SerializeField] private List<SpawnResource> spawnList = new List<SpawnResource>();

    private void Start()
    {
        SpawnAll();
    }
    private void SpawnAll()
    {
        foreach(SpawnResource spawn in spawnList)
        {
            Resource resource = ObjectPoolManager.instance.GetObject<Resource>(spawn.resourceType);
            resource.transform.position = spawn.spawnPoint.position;
        }
    }
}
