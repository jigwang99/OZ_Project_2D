public class Archer : Unit
{
    public override UnitType Type => UnitType.Archer;
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<RangedAttack>();
    }
}
