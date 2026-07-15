using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] private List<Unit> selectUnitList = new List<Unit>();
    [SerializeField] private Building selectBuilding;
    [SerializeField] Camera camera;
    [SerializeField] LayerMask allyLayerMask;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] LayerMask resourceLayerMask;

    private const float spacing = 1.1f;

    private static readonly Key[] productionKeys = { Key.A, Key.S, Key.D, Key.F };
    

    public int Wood {  get; private set; }
    public int Gold { get; private set; }
    public event Action OnResourceChanged;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Wood = 50;
        Gold = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleProduckKeys();
        HandleRightClick();
    }
    private void FixedUpdate()
    {
        
    }
    private void HandleProduckKeys()
    {
        if (!(selectBuilding is ProductionBuilding productionBuilding))
            return;
        if (!productionBuilding.IsAlive)
            return;

        for(int i = 0; i < productionKeys.Length; i++)
        {
            if (Keyboard.current[productionKeys[i]].wasPressedThisFrame)
                productionBuilding.EnqueueUnitByIndex(i);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            productionBuilding.CancelLastProduct();
    }
    private void HandleRightClick()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;
        if (selectUnitList == null || selectUnitList.Count == 0)
            return;

        Vector2 worldPos = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // 유닛공격(공격없는 유닛은 이동)
        Collider2D hit = Physics2D.OverlapCircle(worldPos, 0.2f, enemyLayerMask);
        Unit target = hit != null ? hit.GetComponent<Unit>() : null;

        if (target != null && target.IsAlive)
        {
            List<Unit> notCombatUnits = new List<Unit>();
            foreach (Unit unit in selectUnitList)
            {
                if(unit.Attack == null)
                {
                    notCombatUnits.Add(unit);
                    continue;
                }
                unit.Attack.SetTarget(target);
                unit.StateMachine.ChangeState(unit.ChaseState);
            }
            if (notCombatUnits.Count > 0)
                MoveUnits(notCombatUnits, worldPos);
            return;
        }

        // 유닛 힐(Monk만, 나머지 이동)
        Collider2D allyHit = Physics2D.OverlapCircle(worldPos, 0.2f, allyLayerMask);
        Unit ally = allyHit != null ? allyHit.GetComponent<Unit>() : null;

        if(ally != null && ally.IsAlive)
        {
            List<Unit> moveUnits = new List<Unit>();

            foreach (Unit unit in selectUnitList)
            {
                if(unit is Monk monk && unit != ally)
                {
                    monk.Heal.SetTarget(ally);
                    monk.StateMachine.ChangeState(monk.HealState);
                }
                else
                {
                    moveUnits.Add(unit);
                }
            }
            if (moveUnits.Count > 0)
                MoveUnits(moveUnits, worldPos);
            return;
        }
        
        // 자원채집 (Pawn)
        Collider2D resourceHit = Physics2D.OverlapCircle(worldPos, 0.2f, resourceLayerMask);
        Resource resource = resourceHit != null ? resourceHit.GetComponent<Resource>() : null;

        if (resource != null && !resource.IsDepleted)
        {
            foreach (Unit unit in selectUnitList)
            {
                if (!(unit is Pawn pawn))
                    continue;

                pawn.Gather.SetTargetResource(resource);
                pawn.Gather.SetReturnBuilding(Castle.FindNearestCastle(pawn.transform.position));
                pawn.StateMachine.ChangeState(pawn.GatherState);
            }
            return;
        }

        MoveUnits(selectUnitList, worldPos);
    }
    // 유닛 이동
    private void MoveUnits(List<Unit> units, Vector2 destination)
    {
        if (units.Count == 0)
            return;

        int column = Mathf.CeilToInt(Mathf.Sqrt(selectUnitList.Count));
        int row = Mathf.CeilToInt((float)selectUnitList.Count / column);
        for (int i = 0; i < selectUnitList.Count; i++)
        {
            int x = i % column;
            int y = i / column;

            Vector2 offset = new Vector2(
                (x - (column - 1) * 0.5f) * spacing,
                ((row - 1) * 0.5f - y) * spacing);

            Unit unit = selectUnitList[i];

            unit.Movement.SetDestination(destination + offset);

            // 이동 중이라면 상태변화 없음
            if (unit.StateMachine.CurrentState != unit.MoveState)
            {
                unit.StateMachine.ChangeState(unit.MoveState);
            }
        }
    }
    // 유닛선택
    public void SelectUnit(Unit unit)
    {
        if(!selectUnitList.Contains(unit))
            selectUnitList.Add(unit);
    }
    public void DeselectUnit(Unit unit)
    {
        if(selectUnitList.Contains(unit))
            selectUnitList.Remove(unit);
    }
    public void ClearSelectList()
    {
        foreach (Unit unit in selectUnitList)
            unit.SetSelected(false);
        selectUnitList.Clear();
    }
    // 건물선택
    public void SelectBuilding(Building building)
    {
        ClearSelectList();
        DeselectBuilding();

        selectBuilding = building;
        building.SetSelected(true);
    }
    public void DeselectBuilding()
    {
        if (selectBuilding != null)
            selectBuilding.SetSelected(false);
        selectBuilding = null;
    }
    // 자원
    public void AddResource(ResourceType resourceType, int amount)
    {
        switch(resourceType)
        {
            case ResourceType.Wood:
                AddWood(amount);
                break;
            case ResourceType.Gold:
                AddGold(amount); 
                break;
        }    
    }
    public void AddWood(int amount)
    {
        Wood += amount;
        OnResourceChanged?.Invoke();
    }
    public void AddGold(int amount)
    {
        Gold += amount;
        OnResourceChanged?.Invoke();
    }
    public bool TryReduceResource(int woodCost, int goldCost)
    {
        if (Wood < woodCost || Gold < goldCost)
            return false;

        Wood -= woodCost;
        Gold -= goldCost;
        return true;
    }
}
