using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildPlacer : MonoBehaviour
{
    public static BuildPlacer instance;

    [Serializable]
    public class PlaceInfo
    {
        public BuildingType type;
        public Vector2 size;
        public Sprite ghost;
    }

    [SerializeField] private Camera camera;
    [SerializeField] private SpriteRenderer ghost;
    [SerializeField] private List<PlaceInfo> placeInfos;
    [SerializeField] private LayerMask obstacleLayerMask; 

    private PlaceInfo currentPlaceInfo;
    private bool isPlacing;

    public bool IsPlacing => isPlacing;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Update()
    {
        HandleKeys();

        if (!isPlacing)
            return;

        Vector2 mousePos = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 fitPos = GridManager.instance.FitNode(mousePos);
        ghost.transform.position = fitPos;

        bool canPlace = CanPlaceAt(fitPos);
        ghost.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);

        if(Mouse.current.leftButton.wasPressedThisFrame && canPlace)
            TryPlace(fitPos);

        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            CancelPlacement();
        
    }
    private void HandleKeys()
    {
        if(Keyboard.current.cKey.wasPressedThisFrame)
            StartPlacement(BuildingType.Castle);
        if(Keyboard.current.bKey.wasPressedThisFrame)
            StartPlacement(BuildingType.Barracks);
        if (Keyboard.current.hKey.wasPressedThisFrame)
            StartPlacement(BuildingType.House);
        if (Keyboard.current.tKey.wasPressedThisFrame)
            StartPlacement(BuildingType.Tower);
        //if (Keyboard.current.aKey.wasPressedThisFrame)
        //    StartPlacement(BuildingType.Archery);
        //if (Keyboard.current.mKey.wasPressedThisFrame)
        //    StartPlacement(BuildingType.Monastery);
    }
    private void StartPlacement(BuildingType type)
    {
        if (!Player.instance.HasSelectedPawn())
            return;

        currentPlaceInfo = placeInfos.Find(p => p.type == type);
        if (currentPlaceInfo == null)
            return;

        ghost.sprite = currentPlaceInfo.ghost;
        isPlacing = true;
        ghost.gameObject.SetActive(true);
    }
    private void CancelPlacement()
    {
        isPlacing = false;
        currentPlaceInfo = null;
        ghost.gameObject.SetActive(false);
    }
    private bool CanPlaceAt(Vector2 center)
    {
        return Physics2D.OverlapBox(center, currentPlaceInfo.size, 0f, obstacleLayerMask) == null
            && GridManager.instance.IsAreaWalkable(center, currentPlaceInfo.size);
    }
    private void TryPlace(Vector2 pos)
    {
        BuildingStat stat = BuildingManager.instance.GetBuildingStat(currentPlaceInfo.type);
        if (!Player.instance.TryReduceResource(stat.WoodCost, stat.GoldCost))
            return;

        Building building = ObjectPoolManager.instance.GetObject<Building>(currentPlaceInfo.type.ToString());
        building.transform.position = pos;

        Player.instance.CommandBuild(building);
    }

}   
