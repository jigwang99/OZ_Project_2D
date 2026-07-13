using UnityEngine;
using System.Collections.Generic;
public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    [SerializeField] private BuildingData buildingData;
    private Dictionary<BuildingType, BuildingStat> buildingDictionary = new Dictionary<BuildingType, BuildingStat> ();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadBuildingData();
    }
    private void LoadBuildingData()
    {
        buildingDictionary.Clear();
        for(int i = 0; i < buildingData.buildingList.Count; i++)
        {
            BuildingStat buildingStat = buildingData.buildingList[i].Clone();
            buildingDictionary[buildingStat.BuildingType] = buildingStat;
        }
    }
    public BuildingStat GetBuildingStat(BuildingType type)
    {
        return buildingDictionary.GetValueOrDefault(type);
    }
}
