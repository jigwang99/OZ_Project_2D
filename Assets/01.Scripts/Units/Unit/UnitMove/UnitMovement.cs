using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Unit unit;
    private float moveSpeed;
    private float waypointReachedDistance = 0.15f;

    private List<Vector2> path = new List<Vector2>();
    private int targetIndex;

    public bool HasArrived { get; private set; } = true;
    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        unit = GetComponent<Unit>();
    }
    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }
    public void SetDestination(Vector2 destination)
    {
        path = PathFinder.FindPath(rb.position, destination);
        targetIndex = 0;
        HasArrived = (path == null || path.Count == 0);
    }
    public void SetDestinationNear(Transform target, float offset = 0.5f)
    {
        Collider2D targetCol = target.GetComponent<Collider2D>();
        Vector2 myPos = rb.position;

        Vector2 point;
        if (targetCol != null)
        {
            Vector2 closest = targetCol.ClosestPoint(myPos);
            Vector2 disNear = (myPos - closest).normalized;
            point = closest + disNear * offset;
        }
        else
            point = target.position;
        SetDestination(point);
    }
    public void Move()
    {
        if (HasArrived || path == null || targetIndex >= path.Count) return;

        Vector2 currentPos = rb.position;
        Vector2 targetPoint = path[targetIndex];
        Vector2 toTarget = targetPoint - currentPos;

        if (toTarget.magnitude <= waypointReachedDistance)
        {
            targetIndex++;
            if (targetIndex >= path.Count)
            {
                Stop();
                HasArrived = true;
                return;
            }
            targetPoint = path[targetIndex];
            toTarget = targetPoint - currentPos;
        }

        Vector2 direction = toTarget.normalized;
        unit.FlipSprite(direction.x);
        Vector2 newPos = currentPos + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
        path = null;
    }
}