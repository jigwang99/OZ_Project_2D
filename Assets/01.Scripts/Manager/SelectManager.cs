using UnityEngine;
using UnityEngine.InputSystem;

public class SelectManager : MonoBehaviour
{
    public static SelectManager instance;

    [SerializeField] private RectTransform selectBox;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask buildingLayerMask;
    [SerializeField] private Camera camera;

    private const float drag = 10f;
    private bool isDrag;

    // selectBox Vector
    private Vector2 startPos;
    private Vector2 currentPos;
    private Vector2 min;
    private Vector2 max;

    public void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (BuildPlacer.instance != null && BuildPlacer.instance.IsPlacing)
            return;

        // 드래그 시작
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {   
            startPos = Mouse.current.position.ReadValue();
            isDrag = false;
        }
        // 드래그 중
        if (Mouse.current.leftButton.isPressed)
        {
            currentPos = Mouse.current.position.ReadValue();

            if(!isDrag && Vector2.Distance(startPos, currentPos) > drag)
            {
                isDrag = true;
                selectBox.gameObject.SetActive(true);
            }
            if(isDrag)
            {
                min = Vector2.Min(startPos, currentPos);
                max = Vector2.Max(startPos, currentPos);

                selectBox.anchoredPosition = min;
                selectBox.sizeDelta = max - min;
            }
        }
        // 드래그 종료
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            selectBox.gameObject.SetActive(false);
            Player.instance.ClearSelectList();

            if (isDrag)
                DragSelect();
            else
                ClickSelect();
            
            isDrag=false;
        }
    }
    private void DragSelect()
    {
        Vector3 worldMin = camera.ScreenToWorldPoint(min);
        Vector3 worldMax = camera.ScreenToWorldPoint(max);

        Vector2 center = (worldMin + worldMax) * 0.5f;
        Vector2 size = new Vector2(
            Mathf.Abs(worldMax.x - worldMin.x),
            Mathf.Abs(worldMax.y - worldMin.y)
            );

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, layerMask);

        foreach (Collider2D hit in hits)
        {
            Unit unit = hit.GetComponent<Unit>();

            if (unit != null)
            {
                Player.instance.SelectUnit(unit);
            }
        }
    }
    private void ClickSelect()
    {
        Vector2 worldPos = camera.ScreenToWorldPoint(startPos);
        Collider2D unitHit = Physics2D.OverlapPoint(worldPos, layerMask);

        Unit unit = unitHit != null ? unitHit.GetComponent<Unit>() : null;

        if (unit != null && unit.IsAlive)
        {
            Player.instance.SelectUnit(unit);
        }

        Collider2D buildingHit = Physics2D.OverlapPoint(worldPos, buildingLayerMask);
        Building building = buildingHit != null ? buildingHit.GetComponent<Building>() : null;

        if(building != null && building.IsAlive)
        {
            Player.instance.SelectBuilding(building);
        }
    }
}
