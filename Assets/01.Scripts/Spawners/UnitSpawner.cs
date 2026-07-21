using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    [Serializable]
    private struct SpawnUnit
    {
        public UnitType unitType;
        public Transform spawnPoint;
        public Layer layer;
    }
    [SerializeField] private List<SpawnUnit> spawnList = new List<SpawnUnit>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnAll();
    }
    private void SpawnAll()
    {
        foreach (SpawnUnit spawn in spawnList)
        {
            Unit unit = ObjectPoolManager.instance.GetObject<Unit>(spawn.unitType.ToString());

            if (unit == null)
                continue;

            unit.SetLayer(spawn.layer);
            unit.transform.position = spawn.spawnPoint.position;

            if(spawn.layer == Layer.Player)
            {
                UnitStat unitStat = UnitDataLoader.instance.GetUnitStat(spawn.unitType);
                Player.instance.TryIncreasePopulation(unitStat.Population);
            }
            
        }
    }
}
