using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ProductionRule
{
    public UnitType unitType;
    public int maxCount;
    public int priority;
    public List<EnemyPhase> phases = new List<EnemyPhase>();
}

[Serializable]
public class ProductionSetting
{
    public List<ProductionRule> rules = new List<ProductionRule>();
}
public class EnemyProductionModule : IEnemyModule
{
    private readonly EnemyContext enemyContext;
    private readonly ProductionSetting setting;

    public EnemyProductionModule(EnemyContext enemyContext, ProductionSetting setting)
    {
        this.enemyContext = enemyContext;
        this.setting = setting;
    }
    public void Update()
    {
        HandleProduction();
    }
    private void HandleProduction()
    {
        foreach (ProductionRule rule in setting.rules.OrderBy(r => r.priority))
        {
            if (!rule.phases.Contains(enemyContext.CurrentPhase))
                continue;

            int current = enemyContext.Faction.CountUnits(rule.unitType) + QueueCount(rule.unitType);
            if (current >= rule.maxCount)
                continue;

            ProductionBuilding building = FindProducer(rule.unitType);
            if (building == null)
                continue;

            building.EnqueueUnit(rule.unitType);
            break;
        }
    }
    private int QueueCount(UnitType type)  //생산큐에있는 유닛 수
    {
        int count = 0;
        foreach (Building building in enemyContext.Faction.Buildings)
        {
            if (building is ProductionBuilding pb)
            {
                foreach (UnitType queued in pb.ProductList)
                    if (queued == type)
                        count++;
            }
        }
        return count;
    }
    private ProductionBuilding FindProducer(UnitType type)
    {
        ProductionBuilding best = null;
        int minQueue = int.MaxValue;

        foreach (Building building in enemyContext.Faction.Buildings)
        {
            if (!(building is ProductionBuilding productionBuilding) || !productionBuilding.IsAlive)
                continue;
            if (productionBuilding.IsQueueFull)
                continue;
            if (!productionBuilding.CanProduceType(type))
                continue;

            if (productionBuilding.ProductList.Count < minQueue)
            {
                minQueue = productionBuilding.ProductList.Count;
                best = productionBuilding;
            }
        }
        return best;
    }
}