using System.Collections.Generic;
using UnityEngine;
public class UnitDataLoader : MonoBehaviour
{
    public static UnitDataLoader instance;

    [SerializeField] private UnitData unitData;
    private Dictionary<UnitType, UnitStat> unitDictionary  = new Dictionary<UnitType, UnitStat>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        LoadUnitData();
    }
    private void LoadUnitData()
    {
        unitDictionary.Clear();
        for(int i = 0; i < unitData.unitList.Count; i++)
        {
            UnitStat unitStat = unitData.unitList[i].Clone();
            unitDictionary[unitStat.UnitType] = unitStat;
        }
    }
    public UnitStat GetUnitStat(UnitType unitType)
    {
        return unitDictionary.GetValueOrDefault(unitType);
    }
}
