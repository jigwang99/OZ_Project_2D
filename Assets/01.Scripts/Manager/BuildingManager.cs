using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    private Dictionary<BuildingType, int> playerBuildingCount = new Dictionary<BuildingType, int>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void RegisterPlayerBuilding(BuildingType type)
    {
        playerBuildingCount.TryGetValue(type, out int count);
        playerBuildingCount[type] = count + 1;
    }
    public void UnregisterPlayerBuilding(BuildingType type)
    {
        if (!playerBuildingCount.TryGetValue(type, out int count))
            return;
        playerBuildingCount[type] = Mathf.Max(0, count - 1);
    }
    public bool HasPlayerBuilding(BuildingType type)
    {
        return playerBuildingCount.TryGetValue(type, out int count) && count > 0;
    }
}
