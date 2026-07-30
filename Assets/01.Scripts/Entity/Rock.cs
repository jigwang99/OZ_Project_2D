using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour
{
    [SerializeField] private Vector2 obstacleSize;

    protected void OnEnable()
    {
        StartCoroutine(RegisterObstacleNextFrame());
    }
    private IEnumerator RegisterObstacleNextFrame()
    {
        yield return null;
        GridManager.instance.UpdateArea(transform.position, obstacleSize);
    }
}
