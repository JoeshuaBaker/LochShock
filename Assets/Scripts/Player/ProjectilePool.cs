using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectilePool
{
    public int minPoolSize = 5;
    public int maxPoolSize = 1000;
    public int currentPoolSize;
    public List<Projectile> pool;

    private Transform bulletParent;
    private Projectile projectile;
    private int currentProjectileIndex;

    public ProjectilePool(Projectile projectile, CombinedStatBlock stats)
    {
        this.projectile = projectile;
        currentPoolSize = CalculatePoolSize(stats, World.activeWorld.worldStaticContext);
        pool = new List<Projectile>();
        currentProjectileIndex = 0;
        Transform projectilePoolParent = World.activeWorld.projectilePoolParent;
        bulletParent = new GameObject(projectile.name + " pool").transform;
        bulletParent.SetParent(projectilePoolParent);
        bulletParent.localPosition = Vector3.zero;

        for(int i = 0; i < currentPoolSize; i++)
        {
            Projectile instance = GameObject.Instantiate(projectile, bulletParent);
            instance.gameObject.SetActive(false);
            instance.name = projectile.name + " " + i;
            pool.Add(instance);
        }
    }

    public void UpdateProjectilePool(CombinedStatBlock stats)
    {
        int newPoolSize = CalculatePoolSize(stats, World.activeWorld.worldStaticContext);
        if(newPoolSize > currentPoolSize)
        {
            int difference = newPoolSize - currentPoolSize;
            for(int i = 0; i < difference; i++)
            {
                Projectile instance = GameObject.Instantiate(projectile, bulletParent);
                instance.gameObject.SetActive(false);
                instance.name = projectile.name + " " + (currentPoolSize + i);
                pool.Add(instance);
            }
        }

        currentPoolSize = newPoolSize;
    }

    private int CalculatePoolSize(CombinedStatBlock stats, GameContext worldContext)
    {
        return (int) Mathf.Clamp(
            (stats.GetCombinedStatValue<FireSpeed>(worldContext) *
            stats.GetCombinedStatValue<Lifetime>(worldContext) *
            stats.GetCombinedStatValue<BulletsPerShot>(worldContext) * 
            stats.GetCombinedStatValue<BulletStreams>(worldContext)) * 2,
            Mathf.Max(currentPoolSize, minPoolSize),
            maxPoolSize
        );
    }

    public Projectile GetProjectile()
    {
        Projectile projectile = pool[currentProjectileIndex];
        currentProjectileIndex = (currentProjectileIndex + 1) % currentPoolSize;
        return projectile;
    }

    public Projectile[] GetProjectiles(int bulletStreams, int bulletsPerShot)
    {
        int total = bulletStreams * bulletsPerShot;
        Projectile[] projectiles = new Projectile[total];
        for(int i = 0; i < total; i++)
        {
            projectiles[i] = GetProjectile();
        }

        return projectiles;
    }

    public void Cleanup()
    {
        GameObject.Destroy(bulletParent.gameObject);
    }
}
