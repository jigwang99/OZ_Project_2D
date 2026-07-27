using UnityEngine;
using UnityEngine.UI;

public class ProductSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    private int index;
    private ProductQueueUI owner;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }
    public void SetUp(ProductQueueUI owner, int index)
    {
        this.index = index;
        this.owner = owner;
    }
    public void Bind(Sprite icon)
    {
        iconImage.sprite = icon;
        gameObject.SetActive(true);
    }
    public void Clear()
    {
        gameObject.SetActive(false);
    }
    private void OnClick()
    {
        owner.RequestCancel(index);
    }
}
