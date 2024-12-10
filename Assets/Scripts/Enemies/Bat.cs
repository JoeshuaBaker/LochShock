using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : EliteEnemy
{
    public Vector3 pathPoint;
    public Vector2 pathPointOffset;
    public Vector2 offsetVelocity;
    public float maxOffsetVelocity = 4f;
    public Vector2 offsetAcceleration;
    public float randomOffsetTurnPower = 10f;
    public AnimationCurve turnPowerDistribution;
    public BatMissile missile;
    public float missileFlyTime = 1.5f;
    public Animator animator;
    public float swarmSize;
    private float offsetFloat;
    private float turnPower;
    private bool dying;
    private float attackCooldown = 3f;
    private float attackTimer = 0f;

    public override bool dealsTouchDamage => false;

    public override void Setup(EliteCoordinator coordinator)
    {
        base.Setup(coordinator);

        dying = false;
        swarmSize = (coordinator as BatCoordinator).swarmSize;
        turnPower = (coordinator as BatCoordinator).turnPower + turnPowerDistribution.Evaluate(Random.Range(0f, 1f)) * randomOffsetTurnPower;
        pathPointOffset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * swarmSize;
        maxOffsetVelocity = swarmSize;
        offsetVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * maxOffsetVelocity;
        offsetAcceleration = offsetVelocity.normalized;
        offsetFloat = Random.Range(0f, 2f*Mathf.PI);
        this.transform.position = pathPoint + pathPointOffset.xyz();
        attackCooldown += Random.Range(0f, 2f);
        animator.SetBool("Die", false);
        missile = Instantiate(missile, World.activeWorld.projectilePoolParent);
    }

    public override void CoordinatorUpdate()
    {
        //Do dying behaviors and return if already dead.
        if (dying)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName("BatDie"))
            {
                if (animState.normalizedTime >= 1f)
                {
                    alive = false;
                }
            }

            return;
        }

        offsetFloat += Time.deltaTime;
        var offsetCosin = Mathf.Cos(offsetFloat);
        var offsetSin = Mathf.Sin(offsetFloat);
        var distToPathPoint = pathPoint - this.transform.position;
        var dirToPathPoint = distToPathPoint.normalized.xy();
        offsetAcceleration = Utilities.RotateTowards(offsetAcceleration, dirToPathPoint, Mathf.Deg2Rad * distToPathPoint.magnitude * turnPower).normalized * distToPathPoint.magnitude * turnPower/swarmSize;
        offsetVelocity += (offsetAcceleration + new Vector2(offsetCosin, offsetSin)) * Time.deltaTime;
        offsetVelocity = new Vector2(Mathf.Clamp(offsetVelocity.x, -maxOffsetVelocity, maxOffsetVelocity), Mathf.Clamp(offsetVelocity.y, -maxOffsetVelocity, maxOffsetVelocity));
        pathPointOffset += offsetVelocity * Time.deltaTime;

        attackTimer += Time.deltaTime;
        if(attackTimer >= attackCooldown)
        {
            missile.Fire(missileFlyTime, this.transform.position, Player.activePlayer.transform.position);
            World.activeWorld.explosionSpawner.CreateDangerZone(1, missileFlyTime, Player.activePlayer.transform.position, true, false, false, Vector3.one, false, Quaternion.identity, 0);
            attackTimer %= attackCooldown;
        }

        this.transform.position = pathPoint + pathPointOffset.xyz();

        //check if we died this frame
        if (alive && currentHp <= 0)
        {
            Die();
        }
    }

    public override void Die(float delay)
    {
        dying = true;
        animator.SetBool("Die", true);
        rb.simulated = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null && !(enemy is EliteEnemy))
            {
                enemy.Die();
            }
        }
    }
}
