using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    private Dictionary<(FactionType, BuildingType), int> playerBuildingCount = new Dictionary<(FactionType, BuildingType), int>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void RegisterBuilding(FactionType factionType, BuildingType buildingType)
    {
        var key = (factionType, buildingType);

        playerBuildingCount.TryGetValue(key, out int count);
        playerBuildingCount[key] = count + 1;
    }
    public void UnregisterBuilding(FactionType factionType, BuildingType buildingType)
    {
        var key = (factionType, buildingType);
        if (!playerBuildingCount.TryGetValue(key, out int count))
            return;
        playerBuildingCount[key] = Mathf.Max(0, count - 1);
    }
    public bool HasBuilding(FactionType factionType, BuildingType buildingType)
    {
        return playerBuildingCount.TryGetValue((factionType, buildingType), out int count) && count > 0;
    }
}
