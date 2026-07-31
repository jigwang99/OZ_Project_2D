using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager instance;

    public Faction Player {  get; private set; }
    public Faction Enemy { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Player = new Faction(FactionType.Player, 50, 0);
        Enemy = new Faction(FactionType.Enemy, 50, 0);
    }
    public Faction GetFaction(FactionType type)
    {
        return type == FactionType.Player ? Player : Enemy;
    }
    public Faction FromLayer(int layer)
    {
        bool isPlayer = layer == (int)Layer.Player || layer == (int)Layer.PlayerBuilding;
        return isPlayer ? Player : Enemy;
    }
}
