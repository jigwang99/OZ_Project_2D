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
    public void OnEnable()
    {
        Stop();
        HasArrived = true;
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

        if (HasArrived)
            rb.linearVelocity = Vector2.zero;
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
    public bool IsArrived()
    {
        if (path == null || targetIndex >= path.Count)
        {
            Stop();
            HasArrived = true;
            return true;
        }

        float reach = Mathf.Max(waypointReachedDistance, moveSpeed * Time.fixedDeltaTime);
        float sqrReach = reach * reach;
        Vector2 currentPos = rb.position;

        while (targetIndex < path.Count && (path[targetIndex] - currentPos).sqrMagnitude <= sqrReach)
            targetIndex++;

        if(targetIndex >=  path.Count)
        {
            Stop();
            HasArrived= true;
            return true;
        }
        return false;
    }
    public void Move()
    {
        if (HasArrived || IsArrived())
            return;

        Vector2 toTarget = path[targetIndex] - rb.position;
        Vector2 direction = toTarget.normalized;

        unit.FlipSprite(direction.x);

        float speed = moveSpeed;
        if (targetIndex == path.Count - 1)
            speed = Mathf.Min(moveSpeed, toTarget.magnitude / Time.fixedDeltaTime);

        rb.linearVelocity = direction * speed;

    }
    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
        path = null;
        targetIndex = 0;
    }
}