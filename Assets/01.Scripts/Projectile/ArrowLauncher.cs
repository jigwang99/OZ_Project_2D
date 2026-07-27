using UnityEngine;

public static class ArrowLauncher
{
    public static bool Fire(Vector2 origin, IDamageable target, int damage, int shooterLayer, float offset)
    {
        if (target == null || !target.IsAlive)
            return false;

        Arrow arrow = ObjectPoolManager.instance.GetObject<Arrow>("Arrow");
        if (arrow == null)
            return false;

        Vector2 dir = ((Vector2)target.transform.position - origin).normalized;
        if (dir == Vector2.zero)
            dir = Vector2.right;

        arrow.transform.position = origin + dir * offset;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        arrow.SetDamageAndLayer(damage, shooterLayer);
        return true;
    }
}
