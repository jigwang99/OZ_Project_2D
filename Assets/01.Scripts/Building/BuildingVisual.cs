using UnityEngine;

public class BuildingVisual : MonoBehaviour
{
    [SerializeField] private Sprite allySprite;
    [SerializeField] private Sprite enemySprite;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public void ApplySprite()
    {
        bool isAlly = LayerUtility.IsPlayerBuilding(gameObject.layer);
        sr.sprite = isAlly ? allySprite : enemySprite;
    }
}
