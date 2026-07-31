public enum Layer
{
    Player = 6,
    Enemy = 7,
    PlayerProjectile = 8,
    EnemyProjectile = 9,
    PlayerBuilding = 10,
    EnemyBuilding = 11,
}

public static class LayerUtility
{
    public static bool IsPlayerSide(int layer)
    {
        return layer == (int)Layer.Player || layer == (int)Layer.PlayerBuilding;
    }
    public static bool IsPlayerUnit(int layer)
    {
        return layer == (int)Layer.Player;
    }
    public static bool IsPlayerBuilding(int layer)
    {
        return layer == (int)Layer.PlayerBuilding;
    }
}
