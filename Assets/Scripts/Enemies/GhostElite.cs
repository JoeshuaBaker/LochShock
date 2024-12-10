using UnityEngine;

public class GhostElite : EliteEnemy
{
    public float baseVelocity = 0.25f;
    public float maxVelocity = 10f;
    public float acceleration = 0.75f;
    public float deceleration = 3.5f;
    public float turnSpeed = 45f;
    public float turnSlowAngle = 90f;
    public float chargeVelocityThreshold = 5f;
    public float chargeDistanceThreshold = 2f;
    public float chargeSelfDamageCooldown = 5f;
    public bool dying = false;

    [SerializeField] private Vector2 direction;
    [SerializeField] private Vector2 directionToPlayer;
    [SerializeField] private float velocity;
    [SerializeField] private float chargeSelfDamageTimer;
    [SerializeField] private SpriteRenderer mouthSprite;
    [SerializeField] private SpriteRenderer tailSprite;
    [SerializeField] private Animator mouthAnimator;

    public override void Setup(EliteCoordinator coordinator)
    {
        base.Setup(coordinator);
        velocity = baseVelocity;
        alive = true;
        dying = false;
        direction = Utilities.GetDirectionToPlayer(this.transform.position);
        tailSprite.enabled = true;
        rb.simulated = true;
        mouthAnimator.SetBool("Die", false);
    }

    public override void CoordinatorUpdate()
    {
        //Do dying behaviors and return if already dead.
        if(dying)
        {
            velocity -= deceleration * Time.deltaTime * 2.5f;
            velocity = Mathf.Clamp(velocity, baseVelocity, maxVelocity);
            this.transform.position = this.transform.position + (direction.xyz() * velocity * Time.deltaTime);

            AnimatorStateInfo animState = mouthAnimator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName("Ghost_Die_2"))
            {
                tailSprite.enabled = false;
                if(animState.normalizedTime >= 1f)
                {
                    alive = false;
                }
            }

            return;
        }

        //Calculate direction and turn equal to turn speed
        directionToPlayer = Utilities.GetDirectionToPlayer(this.transform.position);
        direction = Utilities.RotateTowards(direction, directionToPlayer, Mathf.Deg2Rad * turnSpeed * Time.deltaTime, 0f);

        //Move along direction vector
        if(Mathf.Abs(Vector2.SignedAngle(directionToPlayer, direction)) > turnSlowAngle)
        {
            velocity -= deceleration * Time.deltaTime;
        }
        else
        {
            velocity += acceleration * Time.deltaTime;
        }

        velocity = Mathf.Clamp(velocity, baseVelocity, maxVelocity);
        this.transform.position = this.transform.position + (direction.xyz() * velocity * Time.deltaTime);

        //Flip sprites if our direction vector is negative
        mouthSprite.flipY = direction.x < 0;
        tailSprite.flipY = direction.x < 0;

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * direction;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);

        transform.rotation = targetRotation;

        //Damage self each time we charge the player
        if (chargeSelfDamageTimer == 0f && velocity > chargeVelocityThreshold && Utilities.GetDistanceToPlayer(this.transform.position) < chargeDistanceThreshold)
        {
            TakeDamage(maxHp / 10f);
            chargeSelfDamageTimer = chargeSelfDamageCooldown;
        }
        else
        {
            chargeSelfDamageTimer = Mathf.Max(chargeSelfDamageTimer - Time.deltaTime, 0f);
        }

        //check if we died this frame
        if (alive && currentHp <= 0)
        {
            Die();
        }

        mouthAnimator.SetFloat("RandomTransition", Random.Range(0f, 1f));
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die(float delay)
    {
        dying = true;
        mouthAnimator.SetBool("Die", true);
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
