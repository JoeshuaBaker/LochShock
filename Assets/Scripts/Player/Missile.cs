using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile
{
    public List<GameObject> disableOnCleanup;
    public float trailDisableDelay = 1f;
    public float damageRadiusScale = 1f;
    public float explosionRadiusScale = 1f;
    private bool dead = false;
    private bool hasExploded = false;

    public ContactFilter2D explosionHitFilter;

    public override void SetupProjectile(Gun source, CombinedStatBlock stats, Vector3 position, Vector2 direction, Vector2 target, int numStream = 0, int numBullet = 0, bool isPlayerProjectile = true)
    {
        base.SetupProjectile(source, stats, position, direction, target, numStream, numBullet, isPlayerProjectile);
        hasExploded = false;
    }

    public override void Collide(BulletCollidable hitTarget, RaycastHit2D hitInfo)
    {
        if (hasExploded)
            return;

        base.Collide(hitTarget, hitInfo);
        GameContext hitContext = World.activeWorld.worldStaticContext;
        hitContext.damageContext = projectileData.bulletContext;
        float size = projectileData.stats.GetCombinedStatValue<Size>(hitContext);
        List<Collider2D> explosionTargets = new List<Collider2D>();
        Physics2D.OverlapCircle(hitInfo.point, size * damageRadiusScale, explosionHitFilter, explosionTargets);

        foreach(Collider2D hit in explosionTargets)
        {
            BulletCollidable collidable = hit.GetComponent<BulletCollidable>();

            if(collidable is Enemy)
            {
                projectileData.bulletContext.hitEnemies.Add(collidable as Enemy);

                World.activeWorld.hitEffect.EmitZoneHit(collidable.gameObject.transform.position, (collidable.gameObject.transform.position - this.transform.position));
            }
            else if(collidable is BossSeed)
            {
                projectileData.bulletContext.hitBoss = collidable as BossSeed;

                World.activeWorld.hitEffect.EmitZoneHit(collidable.gameObject.transform.position, (collidable.gameObject.transform.position - this.transform.position));
            }

            AdvancedDoodad doodad = hit.GetComponentInParent<AdvancedDoodad>();
            if (doodad != null)
            {
                doodad.Destruct();
                World.activeWorld.hitEffect.EmitTreeHit(doodad.transform.position, (doodad.transform.position - this.transform.position));
                continue;
            }

        }

        foreach(Enemy enemy in projectileData.bulletContext.hitEnemies)
        {
            enemy.ProcessCollision(projectileData, hitInfo);
        }

        if (projectileData.bulletContext.hitBoss != null)
        {
            projectileData.bulletContext.hitBoss.ProcessCollision(projectileData, hitInfo);
        }

        Vector2 direction = projectileData.Velocity.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        World.activeWorld.explosionSpawner.CreateExplosion(hitInfo.point, size * explosionRadiusScale, Quaternion.AngleAxis(angle+90f, Vector3.forward));
        hasExploded = true;
    }

    public override void CleanupProjectile()
    {
        //base.CleanupProjectile();
        foreach (var go in disableOnCleanup)
        {
            go.SetActive(false);
        }

        Invoke("DelayDisable", trailDisableDelay);
        dead = true;
    }

    public override void Update()
    {
        if(!dead)
        {
            base.Update();
        }
    }

    private void DelayDisable()
    {
        foreach (var go in disableOnCleanup)
        {
            go.SetActive(true);
        }

        this.gameObject.SetActive(false);
        dead = false;
    }
}
