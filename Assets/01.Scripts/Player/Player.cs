using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] private List<Unit> selectUnitList = new List<Unit>();
    [SerializeField] Camera camera;
    [SerializeField] LayerMask enemyLayerMask;

    private const float spacing = 1.1f;
    
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
}
