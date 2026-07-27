using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class CommandSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI hotKeyTMP;

    private Command command;

    private void Awake()
    {
        if(button != null)
            button.onClick.AddListener(OnClick);
    }
    public void Bind(Command command)
    {
        this.command = command;

        if(iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = command.Icon;
        }
        if(button != null)
            button.interactable = true;

        if(hotKeyTMP != null)
            hotKeyTMP.text = command.HotKey == Key.None ? "" : KeyLabel(command.HotKey);
    }
    public void Clear()
    {
        command = null;

        if (iconImage != null)
            iconImage.enabled = false;
        if (button != null)
            button.interactable = false;
        if (hotKeyTMP != null)
            hotKeyTMP.text = "";
    }
    private string KeyLabel(Key key)
    {
        return key == Key.Escape ? "ESC" : key.ToString();
    }
    private void OnClick()
    {
        command?.Invoke();
    }
}
