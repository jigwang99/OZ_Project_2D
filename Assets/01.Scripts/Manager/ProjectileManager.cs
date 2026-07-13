using UnityEngine;
using System.Collections.Generic;
public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager instance;

    [SerializeField] private ProjectileData projectileData;
    private Dictionary<ProjectileType, ProjectileStat> projectileDictionary = new Dictionary<ProjectileType, ProjectileStat>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        LoadProjectileData();
    }
    private void LoadProjectileData()
    {
        projectileDictionary.Clear();
        for(int i = 0; i < projectileData.projectileList.Count; i++)
        {
            ProjectileStat projectileStat = projectileData.projectileList[i].Clone();
            projectileDictionary[projectileStat.ProjectileType] = projectileStat;
        }
    }
    public ProjectileStat GetProjectileStat(ProjectileType type)
    {
        return projectileDictionary.GetValueOrDefault(type);
    }
}
