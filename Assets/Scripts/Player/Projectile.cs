using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public class Projectile : MonoBehaviour
{
    public Gun source;
    public Collider2D hitbox;
    public ProjectileData projectileData;
    public Vector2 target;
    public bool isPlayerProjectile;

    //internal state variables
    protected ContactFilter2D hitFilter;
    protected List<RaycastHit2D> hitBuffer;
    protected RaycastHit2D bounceTarget;
    protected float timeSinceSetup = 0f;

    public virtual void SetupProjectile(Gun source, CombinedStatBlock stats, Vector3 position, Vector2 direction, Vector2 target, int numStream = 0, int numBullet = 0, bool isPlayerProjectile = true)
    {
        gameObject.SetActive(true);
        if (projectileData == null)
        {
            projectileData = new ProjectileData();
        }

        timeSinceSetup = 0f;
        projectileData.SetupProjectileData(stats, position, direction * stats.GetCombinedStatValue<Velocity>(World.activeWorld.worldStaticContext), false);
        projectileData.SetupBulletContext(source);
        projectileData.ApplyStatBlock(stats);
        this.source = source;
        this.target = target;
        this.isPlayerProjectile = isPlayerProjectile;
        transform.position = position;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(direction.x, direction.y, 0f));
        hitBuffer = new List<RaycastHit2D>();

        hitFilter = new ContactFilter2D
        {
            layerMask = isPlayerProjectile ? (1 << LayerMask.NameToLayer("Enemy")) : (1 << LayerMask.NameToLayer("Player")),
            useTriggers = false,
            useLayerMask = true
        };

        if (projectileData.bulletContext.hitEnemies == null)
        {
            projectileData.bulletContext.hitEnemies = new HashSet<Enemy>();
        }
        else
        {
            projectileData.bulletContext.hitEnemies.Clear();
        }

        if(projectileData.IgnoreList == null)
        {
            projectileData.IgnoreList = new HashSet<string>();
        }
        else
        {
            projectileData.IgnoreList.Clear();
        }

        this.gameObject.layer = isPlayerProjectile ? LayerMask.NameToLayer("PlayerProjectile") : LayerMask.NameToLayer("EnemyProjectile");
    }

    public virtual void Update()
    {
        CheckCollisions();
        Move();
        Lifetime();
    }

    public virtual void Move()
    {
        projectileData.Position += projectileData.DeltaPosition(Time.deltaTime);
        transform.position = projectileData.Position;
    }

    public virtual void CheckCollisions()
    {
        Vector2 deltaPos = projectileData.DeltaPosition(Time.deltaTime);
        int numCollisions = 0;
        if (hitbox is CircleCollider2D)
        {
            numCollisions = Physics2D.CircleCast(projectileData.Position, (hitbox as CircleCollider2D).radius, deltaPos.normalized, hitFilter, hitBuffer, deltaPos.magnitude);
        }
        else if(hitbox is BoxCollider2D)
        {
            numCollisions = Physics2D.BoxCast(projectileData.Position, (hitbox as BoxCollider2D).size, transform.rotation.eulerAngles.z, deltaPos.normalized, hitFilter, hitBuffer, deltaPos.magnitude);
        }

        for (int i = 0; i < hitBuffer.Count; i++)
        {
            string hitName = hitBuffer[i].transform.name;
            if (projectileData.IgnoreList.Contains(hitName))
            {
                continue;
            }

            BulletCollidable hitTarget = hitBuffer[i].transform.GetComponent<BulletCollidable>();
            Collide(hitTarget, hitBuffer[i]);

            if (hitBuffer[i].distance < bounceTarget.distance)
            {
                bounceTarget = hitBuffer[i];
            }
        }

        //implement bounce & pierce
        if(numCollisions > 0)
        {
            CleanupProjectile();
        }
    }

    public virtual void Collide(BulletCollidable hitTarget, RaycastHit2D hitInfo)
    {
        if (hitTarget is Enemy)
        {
            Enemy enemy = hitTarget as Enemy;
            projectileData.IgnoreList.Add(hitTarget.name);
            projectileData.bulletContext.hitEnemies.Add(enemy);
            hitTarget.ProcessCollision(projectileData, hitInfo);

            foreach (OnHitAction onHit in projectileData.stats.combinedStatBlock.GetEvents<OnHitAction>())
            {
                Item eventSource = Player.activePlayer.inventory.FindEventSource(onHit);
                onHit.OnHit(eventSource, Player.activePlayer, source, projectileData, enemy);
            }

            if (enemy.IsDead())
            {
                foreach (OnKillAction onKill in projectileData.stats.combinedStatBlock.GetEvents<OnKillAction>())
                {
                    Item eventSource = Player.activePlayer.inventory.FindEventSource(onKill);
                    onKill.OnKill(eventSource, Player.activePlayer, source, projectileData, enemy);
                }
            }
        }
        else
        {
            if (hitTarget != null)
            {
                hitTarget.ProcessCollision(projectileData, hitInfo);
            }
        }
    }

    public virtual void Lifetime()
    {
        timeSinceSetup += Time.deltaTime;
        if (timeSinceSetup > projectileData.stats.GetCombinedStatValue<Lifetime>(World.activeWorld.worldStaticContext))
        {
            CleanupProjectile();
        }
    }

    public virtual void CleanupProjectile()
    {
        this.gameObject.SetActive(false);
    }
}
