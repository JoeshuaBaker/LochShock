using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

public abstract class Enemy : BulletCollidable
{
    //References
    public SpriteRenderer sprite;
    public Rigidbody2D rb;

    //Internal state variables
    public int maxHp = 10;
    public int currentHp = 10;
    public float bonusHpPerMinute = 2f;
    public float bonusHpPerOrbUsedMinute = 3f;
    public int instanceId = 0;

    public Color hitFreezeColor = new Color(220, 243, 255, 1f);
    public float freezeTime;
    public float freezeTimePerHit = 0.4f;

    public virtual void Start()
    {
    }

    public virtual bool HitFreeze()
    {
        freezeTime = Mathf.Max(freezeTime - Time.deltaTime, 0f);
        sprite.color = (freezeTime == 0f) ? Color.white : hitFreezeColor;

        return freezeTime > 0;
    }

    public override void ProcessCollision(ProjectileData projectile)
    {
        GameContext enemyContext = World.activeWorld.worldStaticContext;
        enemyContext.damageContext = projectile.bulletContext;
        float damage = projectile.stats.GetCombinedStatValue<Damage>(enemyContext);
        float knockback = projectile.stats.GetCombinedStatValue<Knockback>(enemyContext);
        TakeDamage(damage);
        ApplyKnockback(projectile.Velocity.normalized * knockback);

        World.activeWorld.hitEffect.EmitBulletHit(projectile);

    }

    public virtual void ApplyKnockback(Vector2 force)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(force*10);
    }

    public bool IsDead()
    {
        return currentHp <= 0;
    }

    public virtual void TakeDamage(float damage)
    {
        if(damage > 0)
        {
            freezeTime = freezeTimePerHit;
        }

        currentHp = (int)Mathf.Max(currentHp - damage, 0);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Die(0f);
    }

    public virtual void Die(float delay)
    {
        Destroy(gameObject, delay);
    }

    public virtual void Reset()
    {
        currentHp = (int)(maxHp + maxHp * ((bonusHpPerMinute) * Time.timeSinceLevelLoad / 60f) + maxHp * ((bonusHpPerOrbUsedMinute) * Player.activePlayer.timeSinceOrbUsed / 60f));
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        instanceId = 0;
        this.gameObject.SetActive(true);
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    public abstract int EnemyId();
}
