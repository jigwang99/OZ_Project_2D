using System;
using System.Collections.Generic;
using UnityEngine;

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

    public ProjectileStat(ProjectileType projectileType, float speed, float lifeTime)
    {
        this.projectileType = projectileType;
        this.speed = speed;
        this.lifeTime = lifeTime;
    }
    public ProjectileStat Clone()
    {
        return new ProjectileStat(projectileType, speed, lifeTime);
    }
}

[CreateAssetMenu(fileName = "ProjectileData", menuName = "RTS/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    public List<ProjectileStat> projectileList = new List<ProjectileStat>();
}
