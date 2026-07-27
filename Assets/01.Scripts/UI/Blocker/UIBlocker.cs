using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public static class UIBlocker
{
    private static readonly List<RaycastResult> results = new List<RaycastResult>();
    private static PointerEventData pointerData;

    public static bool IsPointerOverUI()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
            return false;
        if (Mouse.current == null)
            return false;

        if (pointerData == null)
            pointerData = new PointerEventData(eventSystem);

        pointerData.Reset();
        pointerData.position = Mouse.current.position.ReadValue();

        results.Clear();
        eventSystem.RaycastAll(pointerData, results);

        return eventSystem.IsPointerOverGameObject();
    }
}
