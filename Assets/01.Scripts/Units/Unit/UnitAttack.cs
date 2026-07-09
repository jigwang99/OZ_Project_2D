using UnityEngine;

public abstract class UnitAttack : MonoBehaviour
{
    private Unit unit;
    private float cooldown = 0;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }
    private void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }
    public bool CanAttack()
    {
        return cooldown <= 0;
    }
    public abstract void Attack(Unit target);
}