using System.Collections.Generic;
using UnityEngine;

public static class PathFinder
{
    public static List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        if(GridManager.instance == null)
        {
            Debug.Log("그리드 매니저가 없음");
            return null;
        }

        Node startNode = GridManager.instance.NodeFromWorldPoint(startPos);
        Node targetNode = GridManager.instance.NodeFromWorldPoint(targetPos);

        // 목표지점이 장애물이라면 근처 노드로 이동
        if(!targetNode.walkable)
        {
            targetNode = FindNearestWalkableNode(targetNode);
            if (targetNode == null)
                return null;
        }

        Heap<Node> openSet = new Heap<Node>(GridManager.instance.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
       
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        startNode.parent = null;

        while (openSet.Count > 0)
        {
            // FCost가 가장 낮은 노드 추출
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);

            foreach(Node neighbour in GridManager.instance.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if(newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }
        return null;
    }
    // 목표지점이 장애물일 경우 가장 가까운 곳 반환
    private static Node FindNearestWalkableNode(Node from)
    {
        for(int radius = 1; radius < 20; radius++)
        {
            for(int x = -radius; x <= radius; x++)
            {
                for(int y = -radius; y <= radius; y++)
                {
                    Node node = GridManager.instance.NodeFromWorldPoint(from.worldPos + new Vector2(x, y) * 0.5f);

                    if (node.walkable)
                        return node;
                }
            }
        }
        return null;
    }
    // 경로 역추적
    private static List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode.worldPos);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return SimplifyPath(path);
    }
    private static List<Vector2> SimplifyPath(List<Vector2> path)
    {
        if (path.Count < 3)
            return path;

        List<Vector2> simplified = new List<Vector2> { path[0] };
        Vector2 lastDirection = (path[1] - path[0]).normalized;

        for(int i  = 1; i < path.Count - 1; i++)
        {
            Vector2 newDirection = (path[i - 1] - path[i]).normalized;
            if (Vector2.Distance(newDirection, lastDirection) > 0.01f)
            {
                simplified.Add(path[i]);
                lastDirection = newDirection;
            }
        }
        simplified.Add(path[path.Count - 1]);
        return simplified;
    }
    private static int GetDistance(Node a, Node b)
    {
        int distanceX = Mathf.Abs(a.gridX - b.gridX);
        int distanceY = Mathf.Abs(a.gridY - b.gridY);

        // sqrt2 = 1.4 => 14   1 => 10
        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
