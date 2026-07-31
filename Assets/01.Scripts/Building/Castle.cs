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
    public static Castle FindNearestCastle(Vector2 position, FactionType factionType)
    {
        Castle nearest = null;
        float minDistance = float.MaxValue;

        foreach(Castle castle in ActiveCastle)
        {
            if (!castle.IsAlive)
                continue;
            if (castle.OwnerFaction.Type != factionType)
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
    public static int CountCastles(FactionType factionType)
    {
        int count = 0;
        for(int i = 0; i < ActiveCastle.Count; i++)
        {
            Castle castle = ActiveCastle[i];
            if (castle != null && castle.IsAlive && castle.OwnerFaction != null && castle.OwnerFaction.Type == factionType)
                count++;
        }
        return count;
    }
    public override void ReturnToPool()
    {
        ActiveCastle.Remove(this);
        base.ReturnToPool();
    }
}
