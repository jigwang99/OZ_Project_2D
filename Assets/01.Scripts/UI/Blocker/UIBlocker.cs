using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public static class UIBlocker
{
    public static bool IsPointerOverUI()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
            return false;
        if (Mouse.current == null)
            return false;

        return eventSystem.IsPointerOverGameObject();
    }
}
