using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [Serializable]
    private struct SpawnBuilding
    {
        public BuildingType buildingType;
        public Transform spawnPoint;
        public Layer layer;
    }
    [SerializeField] private List<SpawnBuilding> spawnList = new List<SpawnBuilding>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnAll();
    }
    private void SpawnAll()
    {
        foreach (SpawnBuilding spawn in spawnList)
        {
            Building building = ObjectPoolManager.instance.GetObject<Building>(spawn.buildingType);
            
            building.SetLayer(spawn.layer);
            building.transform.position = spawn.spawnPoint.position;
            building.SetSkipBuilded(true);
            building.Init();
        }
    }
}
