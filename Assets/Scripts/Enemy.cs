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
    public int instanceId = 0;

    public Color hitFreezeColor = new Color(220, 243, 255, 1f);
    public float freezeTime;
    public float freezeTimePerHit = 0.4f;

    public virtual void Start()
    {
        sprite ??= GetComponent<SpriteRenderer>();
        rb ??= GetComponent<Rigidbody2D>();
    }

    public virtual bool HitFreeze()
    {
        freezeTime = Mathf.Max(freezeTime - Time.deltaTime, 0f);
        sprite.color = (freezeTime == 0f) ? Color.white : hitFreezeColor;

        return freezeTime > 0;
    }

    public override void ProcessCollision(ProjectileData projectile)
    {
        TakeDamage(projectile.Damage);
        ApplyKnockback(projectile.Velocity.normalized * projectile.Knockback);
    }

    public virtual void ApplyKnockback(Vector2 force)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(force*10);
    }

    public virtual void TakeDamage(int damage)
    {
        if(damage > 0)
        {
            freezeTime = freezeTimePerHit;
        }

        currentHp = Mathf.Max(currentHp - damage, 0);

        if (currentHp == 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }

    public virtual void Reset()
    {
        currentHp = maxHp;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        instanceId = 0;
        this.gameObject.SetActive(true);
    }

    public abstract int EnemyId();
}
