using UnityEngine;

public enum PawnTool
{
    None = 0,
    Axe,
    Pickaxe,
    Hammer,
}
public class Pawn : Unit
{
    public override UnitType Type => UnitType.Pawn;
    public UnitGather Gather { get; private set; }
    public UnitBuild Build { get; private set; }
    public UnitGatherState GatherState { get;  private set; }
    public UnitBuildState BuildState { get; private set; }

    private int carryType;
    private int isInteract;
    private int toolType;
    protected override void Awake()
    {
        base.Awake();
        Attack = GetComponent<MeleeAttack>();
        Gather = GetComponent<UnitGather>();
        Build = GetComponent<UnitBuild>();

        GatherState = new UnitGatherState(this);
        BuildState = new UnitBuildState(this);

        carryType = Animator.StringToHash("CarryType");
        isInteract = Animator.StringToHash("isInteract");
        toolType = Animator.StringToHash("ToolType");
    }
    public void SetCarryAnimation(ResourceType resourceType, bool carrying)
    {
        int carry = 0;
        if(carrying)
            carry = resourceType == ResourceType.Wood ? 1 : 2;
        animator.SetInteger(carryType, carry);   // 0 없음 1 나무 2 골드
    }
    public void ClearCarryAnimation()
    {
        animator.SetInteger(carryType, 0);
    }
    public void SetInteractAnimation(PawnTool tool)
    {
        animator.SetInteger(toolType, (int)tool);
        animator.SetBool(isInteract, true);
    }
    public void StopInteractAnimation()
    {
        animator.SetBool(isInteract, false);
        animator.SetInteger(toolType, (int)PawnTool.None);
    }
    public override void Init()
    {
        base.Init();
        ClearCarryAnimation();
        StopInteractAnimation();
    }
    public override void ReturnToPool()
    {
        Gather.SetTargetResource(null);
        Build.SetTarget(null);
        ObjectPoolManager.instance.ReturnObject("Pawn", this.gameObject);
    }
}
