using System.Collections.Generic;
using UnityEngine;

public class EnemyCommander : MonoBehaviour
{
    [Header("판단 주기")]
    [SerializeField] private float thinkInterval = 1.0f;

    [Header("페이즈 분리")]
    [SerializeField] private int buildCount = 200;
    [SerializeField] private int combatCount = 15;

    [Header("자원")]
    [SerializeField] private GatherSetting gatherSetting = new GatherSetting();

    [Header("생산")]
    [SerializeField] private ProductionSetting productionSetting = new ProductionSetting();

    [Header("건설")]
    [SerializeField] private ConstructionSetting constructionSetting = new ConstructionSetting();

    [Header("전투")]
    [SerializeField] private CombatSetting combatSetting = new CombatSetting();

    private EnemyContext context;
    private readonly List<IEnemyModule> modules = new List<IEnemyModule>();

    private float thinkTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Faction faction = FactionManager.instance?.Enemy;
        Faction playerFaction = FactionManager.instance?.Player;

        context = new EnemyContext(faction, playerFaction, transform);
        context.CurrentPhase = EnemyPhase.Gather;

        modules.Add(new EnemyGatherModule(context, gatherSetting));
        modules.Add(new EnemyProductionModule(context, productionSetting));
        modules.Add(new EnemyContructionModule(context, constructionSetting));
        modules.Add(new EnemyCombatModule(context, combatSetting));
    }

    // Update is called once per frame
    void Update()
    {
        thinkTimer -= Time.deltaTime;
        if (thinkTimer > 0f)
            return;
        thinkTimer = thinkInterval;

        context.CurrentPhase = UpdateEnemyPhase();

        for(int i = 0; i < modules.Count; i++)
            modules[i].Update();
    }
    private EnemyPhase UpdateEnemyPhase()
    {
        switch (context.CurrentPhase)
        {
            case EnemyPhase.Gather:
                return context.Faction.Wood >= buildCount ? EnemyPhase.Build : EnemyPhase.Gather;
            case EnemyPhase.Build:
                return CountCombatUnits() > combatCount ? EnemyPhase.Combat : EnemyPhase.Build;
            default:
                return context.CurrentPhase;
        }
    }
    private int CountCombatUnits()
    {
        return context.Faction.CountUnits(UnitType.Lancer) + context.Faction.CountUnits(UnitType.Archer) + context.Faction.CountUnits(UnitType.Monk) + context.Faction.CountUnits(UnitType.Warrior);
    }
}
