using UnityEngine;

public static class Formation
{
    public static Vector2 Offset(int index, int count, float spacing)
    {
        int column = Mathf.CeilToInt(Mathf.Sqrt(count));
        int row = Mathf.CeilToInt((float)count / column);
        int x = index % column;
        int y = index / column;

        return new Vector2(
            (x - (column - 1) * 0.5f) * spacing,
            ((row - 1) * 0.5f - y) * spacing);
    }
}
