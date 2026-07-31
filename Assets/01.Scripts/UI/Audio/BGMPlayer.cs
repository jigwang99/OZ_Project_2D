using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(bgmClip != null)
            AudioManager.instance?.PlayBGM(bgmClip);
    }
}
