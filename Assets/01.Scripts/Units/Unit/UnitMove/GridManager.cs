using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A* 탐색용 그리드생성
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [SerializeField] private Vector2 gridWorldSize = new Vector2(50, 50);
    [SerializeField] private float nodeRadius = 0.5f;    // 노드 반지름
    [SerializeField] private LayerMask obstacleLayerMask;  // 장애물 레이어마스크

    private Node[,] grid;
    private float nodeDiameter;        // 노드 지름
    private int gridSizeX, gridSizeY;

    public int MaxSize => gridSizeX * gridSizeY;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        nodeDiameter = nodeRadius * 2;
        // 전체 크기를 가로세로 몇칸인지 계산
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); 
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        
        CreateGrid();
    }
    // 그리드 생성
    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        // 오브젝트 위치가 그리드 중심(0,0) 그리드 좌하단이 시작점
        Vector2 worldBottomLeft = (Vector2)transform.position
            - Vector2.right * gridWorldSize.x / 2
            - Vector2.up * gridWorldSize.y / 2;
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft
                    + Vector2.right * (x * nodeDiameter + nodeRadius)
                    + Vector2.up * (y * nodeDiameter + nodeRadius);

                bool walkable = CheckWalkable(worldPoint);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }
    // 월드 좌표를 가장 가까운 그리드 노드로 변환
    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        Vector2 localPos = worldPosition - (Vector2)transform.position + gridWorldSize / 2;
        float percentX = Mathf.Clamp01(localPos.x / gridWorldSize.x);
        float percentY = Mathf.Clamp01(localPos.y / gridWorldSize.y);

        int x = Mathf.Clamp(Mathf.RoundToInt((gridSizeX - 1) * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((gridSizeY - 1) * percentY), 0, gridSizeY - 1);
        return grid[x, y];
    }

    // 이웃 노드 (상하좌우 대각 8방향)
    public List<Node> GetNeighbours(Node node)
    {
        var neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // 자신
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    private bool CheckWalkable(Vector2 worldPoint)
    {
        return !Physics2D.OverlapCircle(worldPoint, nodeRadius * 0.9f, obstacleLayerMask);
    }
    public void UpdateArea(Vector2 center, Vector2 size)
    {
        if (grid == null)
            return;

        Vector2 half = size * 0.5f;
        Node minNode = NodeFromWorldPoint(center - half - Vector2.one * nodeDiameter);
        Node maxNode = NodeFromWorldPoint(center + half + Vector2.one * nodeDiameter);

        for (int x = minNode.gridX; x <= maxNode.gridX; x++)
            for (int y = minNode.gridY; y <= maxNode.gridY; y++)
                grid[x, y].walkable = CheckWalkable(grid[x, y].worldPos);
    }
    public bool IsAreaWalkable(Vector2 center, Vector2 size)
    {
        if(grid == null) return false;

        Vector2 half = size * 0.5f;
        Node minNode = NodeFromWorldPoint(center - half);
        Node maxNode = NodeFromWorldPoint(center + half);

        for (int x = minNode.gridX; x <= maxNode.gridX; x++)
            for (int y = minNode.gridY; y <= maxNode.gridY; y++)
                if (!grid[x, y].walkable)
                    return false;
        return true;
    }
    public Vector2 FitNode(Vector2 worldPos)
    {
        return NodeFromWorldPoint(worldPos).worldPos;
    }
    // 그리드 시각화
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = n.walkable ? new Color(1, 1, 1, 0.1f) : new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - 0.05f));
            }
        }
    }
}
