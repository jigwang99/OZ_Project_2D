using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour
{
    private Collider2D col;
    private Vector2 colSize;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        colSize = col.bounds.size;
    }
    private void OnEnable()
    {
        StartCoroutine(RegisterObstacleNextFrame());
    }
    private IEnumerator RegisterObstacleNextFrame()
    {
        yield return null;
        GridManager.instance.UpdateArea(transform.position, colSize);
    }
}
