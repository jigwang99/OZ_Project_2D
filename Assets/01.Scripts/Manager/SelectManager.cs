using UnityEngine;
using UnityEngine.InputSystem;

public class SelectManager : MonoBehaviour
{
    public static SelectManager instance;

    [SerializeField] private RectTransform selectBox;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Camera camera;

    // selectBox Vector
    private Vector2 startPos;
    private Vector2 currentPos;
    private Vector2 min;
    private Vector2 max;

    // OverlapBoxAll Vector
    private Vector3 worldMin;
    private Vector3 worldMax;
    private Vector2 center;
    private Vector2 size;

    private Collider2D[] hits;

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
        // 드래그 시작
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {   
            startPos = Mouse.current.position.ReadValue();
            selectBox.gameObject.SetActive(true);
        }
        // 드래그 중
        if (Mouse.current.leftButton.isPressed)
        {
            currentPos = Mouse.current.position.ReadValue();

            min = Vector2.Min(startPos, currentPos);
            max = Vector2.Max(startPos, currentPos);

            selectBox.anchoredPosition = min;
            selectBox.sizeDelta = max - min;
        }
        // 드래그 종료
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            selectBox.gameObject.SetActive(false);
            Player.instance.ClearSelectList();

            worldMin = camera.ScreenToWorldPoint(min);
            worldMax = camera.ScreenToWorldPoint(max);

            center = (worldMin + worldMax) * 0.5f;
            size = new Vector2(
                Mathf.Abs(worldMax.x - worldMin.x),
                Mathf.Abs(worldMax.y - worldMin.y)
                );

            hits = Physics2D.OverlapBoxAll(center, size, 0f, layerMask);

            foreach(Collider2D hit in hits)
            {
                Unit unit = hit.GetComponent<Unit>();

                if (unit != null)
                {
                    Player.instance.SelectUnit(unit);
                }
            }
        }
    }
}
