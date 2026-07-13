using UnityEngine;
using UnityEngine.InputSystem;

public class Barrack : MonoBehaviour
{
    [SerializeField] GameObject warrior;

    private void Update()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
            Instantiate(warrior, transform.position, Quaternion.identity);
    }
}
