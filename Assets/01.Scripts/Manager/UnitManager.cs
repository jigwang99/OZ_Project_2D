using System.Collections.Generic;
using UnityEngine;
public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

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
