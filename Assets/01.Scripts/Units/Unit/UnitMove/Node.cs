using UnityEngine;

/// <summary>
/// A* 경로 탐색에 사용하는 그리드 노드
/// </summary>
public class Node : IHeapItem<Node>
{
    public bool walkable;          // 해당 칸으로 이동 가능 여부
    public Vector2 worldPos;
    public int gridX, gridY;       // 그리드 상에서 인덱스

    public int gCost;              // 시작 노드부터 현재 노드까지의 실제 비용
    public int hCost;              // 현재 노드부터 목표 노드 까지의 예상 비용
    public Node parent;            // 이전 노드

    public int searchVersion = 0;
    public bool closed;

    public int FCost => gCost + hCost;
    
    public int HeapIndex { get; set; }

    public Node(bool walkable, Vector2 worldPos, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPos = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
    }
    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if(compare == 0)
            compare = hCost.CompareTo(other.hCost);

        // Heap은 CompareTo > 0 일 때 우선순위가 높음
        return -compare;
    }
}
