using UnityEngine;
public enum EnemyPhase
{
    Gather,
    Build,
    Combat,
}
public class EnemyContext
{
    public Faction Faction { get; }
    public Faction PlayerFaction { get; }
    public Transform Transform { get; }

   public EnemyPhase CurrentPhase { get; set; }

    public EnemyContext(Faction faction, Faction playerFaction, Transform transform)
    {
        Faction = faction;
        PlayerFaction = playerFaction;
        Transform = transform;
    }
    public Vector2 GetCenter()
    {
        Castle castle = Castle.FindNearestCastle(Transform.position, FactionType.Enemy);
        return castle != null ? (Vector2)castle.transform.position : (Vector2)Transform.position;
    }
}
