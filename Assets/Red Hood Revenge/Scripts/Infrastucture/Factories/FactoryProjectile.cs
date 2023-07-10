using UnityEngine;

public class FactoryProjectile : IService
{
    public Projectile ProjectilePrefab;

    public FactoryProjectile(Projectile projectilePrefab)
    {
        ProjectilePrefab = projectilePrefab;
    }

    public Projectile BuildProjectile(Vector3 at) => BuildProjectile(ProjectilePrefab, at);

    public ProjectileType BuildProjectile<ProjectileType>(ProjectileType projectilePrefab, Vector3 at) where ProjectileType : Projectile
    {
        at.z = -0.01f;
        return Object.Instantiate(projectilePrefab, at, Quaternion.identity);
    }
}
