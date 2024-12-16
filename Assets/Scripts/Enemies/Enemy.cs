using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

[System.Serializable]
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

    public virtual bool dealsTouchDamage => true;
    public virtual int touchDamage => 1;

    public virtual void Start()
    {
    }

    public virtual bool HitFreeze()
    {
        freezeTime = Mathf.Max(freezeTime - Time.deltaTime, 0f);
        sprite.color = (freezeTime == 0f) ? Color.white : hitFreezeColor;

        return freezeTime > 0;
    }

    public override void ProcessCollision(ProjectileData projectile, RaycastHit2D hitInfo)
    {
        GameContext enemyContext = World.activeWorld.worldStaticContext;
        enemyContext.damageContext = projectile.bulletContext;
        float secondaryDamageRatio = projectile.stats.GetCombinedStatValue<SecondaryHitDamage>();
        float damage = projectile.stats.GetCombinedStatValue<Damage>(enemyContext);
        float knockback = projectile.stats.GetCombinedStatValue<Knockback>(enemyContext);
        float secondaryDamageMultiplier = 1f;
        if (enemyContext.damageContext.numBounces > 0 || enemyContext.damageContext.numPierces > 0)
        {
            secondaryDamageMultiplier = Mathf.Pow(secondaryDamageRatio, enemyContext.damageContext.numBounces + enemyContext.damageContext.numPierces);
        }
        float finalDamage = damage * secondaryDamageMultiplier;
        TakeDamage(finalDamage);
        ApplyKnockback(projectile.Velocity.normalized * knockback);

        if(currentHp <= 0)
        {
            World.activeWorld.hitEffect.EmitBulletHit(projectile , hitInfo , this.transform.position , true);
        }
        else
        {
            World.activeWorld.hitEffect.EmitBulletHit(projectile , hitInfo , this.transform.position ,false);
        }
        

        ////Audio Section
        ////Sound is coming from Left of player
        //if (this.gameObject.transform.position.x < Player.activePlayer.transform.position.x)
        //{
        //    AkSoundEngine.SetRTPCValue("BulletImpactSpeakerPan_LR", 0 - Vector3.Distance(Player.activePlayer.transform.position, this.gameObject.transform.position));
        //}
        ////Sound is coming from right of player
        //else if (this.gameObject.transform.position.x > Player.activePlayer.transform.position.x)
        //{
        //    AkSoundEngine.SetRTPCValue("BulletImpactSpeakerPan_LR", Vector3.Distance(Player.activePlayer.transform.position, this.gameObject.transform.position));
        //}
        ////Play enemy hit sound after panning has been set
        //AkSoundEngine.PostEvent("PlayEnemyHit", this.gameObject);

        //Audio Section
        PlayAudioOnEnemy("PlayEnemyHit", "BulletImpactSpeakerPan_LR");
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

    public virtual void BombHit(float bombKillDistanceDelay)
    {
        Die(Utilities.GetDistanceToPlayer(this.transform.position) * bombKillDistanceDelay);
    }

    public virtual void TouchPlayer()
    {
        Die();
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

    public void PlayAudioOnEnemy(string eventName, string RTPCname)
    {
        //Audio Section
        //Sound is coming from Left of player
        if (this.gameObject.transform.position.x < Player.activePlayer.transform.position.x)
        {
            AkSoundEngine.SetRTPCValue(RTPCname, 0 - Vector3.Distance(Player.activePlayer.transform.position, this.gameObject.transform.position));
        }
        //Sound is coming from right of player
        else if (this.gameObject.transform.position.x >= Player.activePlayer.transform.position.x)
        {
            AkSoundEngine.SetRTPCValue(RTPCname, Vector3.Distance(Player.activePlayer.transform.position, this.gameObject.transform.position));
        }
        //Play enemy hit sound after panning has been set
        AkSoundEngine.PostEvent(eventName, this.gameObject);
    }
}
