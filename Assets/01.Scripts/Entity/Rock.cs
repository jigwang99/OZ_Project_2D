using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour
{
    [SerializeField] private Vector2 obstacleSize;

    protected void OnEnable()
    {
        StartCoroutine(RegisterObtacleNextFrame());
    }
    private IEnumerator RegisterObtacleNextFrame()
    {
        yield return null;
        GridManager.instance.UpdateArea(transform.position, obstacleSize);
    }
}
