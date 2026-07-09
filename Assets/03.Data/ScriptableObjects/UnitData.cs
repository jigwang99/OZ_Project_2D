using UnityEngine;

[CreateAssetMenu(menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string unitName;
    [SerializeField] private GameObject prefab;

    [Header("Stats")]
    [SerializeField] private int maxHp;
    [SerializeField] private int attackDamage;
    [SerializeField] private int defense;
    [SerializeField] private int population;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    [Header("Production")]
    [SerializeField] private int woodCost;
    [SerializeField] private int goldCost;
    [SerializeField] private float buildTime;

    public string UnitName => unitName;
    public GameObject Prefab => prefab;

    public int MaxHp => maxHp;
    public int AttackDamage => attackDamage;
    public int Defense => defense;
    public int Population => population;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;

    public int GoldCost => goldCost;
    public int WoodCost => woodCost;
    public float BuildTime => buildTime;
}