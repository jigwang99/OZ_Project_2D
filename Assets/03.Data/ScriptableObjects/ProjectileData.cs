using System;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Arrow,
}
[Serializable]
public class ProjectileStat
{
    [Header("Info")]
    [SerializeField] private ProjectileType projectileType;

    [Header("Stat")]
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    public ProjectileType ProjectileType => projectileType;
    public float Speed => speed;
    public float LifeTime => lifeTime;
}

[CreateAssetMenu(fileName = "ProjectileData", menuName = "RTS/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    public List<ProjectileStat> projectileList = new List<ProjectileStat>();
}
