using System.Collections.Generic;
using UnityEngine;

public static class PathFinder
{
    private static int currentVersion = 0;
    private static Heap<Node> openSet;

    private static void NodeInit(Node node)
    {
        if(node.searchVersion != currentVersion)
        {
            node.searchVersion = currentVersion;
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
            node.closed = false;
        }
    }
    public static List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        if(GridManager.instance == null)
        {
            return null;
        }
        if (openSet == null || openSet.Capacity < GridManager.instance.MaxSize)
            openSet = new Heap<Node>(GridManager.instance.MaxSize);
        else
            openSet.Clear();

        currentVersion++;

        Node startNode = GridManager.instance.NodeFromWorldPoint(startPos);
        Node targetNode = GridManager.instance.NodeFromWorldPoint(targetPos);

        // 목표지점이 장애물이라면 근처 노드로 이동
        if(!targetNode.walkable)
        {
            targetNode = FindNearestWalkableNode(targetNode);
            if (targetNode == null)
                return null;
        }
       
        NodeInit(startNode);
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // FCost가 가장 낮은 노드 추출
            Node currentNode = openSet.RemoveFirst();
            currentNode.closed = true;

            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);

            foreach(Node neighbour in GridManager.instance.GetNeighbours(currentNode))
            {
                NodeInit(neighbour);

                if (!neighbour.walkable || neighbour.closed)
                    continue;

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if(newCostToNeighbour < neighbour.gCost)
                {
                    bool isOpen = neighbour.gCost != int.MaxValue;

                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if(!isOpen)
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
        GridManager grid = GridManager.instance;
        int maxRadius = Mathf.Max(grid.SizeX, grid.SizeY);

        for(int radius = 1; radius < maxRadius; radius++)
        {
            for(int x = -radius; x <= radius; x++)
            {
                for(int y = -radius; y <= radius; y++)
                {
                    if (Mathf.Abs(x) != radius && Mathf.Abs(y) != radius)
                        continue;
                    Node node =grid.GetNode(from.gridX + x, from.gridY + y);

                    if (node != null && node.walkable)
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
            Vector2 newDirection = (path[i] - path[i - 1]).normalized;
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