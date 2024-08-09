using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

public abstract class BasicEnemy : Enemy
{
    //references
    public Player player;
    public Animator animator;
    public string deathAnimationState;
    public string deathBool = "Die";

    //public properties
    public float speed = 1f;

    //internal state vars
    protected bool dying = false;
    protected float deathTimer = 0f;
    protected Vector3 directionToPlayer;
    public override void Start()
    {
        base.Start();
        player = Player.activePlayer;
    }

    public virtual void Update()
    {
        if (dying)
        {
            if (deathTimer > 0)
            {
                deathTimer -= Time.deltaTime;
            }
            else
            {
                AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
                if (!animState.IsName(deathAnimationState))
                {
                    DeathAnimationBegin();
                }

                if (animState.IsName(deathAnimationState) && animState.normalizedTime >= 1f)
                {
                    DisableGameObject();
                }
                return;
            }
        }

        if (HitFreeze())
        {
            return;
        }

        directionToPlayer = player.transform.position - this.transform.position;

        if (directionToPlayer.magnitude > 30f)
        {
            this.gameObject.SetActive(false);
        }

        rb.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y).normalized * speed;
        if(player.isDead)
        {
            rb.velocity = -rb.velocity;
        }
    }

    public override void Die()
    {
        dying = true;
        animator.SetBool(deathBool, true);
        rb.simulated = false;
    }

    public override void Die(float delay)
    {
        dying = true;
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        deathTimer = delay;
    }

    public override void Reset()
    {
        base.Reset();

        dying = false;
        animator.SetBool(deathBool, false);
        rb.simulated = true;
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    public virtual void DeathAnimationBegin()
    {
        animator.SetBool(deathBool, true);
    }

    public virtual void DisableGameObject()
    {
        this.gameObject.SetActive(false);
        World.activeWorld.enemyPool.activeEnemies.Remove(this);
    }
}
