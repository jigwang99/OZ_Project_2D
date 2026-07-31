using UnityEngine;

public static class MapBounds
{
    public static bool Contains(Vector2 center, Vector2 size)
    {
        if (CameraManager.instance == null)
            return true;

        Vector2 half = size * 0.5f;
        Vector2 min = CameraManager.instance.MapMin;
        Vector2 max = CameraManager.instance.MapMax;

        return center.x - half.x >= min.x
            && center.x + half.x <= max.x
            && center.y - half.y >= min.y
            && center.y + half.y <= max.y;
    }
}
