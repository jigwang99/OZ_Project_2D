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
            {
                Debug.Log("xxx");
            }
            unit.transform.position = spawn.spawnPoint.position;

            UnitStat unitStat = UnitManager.instance.GetUnitStat(spawn.unitType);
            Player.instance.TryIncreasePopulation(unitStat.Population);
        }
    }
}
