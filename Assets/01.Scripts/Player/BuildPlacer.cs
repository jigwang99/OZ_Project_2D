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
        public Key hotKey = Key.None;
    }

    [SerializeField] private Camera camera;
    [SerializeField] private SpriteRenderer ghost;
    [SerializeField] private List<PlaceInfo> placeInfos;
    [SerializeField] private LayerMask obstacleLayerMask;

    private PlaceInfo currentPlaceInfo;
    private bool isPlacing;
    private int cancelFrame = -1;

    public bool IsPlacing => isPlacing;
    public bool BlockCommand => isPlacing || cancelFrame == Time.frameCount;
    public IReadOnlyList<PlaceInfo> PlaceInfos => placeInfos;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Update()
    {
        if (!isPlacing)
            return;
        if(!Player.instance.IsAllPawnSelected())
        {
            CancelPlacement();
            return;
        }

        Vector2 mousePos = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 fitPos = GridManager.instance.FitNode(mousePos);
        ghost.transform.position = fitPos;

        bool canPlace = CanPlaceAt(fitPos);
        ghost.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);

        bool overUI = UIBlocker.IsPointerOverUI();

        if (Mouse.current.leftButton.wasPressedThisFrame && canPlace && !overUI)
        {
            TryPlace(fitPos);
            return;
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
            return;
        }

    }
    public void StartPlacement(BuildingType type)
    {
        if (!Player.instance.IsAllPawnSelected())
            return;

        PlaceInfo info  = placeInfos.Find(p => p.type == type);
        if (info == null)
            return;

        BuildingStat stat = BuildingDataLoader.instance.GetBuildingStat(type);
        Faction faction = FactionManager.instance.Player;

        if(stat == null || faction.Wood < stat.WoodCost || faction.Gold < stat.GoldCost)
        {
            return;
        }
        
        currentPlaceInfo = info;
        ghost.sprite = info.ghost;
        ghost.gameObject.SetActive(true);
        isPlacing = true;
    }
    public void CancelPlacement()
    {
        if (!isPlacing)
            return;

        isPlacing = false;
        currentPlaceInfo = null;
        cancelFrame = Time.frameCount;
        ghost.gameObject.SetActive(false);
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
        {
            CancelPlacement();
            return;
        }

        Building building = ObjectPoolManager.instance.GetObject<Building>(currentPlaceInfo.type.ToString());
        building.SetLayer(Layer.Player);
        building.SetSkipBuilded(false);
        building.transform.position = pos;
        building.Init();

        Player.instance.CommandBuild(building);
        CancelPlacement();
    }

}   
