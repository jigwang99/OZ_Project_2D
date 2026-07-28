using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioClip clickClip;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { if (clickClip != null) AudioManager.instance?.PlaySFX(clickClip); });
    }
}
