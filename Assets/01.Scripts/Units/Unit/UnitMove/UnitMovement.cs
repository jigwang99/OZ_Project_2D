using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [Header("끼임 처리")]
    [SerializeField] private float stuckCheckTime = 0.5f;
    [SerializeField] private float stuckDistance = 0.1f;
    [SerializeField] private float escapeDistance = 1.0f;
    [SerializeField] private float escapeTimeout = 1.0f;
    [SerializeField] private float blockCheckRadius = 0.3f;
    [SerializeField] private LayerMask blockLayerMask;

    [Header("Audio")]
    [SerializeField] private AudioClip footStepClip;
    [SerializeField] private float footStepInterval = 0.3f;
    private float footStepTimer;

    private const float destinationChangeThreshold = 0.3f;

    private static Vector2[] escapeDirection = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private Rigidbody2D rb;
    private Unit unit;
    private float moveSpeed;
    private float waypointReachedDistance = 0.15f;

    private List<Vector2> path = new List<Vector2>();
    private int targetIndex;

    private Vector2 finalDestination;
    private bool hasFinalDestination;

    private Vector2 lastCheckPosition;
    private float stuckTimer;

    private bool isEscaping;
    private Vector2 escapeTarget;
    private float escapeTimer;

    public bool HasArrived { get; private set; } = true;
    public bool IsEscaping => isEscaping;
    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        unit = GetComponent<Unit>();
    }
    public void OnEnable()
    {
        Stop();
        HasArrived = true;
        hasFinalDestination = false;
    }
    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }
    public void SetDestination(Vector2 destination)
    {
        bool changed = !hasFinalDestination || Vector2.Distance(destination, finalDestination) > destinationChangeThreshold;

        finalDestination = destination;
        hasFinalDestination = true;

        if (isEscaping && !changed)
            return;

        isEscaping = false;

        path = PathFinder.FindPath(rb.position, destination);
        targetIndex = 0;
        HasArrived = (path == null || path.Count == 0);

        if (HasArrived)
            rb.linearVelocity = Vector2.zero;

        if (changed)
            ResetStuck();
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
        PlayFootStep();

        if (isEscaping)
        {
            MoveEscape();
            return;
        }
        if (HasArrived || IsArrived())
            return;

        Vector2 toTarget = path[targetIndex] - rb.position;
        Vector2 direction = toTarget.normalized;

        unit.FlipSprite(direction.x);

        float speed = moveSpeed;
        if (targetIndex == path.Count - 1)
            speed = Mathf.Min(moveSpeed, toTarget.magnitude / Time.fixedDeltaTime);

        rb.linearVelocity = direction * speed;

        CheckStuck();
    }
    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
        path = null;
        targetIndex = 0;
        isEscaping = false;
        stuckTimer = 0f;
        footStepTimer = 0f;
    }
    private void PlayFootStep()
    {
        if (footStepClip == null)
            return;
        if(rb.linearVelocity.sqrMagnitude < 0.01f)
        {
            footStepTimer = 0f;
            return;
        }
        footStepTimer -= Time.fixedDeltaTime;
        if(footStepTimer <= 0f)
        {
            AudioManager.instance?.PlaySFXAt(footStepClip, rb.position);
            footStepTimer = footStepInterval;
        }
    }
    #region stuck
    private void ResetStuck()
    {
        stuckTimer = 0f;
        lastCheckPosition = rb.position;
    }
    private void CheckStuck()
    {
        stuckTimer += Time.fixedDeltaTime;
        if (stuckTimer < stuckCheckTime)
            return;
        if (Vector2.Distance(rb.position, lastCheckPosition) < stuckDistance)
        {
            BeginEscape();
        }

        lastCheckPosition = rb.position;
        stuckTimer = 0f;
    }
    private void BeginEscape()
    {
        Vector2 origin = rb.position;
        Vector2 best = Vector2.zero;

        float bestDistance = float.MaxValue;
        bool found = false;

        int start = Random.Range(0, escapeDirection.Length);

        for (int i = 0; i < escapeDirection.Length; i++)
        {
            Vector2 dir = escapeDirection[(start + i) % escapeDirection.Length];
            Vector2 candidate = origin + dir * escapeDistance;

            if (!CanMoveTo(candidate))
                continue;

            float distance = hasFinalDestination ? Vector2.Distance(candidate, finalDestination) : 0f;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
                found = true;
            }
        }

        if (!found)
            return;

        isEscaping = true;
        escapeTarget = best;
        escapeTimer = escapeTimeout;
        rb.linearVelocity = Vector2.zero;
    }
    private bool CanMoveTo(Vector2 position)
    {
        if (GridManager.instance != null && !GridManager.instance.NodeFromWorldPoint(position).walkable)
            return false;
        if (blockLayerMask != 0 && Physics2D.OverlapCircle(position, blockCheckRadius, blockLayerMask) != null)
            return false;

        return true;
    }
    private void MoveEscape()
    {
        escapeTimer -= Time.fixedDeltaTime;

        Vector2 toTarget = escapeTarget - rb.position;

        if (escapeTimer <= 0f || toTarget.magnitude <= waypointReachedDistance)
        {
            EndEscape();
            return;
        }
        Vector2 direction = toTarget.normalized;
        unit.FlipSprite(direction.x);
        rb.linearVelocity = direction * moveSpeed;
    }
    private void EndEscape()
    {
        isEscaping = false;
        rb.linearVelocity = Vector2.zero;

        if (hasFinalDestination)
        {
            path = PathFinder.FindPath(rb.position, finalDestination);
            targetIndex = 0;
            HasArrived = (path == null || path.Count == 0);
        }
        else
            HasArrived = true;

        ResetStuck();
    }
    #endregion
}