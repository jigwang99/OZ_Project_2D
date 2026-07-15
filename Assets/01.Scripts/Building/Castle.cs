using System.Collections.Generic;
using UnityEngine;

public class Castle : ProductionBuilding
{
    public override BuildingType Type => BuildingType.Castle;

    public static List<Castle> ActiveCastle = new List<Castle>();
    protected override void OnEnable()
    {
        base.OnEnable();
        ActiveCastle.Add(this);
    }
    public static Castle FindNearestCastle(Vector2 position)
    {
        Castle nearest = null;
        float minDistance = float.MaxValue;

        foreach(Castle castle in ActiveCastle)
        {
            if (!castle.IsAlive)
                continue;

            float distance = Vector2.Distance(castle.transform.position, position);
            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = castle;
            }

        }
        return nearest;
    }
    public override void ReturnToPool()
    {
        ActiveCastle.Remove(this);
        ObjectPoolManager.instance.ReturnObject("Castle", this.gameObject);
    }
}
