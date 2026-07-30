using System.Collections.Generic;
using UnityEngine;

public class BuildingEffect : MonoBehaviour
{
    [SerializeField] private List<GameObject> fireObjects = new List<GameObject>();
    [SerializeField] private EffectType explosionKey = EffectType.Explosion;
    [SerializeField, Range(0f, 1f)] private float fireThreshold = 1f / 3f;

    [SerializeField] private AudioClip explosionClip;

    private void OnDisable()
    {
        SetFire(false);
    }
    public void UpdateFire(float hpRatio)
    {
        SetFire(hpRatio <= fireThreshold && hpRatio > 0f);
    }
    private void SetFire(bool on)
    {
        if(fireObjects.Count > 0)
        {
            foreach(GameObject go in fireObjects)
            {
                go.SetActive(on);
            }
        }    
    }
    public void PlayExplosion()
    {
        SetFire(false);
        if(explosionClip != null)
            AudioManager.instance?.PlaySFXAt(explosionClip, transform.position);
        PooledEffect explosion = ObjectPoolManager.instance.GetObject<PooledEffect>(explosionKey);
        if (explosion == null)
            return;
        explosion.transform.position = transform.position;
    }
}
