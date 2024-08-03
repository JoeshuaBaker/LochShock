using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : BasicEnemy
{

    public bool playerClose = false;
    public float explosionDelay;
    public float explosionSize;
    private float explosionCountdown;
    private float moveSpeed;
    public float activationRange;
    public GameObject dangerZone;
    private bool secondaryZoneSpawned = false;
    public float explosionResetTime = 0.5f;
    public Vector3 explosionScale;


    public override int EnemyId()
    {
        return 10;
    }

    public override void Update()
    {
        base.Update();

        explosionResetTime = explosionResetTime - Time.deltaTime;

        if(dangerZone != null)
        {
            dangerZone.transform.position = this.transform.position;
        }

        if (directionToPlayer.magnitude < activationRange && !playerClose && !dying && explosionResetTime < 0f)
        {
            animator.SetBool("playerNear", (true));
            dangerZone = World.activeWorld.explosionSpawner.CreateDangerZone(maxHp * 500, explosionDelay, this.transform.position, true , false , false, explosionScale);
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
            if (!secondaryZoneSpawned)
            {
                World.activeWorld.explosionSpawner.CreateDangerZone(maxHp * 500, 0f, this.transform.position, true , false , false , explosionScale);
                secondaryZoneSpawned = true;
            }

            if (dangerZone != null)
            {
                
                dangerZone.SetActive(false);
                dangerZone = null;
            }

            playerClose = false;

            this.gameObject.SetActive(false);
            
        }
    }

    public override void Die()
    {
        base.Die();

        if (!secondaryZoneSpawned)
        {
            World.activeWorld.explosionSpawner.CreateDangerZone(maxHp * 500, 0f, this.transform.position, true , false , false , explosionScale);
            secondaryZoneSpawned = true;
        }

        if (dangerZone != null)
        {
            dangerZone.SetActive(false);
            dangerZone = null;
        }

        playerClose = false;

       this.gameObject.SetActive(false);
    }

    public override void Reset()
    {
        base.Reset();

        if(speed != 0f)
        {
            moveSpeed = speed;
        }

        if(dangerZone != null)
        {
            dangerZone = null;
        }

        secondaryZoneSpawned = false;

        explosionResetTime = 0.5f;

        playerClose = false;

        explosionCountdown = 0f;
        speed = moveSpeed;
    }

}
