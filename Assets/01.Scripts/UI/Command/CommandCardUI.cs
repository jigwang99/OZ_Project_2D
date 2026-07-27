using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class CommandCardUI : MonoBehaviour
{
    public static CommandCardUI instance;

    public const int SlotCount = 9;
    private const int CancelIndex = 8;

    private enum Page
    {
        Root,
        Build
    }

    [SerializeField] private List<CommandSlotUI> slots = new List<CommandSlotUI>();

    [Header("Icon")]
    [SerializeField] private Sprite moveIcon;  //M
    [SerializeField] private Sprite stopIcon;  //S
    [SerializeField] private Sprite attackIcon; //A
    [SerializeField] private Sprite buildIcon;  //B
    [SerializeField] private Sprite cancelIcon; // C

    private static Key[] productKeys = { Key.A, Key.S, Key.D, Key.F };

    private Command[] commands=  new Command[SlotCount];
    private Page page = Page.Root;

    private ProductionBuilding boundBuilding;
    private bool placingCache;
    private Player.TargetingMode targetingCache;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        Player.instance.OnSelectionChanged += OnSelectionChanged;
        Refresh();
    }
    private void OnDestroy()
    {
        if(Player.instance != null)
            Player.instance.OnSelectionChanged -= OnSelectionChanged;
        BindProduction(null);
    }
    private void Update()
    {
        bool placing = BuildPlacer.instance != null && BuildPlacer.instance.IsPlacing;
        Player.TargetingMode  targeting = Player.instance.Targeting;

        if(placing != placingCache || targeting != targetingCache)
        {
            placingCache = placing;
            targetingCache = targeting;
            Refresh();
        }
        HandleHotKeys();
    }
    private void OnSelectionChanged()
    {
        page = Page.Root;
        Refresh();
    }
    public void Back()
    {
        if (Player.instance.Targeting != Player.TargetingMode.None)
            Player.instance.CancelTargeting();
        else if(BuildPlacer.instance != null && BuildPlacer.instance.IsPlacing)
            BuildPlacer.instance.CancelPlacement();
        else if(page == Page.Build)
            page = Page.Root;

        Refresh();
    }
    private void Refresh()
    {
        for (int i = 0; i < SlotCount; i++)
            commands[i] = null;

        if(Player.instance.Targeting != Player.TargetingMode.None || (BuildPlacer.instance != null && BuildPlacer.instance.IsPlacing))
        {
            BindProduction(null);
            SetCancel();
        }
        else if(page == Page.Build && Player.instance.IsAllPawnSelected())
        {
            BindProduction(null);
            BuildPage();
        }
        else
        {
            page = Page.Root;
            RootPage();
        }
        Apply();
    }
    private void RootPage()
    {
        if(Player.instance.SelectUnitList.Count > 0)
        {
            BindProduction(null);
            UnitPage();
            return;
        }

        Building building = Player.instance.SelectBuilding;

        if(building == null || !building.IsAlive || building.IsConstruction)
        {
            BindProduction(null);
            return;
        }
        if(building is Tower)
        {
            BindProduction(null);
            TowerPage();
            return;
        }
        if(building is ProductionBuilding pb)
        {
            BindProduction(pb);
            ProductPage(pb);
            return;
        }
        BindProduction(null);
    }
    private void UnitPage()
    {
        SetCommand(0, moveIcon, Key.M, "Move",
            () => Player.instance.BeginTargeting(Player.TargetingMode.Move));
        SetCommand(1, stopIcon, Key.S, "Stop",
            () => Player.instance.StopUnits());
        SetCommand(2, attackIcon, Key.A, "Attack",
            () => Player.instance.BeginTargeting(Player.TargetingMode.Attack));

        if(Player.instance.IsAllPawnSelected())
            SetCommand(6, buildIcon, Key.B, "Build",
            () => { page = Page.Build; Refresh(); });
    }
    private void TowerPage()
    {
        SetCommand(2, attackIcon, Key.A, "Attack",
            () => Player.instance.BeginTargeting(Player.TargetingMode.Attack));
    }
    private void ProductPage(ProductionBuilding pb)
    {
        IReadOnlyList<UnitType> producible = pb.ProducibleUnits;

        for(int i = 0; i < producible.Count && i < CancelIndex; i++)
        {
            UnitType type = producible[i];
            
            if(!pb.CanProduce(type))
                continue;

            UnitStat unitStat = UnitDataLoader.instance.Get(type);
            if(unitStat == null)
                continue;

            Key key = i < productKeys.Length ? productKeys[i] : Key.None;

            SetCommand(i, unitStat.Icon, key, type.ToString(), () => TryEnqueue(pb, type));
        }
        if(pb.HasList)
        {
            SetCommand(CancelIndex, cancelIcon, Key.Escape, "CancelProduct", () => pb.CancelLastProduct());
        }
    }
    private void BuildPage()
    {
        int index = 0;

        foreach(BuildPlacer.PlaceInfo info in BuildPlacer.instance.PlaceInfos)
        {
            if (index >= CancelIndex)
                break;

            BuildingStat stat = BuildingDataLoader.instance.Get(info.type);
            if (stat == null)
                continue;

            BuildingType type = info.type;

            SetCommand(index, stat.Icon, info.hotKey, type.ToString(),
             () => BuildPlacer.instance.StartPlacement(type));

            index++;
        }
        SetCancel();
    }
    private void SetCancel()
    {
        SetCommand(CancelIndex, cancelIcon, Key.Escape, "Cancel", Back);
    }
    private void TryEnqueue(ProductionBuilding pb, UnitType type)
    {
        if (!pb.EnqueueUnit(type))
            Debug.Log("자원 또는 인구 부족");
    }
    private void SetCommand(int index, Sprite icon, Key key, string label, Action action)
    {
        if (index < 0 || index >= SlotCount)
            return;
        commands[index] = new Command(icon, key, label, action);
    }
    private void Apply()
    {
        for(int i = 0; i < slots.Count && i < SlotCount; i++)
        {
            if (slots[i] == null)
                continue;

            if (commands[i] != null)
                slots[i].Bind(commands[i]);
            else
                slots[i].Clear();
        }
    }
    private void HandleHotKeys()
    {
        if (Keyboard.current == null)
            return;

        for(int i =0; i < SlotCount; i++)
        {
            Command command = commands[i];
            if(command == null || command.HotKey == Key.None)
                continue;

            if (Keyboard.current[command.HotKey].wasPressedThisFrame)
            {
                command.Invoke();
                return;
            }
        }
    }
    private void BindProduction(ProductionBuilding pb)
    {
        if (boundBuilding == pb)
            return;
        if (boundBuilding != null)
            boundBuilding.OnProductChanged -= Refresh;

        boundBuilding = pb;

        if(boundBuilding != null)
            boundBuilding.OnProductChanged += Refresh;
    }
}
