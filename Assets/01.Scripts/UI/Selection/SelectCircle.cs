using UnityEngine;

public class SelectCircle : MonoBehaviour
{
    [SerializeField] private GameObject selectCircle;

    private void OnEnable()
    {
        SetVisible(false);
    }
    public void SetVisible(bool visible)
    {
        selectCircle.gameObject.SetActive(visible);
    }

}
