using UnityEngine;
public static class RangeUtility
{
    public static float DistanceTo(Vector2 from, Transform target)
    {
        Collider2D col = target.GetComponent<Collider2D>();
        Vector2 point = col != null ? col.ClosestPoint(from) : (Vector2)target.position;

        return Vector2.Distance(from, point);
    }
    public static bool IsNear(Vector2 from, Transform target, float range)
    {
        return DistanceTo(from, target) <= range;
    }
}
