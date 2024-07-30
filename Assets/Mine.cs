using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : BasicEnemy
{

    public bool playerClose;
    public float explosionDelay;
    public float explosionSize;
    private float explosionCountdown;
    private float moveSpeed;


    public override int EnemyId()
    {
        return 10;
    }

    public override void Update()
    {
        base.Update();

        if(directionToPlayer.magnitude < 2f)
        {
            animator.SetBool("playerNear", (true));
            playerClose = true;
        }

        if (playerClose)
        {
            speed = 0f;
            explosionCountdown = (explosionCountdown + Time.deltaTime);
        }

        if (explosionCountdown >= explosionDelay)
        {
            Die();
        }

        if (dying && deathTimer < 0f)
        {
            World.activeWorld.explosionSpawner.CreateExplosionWithCrater(this.transform.position, explosionSize);
            this.gameObject.SetActive(false);
        }
    }

    public override void Die()
    {
        base.Die();
        World.activeWorld.explosionSpawner.CreateExplosionWithCrater(this.transform.position, explosionSize);
        this.gameObject.SetActive(false);
    }

    public override void Reset()
    {
        base.Reset();

        if(speed != 0f)
        {
            moveSpeed = speed;
        }

        playerClose = false;
        explosionCountdown = 0f;
        speed = moveSpeed;
    }


}
