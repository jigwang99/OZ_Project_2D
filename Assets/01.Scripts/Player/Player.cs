using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] private List<Unit> selectUnitList = new List<Unit>();
    [SerializeField] Camera camera;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] LayerMask resourceLayerMask;

    private const float spacing = 1.1f;
    
    
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
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            // 선택한 유닛이 없을 경우
            if (selectUnitList == null || selectUnitList.Count == 0)
                return;
            Vector2 worldPos = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Collider2D hit = Physics2D.OverlapCircle(worldPos, 0.2f, enemyLayerMask);
            Unit target = hit != null ? hit.GetComponent<Unit>() : null;
            
            if (target != null && target.IsAlive)
            {
                foreach(Unit unit in selectUnitList)
                {
                    unit.Attack.SetTarget(target);
                    unit.StateMachine.ChangeState(unit.ChaseState);
                }
                return;
            }

            Vector2 destination = worldPos;

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

            Collider2D resourceHit = Physics2D.OverlapCircle(worldPos, 0.2f, resourceLayerMask);
            Resource resource = resourceHit != null ? resourceHit.GetComponent<Resource>() : null;

            if(resource != null && !resource.IsDepleted)
            {
                foreach(Unit unit in selectUnitList)
                {
                    if (!(unit is Pawn pawn))
                        continue;

                    pawn.Gather.SetTargetResource(resource);
                    pawn.Gather.SetReturnBuilding(Castle.FindNearestCastle(pawn.transform.position));
                    pawn.StateMachine.ChangeState(pawn.GatherState);
                }
                return;
            }
        }
    }
    private void FixedUpdate()
    {
        
    }
    public void SelectUnit(Unit unit)
    {
        if(!selectUnitList.Contains(unit))
            selectUnitList.Add(unit);
    }
    public void ClearSelectList()
    {
        selectUnitList.Clear();
    }
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
