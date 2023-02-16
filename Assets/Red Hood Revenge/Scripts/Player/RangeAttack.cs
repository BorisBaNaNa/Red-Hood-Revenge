using UnityEngine;
using System.Collections;

public class RangeAttack : MonoBehaviour
{
    #region Inspector
    public Transform FirePoint;
    public Projectile Projectile;
    [Tooltip("Запуск снарядя после задержки, полезно для синхронизации с анимацией")]
    public float fireDelay;
    public float fireRate;
    public bool inverseDirection = false;
    #endregion

    public int BulletCount { get; set; } = 10;

    float nextFire = 0;

    public bool Fire()
    {
        if (BulletCount > 0 && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            BulletCount--;
            StartCoroutine(DelayAttack(fireDelay));
            return true;
        }
        else
            return false;
    }

    IEnumerator DelayAttack(float time)
    {
        yield return new WaitForSeconds(time);

        var direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        if (inverseDirection)
            direction *= -1;

        var projectile = (Projectile)Instantiate(Projectile, FirePoint.position, Quaternion.identity);

        projectile.Initialize(gameObject, direction, Vector2.zero);
    }
}
