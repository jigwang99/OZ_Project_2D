using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildPlacer : MonoBehaviour
{
    public static BuildPlacer instance;
    
    public enum BuildMode
    {
        None,
        Menu,
        Placing,
    }

    [Serializable]
    public class PlaceInfo
    {
        public BuildingType type;
        public Vector2 size;
        public Sprite ghost;
        public Key hotKey = Key.None;
    }

    [SerializeField] private Camera camera;
    [SerializeField] private SpriteRenderer ghost;
    [SerializeField] private List<PlaceInfo> placeInfos;
    [SerializeField] private LayerMask obstacleLayerMask;

    [SerializeField] private Key buildMenuKey = Key.B;
    [SerializeField] private GameObject buildMenuUI;

    private PlaceInfo currentPlaceInfo;
    private BuildMode mode = BuildMode.None;
    private int cancelFrame = 1;

    public bool IsPlacing => mode == BuildMode.Placing;
    public bool IsMenuOpen => mode == BuildMode.Menu;
    public bool IsBuildMode => mode != BuildMode.None;
    public bool BlockCommand => IsBuildMode || cancelFrame == Time.frameCount;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Update()
    {
        switch (mode)
        { 
            case BuildMode.None:
                HandleOpenKey();
                break;
            case BuildMode.Menu:
                HandleMenu();
                break;
            case BuildMode.Placing:
                HandlePlacing();
                break;
        }

    }
    private void HandleOpenKey()
    {
        if (!Keyboard.current[buildMenuKey].wasPressedThisFrame)
            return;
        if (!Player.instance.HasSelectedPawn())
            return;

        OpenMenu();
    }
    private void HandleMenu()
    {
        if(!Player.instance.HasSelectedPawn())
        {
            ExitBuildMode();
            return;
        }
        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ExitBuildMode();
            return;
        }
        foreach(PlaceInfo info in placeInfos)
        {
            if (info.hotKey == Key.None)
                continue;
            if (Keyboard.current[info.hotKey].wasPressedThisFrame )
            {
                StartPlacement(info);
                return;
            }
        }
    }
    private void HandlePlacing()
    {
        if(!Player.instance.HasSelectedPawn())
        {
            ExitBuildMode();
            return;
        }

        Vector2 mousePos = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 fitPos = GridManager.instance.FitNode(mousePos);
        ghost.transform.position = fitPos;

        bool canPlace = CanPlaceAt(fitPos);
        ghost.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);

        if (Mouse.current.leftButton.wasPressedThisFrame && canPlace)
        {
            TryPlace(fitPos);
            return;
        }
        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OpenMenu();
            return;
        }
    }
    private void OpenMenu()
    {
        mode = BuildMode.Menu;
        currentPlaceInfo = null;
        ghost.gameObject.SetActive(false);
        buildMenuUI?.SetActive(true);
    }
    private void StartPlacement(PlaceInfo info)
    {
        currentPlaceInfo = info;
        ghost.sprite = info.ghost;
        ghost.gameObject.SetActive(true);
        mode = BuildMode.Placing;
        buildMenuUI?.SetActive(false);
    }
    private void ExitBuildMode()
    {
        mode = BuildMode.Menu;
        currentPlaceInfo = null;
        cancelFrame = Time.frameCount;
        ghost.gameObject.SetActive(false);
        buildMenuUI?.SetActive(false);
    }
    private bool CanPlaceAt(Vector2 center)
    {
        return Physics2D.OverlapBox(center, currentPlaceInfo.size, 0f, obstacleLayerMask) == null
            && GridManager.instance.IsAreaWalkable(center, currentPlaceInfo.size);
    }
    private void TryPlace(Vector2 pos)
    {
        BuildingStat stat = BuildingDataLoader.instance.GetBuildingStat(currentPlaceInfo.type);
        if (!FactionManager.instance.Player.TryReduceResource(stat.WoodCost, stat.GoldCost))
            return;

        Building building = ObjectPoolManager.instance.GetObject<Building>(currentPlaceInfo.type.ToString());
        building.SetLayer(Layer.Player);
        building.SetSkipBuilded(false);
        building.Init();
        building.transform.position = pos;

        Player.instance.CommandBuild(building);
        ExitBuildMode();
    }

}   
