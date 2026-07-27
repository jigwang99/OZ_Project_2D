using UnityEngine;

public class MinimapMarker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer marker;
    [SerializeField] private Color allyColor = Color.green;
    [SerializeField] private Color enemyColor = Color.red;

    public void ApplyFaction(int layer)
    {
        if (marker == null)
            return;
        bool isAlly = LayerUtility.IsPlayerSide(layer);
        marker.color = isAlly ? allyColor : enemyColor;
    }
}
