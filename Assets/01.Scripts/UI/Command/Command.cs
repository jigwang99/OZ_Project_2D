using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Command
{
    public Sprite Icon;
    public Key HotKey;
    public string Label;
    public Action Action;

    public Command(Sprite icon, Key hotKey, string label, Action action)
    {
        Icon = icon;
        HotKey = hotKey;
        Label = label;
        Action = action;
    }
    public void Invoke()
    {
         Action?.Invoke();
    }
}
