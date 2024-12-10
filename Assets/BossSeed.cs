using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;
using Cinemachine;

public class BossSeed : BulletCollidable, ILevelLoadComponent
{
    public Animator animatorSeed;
    public Animator animatorWarning;
    public ParticleSystem thornPSHard;
    public ParticleSystem thornPSHardSmall;
    public ParticleSystem thornPSBlur;
    public ParticleSystem thornPSDrift;

    public BezierCurve curve;
    public BezierPoint p0;
    public Vector3 p0Pos;
    public BezierPoint p1;
    public Vector3 p1Pos;
    public float length;
    public float bossSpeed = 1f;
    public float catchUpSpeed;
    public float bossMaxDis;
    public float frameLengthOnCurve;
    public float bossLengthOnCurve;
    public float bossPercentDownCurve = 0f;
    public float bossAccelerationTime;
    public float bossCurrentAccelertationTime;
    public float bossCurrentAccelertation;

    public int bossSpawnX = 0;
    public float bossMaxHP;
    public float bossCurrentHP;
    public float bossHPPercent = 1f;
    public float hpDecayPerSecond;
    public float decayFloor;
    public float bossDRDecayPerSecond;
    public float bossCurrentDR;
    public float bossDRMax;
    public float bossHpThresholdOne;
    public bool thresholdOne;
    public float bossHpThresholdTwo;
    public bool thresholdTwo;

    public float attackTimeTotal;
    public float attackTime;
    public float currentSecondsForAttack;
    public float attacksPerSecondNoThreshold;
    public float attacksPerSecondThresholdOne;
    public float attacksPerSecondThresholdTwo;
    public float secondMultNoThreshold;
    public float secondMultThresholdOne;
    public float secondMultThresholdTwo;
    public float squareAtThresholdOne;
    public float squareAtThresholdTwo;

    public bool dying;
    public Animator deathAnimator;
    public GameObject deathContainer;
    public GameObject deathAnimRotator;
    public bool deathSetup;
    public ParticleSystem deathPS;
    public bool triggerDeathPS;
    public float deathPSRadius;
    public float radiusPerSecond;
    public GameObject bossBody;
    public GameObject bossColorPSParent;
    public ParticleSystem bloodPSOne;
    public ParticleSystem bloodPSTwo;
    public bool deathFinished;
    public float deathFinishedTime = 3f;
    public float deathFinishedCurrent;

    public List<Collider2D> hitBuffer;
    public ContactFilter2D hitFilter;

    public CinemachineImpulseSource shaker;
    public float shakerLoopMultperSecond =5f;
    public float shakeMult;
    public float baseShake = 0.1f;

    public Player player;
    public World world;
    public ExplosionSpawner explosionSpawner;


    public bool setUp;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.activePlayer;
        world = World.activeWorld;
        explosionSpawner = world.explosionSpawner;

        hitBuffer = new List<Collider2D>();
        hitFilter = new ContactFilter2D
        {
            useTriggers = false,
            layerMask = 1 << LayerMask.NameToLayer("Enemy"),
            useLayerMask = true
        };

        //AudioSection
        AkSoundEngine.SetState("SeedBossPhase", "PhaseOne");
        //AkSoundEngine.PostEvent("PlayBossSeedMusic", this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        if (world.paused)
        {
            return;
        }

        if (!setUp)
        {
            bossCurrentHP = bossMaxHP;
        }
        if (!dying)
        {
            UpdateBossPos();
            DecayStats();
            Attack();
            KillNearEnemies();
        }
        if (dying)
        {

            bossHPPercent = 0f;
            DieUpdate();

        }

    }

    public void UpdateBossPos()
    {
        if (!setUp)
        {
            //movement setup
            Tile tile = world.RandomPathTileAtXPosition(bossSpawnX + 25f);
            p0Pos = tile.transform.position;
            this.transform.position = p0Pos;
            p0.transform.position = p0Pos;


            Tile tile2 = world.RandomPathTileAtXPosition(bossSpawnX + 50f);
            p1Pos = tile2.transform.position;
            p1.transform.position = p1Pos;

            //stat setup
            bossCurrentDR = bossDRMax;

            setUp = true;
        }

        if (bossPercentDownCurve >= 1f)
        {
            bossLengthOnCurve = 0f;
            bossPercentDownCurve = 0f;

            p0Pos = p1Pos;

            float newX = p1Pos.x + 25f;

            Tile tile = world.RandomPathTileAtXPosition(newX);

            p1Pos = tile.transform.position;

        }

        if (bossPercentDownCurve < 1f && p0Pos != Vector3.zero)
        {
            Vector3 disToPlayer = this.transform.position - player.transform.position;
            float disToPlayerMag = disToPlayer.magnitude;

            float xDif = this.transform.position.x - player.transform.position.x;

            if (xDif >= 25f)
            {
                return;
            }

            if (xDif < 0f)
            {
                catchUpSpeed = catchUpSpeed + (bossSpeed * 0.2f * Time.deltaTime);
            }
            else
            { 
                if(catchUpSpeed > 0f)
                {
                    catchUpSpeed = Mathf.Max(catchUpSpeed - (bossSpeed * 0.4f * Time.deltaTime), 0f);
                }    
            }

            if(disToPlayerMag < 15f)
            {
                animatorWarning.SetBool("transition", true);
            }

            if(bossCurrentAccelertation < 10f)
            {
                bossCurrentAccelertationTime = bossCurrentAccelertationTime + Time.deltaTime;
                bossCurrentAccelertation = Mathf.Min(bossCurrentAccelertationTime / bossAccelerationTime, 1f);
            }

            
            xDif = Mathf.Clamp(xDif, 5f, 15f);
            float setSpeed = (bossSpeed*0.5f)+(bossSpeed * (5f / xDif) * 0.5f)+ catchUpSpeed;

            p0.transform.position = p0Pos;
            p1.transform.position = p1Pos;
            length = BezierCurve.ApproximateLength(p1, p0, 100);

            frameLengthOnCurve = (setSpeed * Time.deltaTime *60f)* bossCurrentAccelertation;

            bossLengthOnCurve = bossLengthOnCurve + frameLengthOnCurve;

            bossPercentDownCurve = 1f / (length / bossLengthOnCurve);

            this.transform.position = BezierCurve.GetPoint(p1, p0, 1f - bossPercentDownCurve);
        }
    }

    public override void ProcessCollision(ProjectileData projectile, RaycastHit2D hitInfo)
    {
        GameContext enemyContext = World.activeWorld.worldStaticContext;
        enemyContext.damageContext = projectile.bulletContext;
        enemyContext.damageContext.hitBoss = this;
        float damage = projectile.stats.GetCombinedStatValue<Damage>(enemyContext);
        TakeDamage(damage);


        World.activeWorld.hitEffect.EmitBulletHit(projectile , hitInfo , this.transform.position ,false);
    }

    public virtual void TakeDamage(float damage)
    {

        bossCurrentHP = (int)Mathf.Max(bossCurrentHP - (damage*(1f- bossCurrentDR)), 0f);

        //Debug.Log($"{damage} * {(1f - bossCurrentDR)} = {(damage * (1f - bossCurrentDR))}");

        if (bossCurrentHP <= 0)
        {
            Die();
        }
    }

    public void DecayStats()
    {
        bossHPPercent = bossCurrentHP / bossMaxHP;

        if (bossHPPercent <= bossHpThresholdOne && !thresholdOne)
        {
            bossCurrentDR = bossDRMax;
            thresholdOne = true;
        }

        if(bossHPPercent <= bossHpThresholdTwo && !thresholdTwo)
        {
            bossCurrentDR = bossDRMax;
            thresholdTwo = true;
            AkSoundEngine.SetState("SeedBossPhase", "PhaseTwo");
        }



        if (bossHPPercent > decayFloor)
        {
            bossCurrentHP = bossCurrentHP - (hpDecayPerSecond * bossMaxHP * Time.deltaTime);
            
        }

        if (bossCurrentDR > 0f)
        {
            bossCurrentDR = bossCurrentDR - (bossDRDecayPerSecond * Time.deltaTime);
        }
        else
        {
            bossCurrentDR = 0f;
        }

   
    }

    public void Attack()
    {
        attackTimeTotal = attackTimeTotal + Time.deltaTime;
        attackTime = attackTime + Time.deltaTime;

        Vector3 playerPos = player.transform.position;

        Vector3 attackPos = new Vector3((playerPos.x + Random.Range(-6f, 6f)), (playerPos.y + Random.Range(-6f, 6f)), 0f);

        float randomSize = Random.Range(1f, 2f);

        float randomSquare = Random.Range(0f, 1f);

        Vector3 scale = new Vector3(randomSize, randomSize, 1f);

        Quaternion rotation = Quaternion.identity;

        bool square = false;

        //create flow of boss attack parameters based on boss currnt hp state

        if (thresholdTwo)
        {
            var t = Mathf.Min(attackTimeTotal / secondMultThresholdTwo, 1f);

            if (randomSquare < squareAtThresholdTwo)
            {
                square = true;
                scale = new Vector3(scale.x, (scale.y * 30f), 1f);
                rotation = Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f));
            }

            currentSecondsForAttack = attacksPerSecondThresholdTwo / (t + 1f);

        }
        else if (thresholdOne)
        {
            var t = Mathf.Min(attackTimeTotal / secondMultThresholdOne, 1f);

            if (randomSquare < squareAtThresholdOne)
            {
                square = true;
                scale = new Vector3(scale.x, (scale.y * 30f), 1f);
                rotation = Quaternion.Euler(0f, 0f, Random.Range(-25f, 25f));
            }

            currentSecondsForAttack = attacksPerSecondThresholdOne / (t + 1f);

        }
        else
        {
            var t = Mathf.Min(attackTimeTotal / secondMultNoThreshold, 1f);

            currentSecondsForAttack = attacksPerSecondNoThreshold / (t + 1f);

        }

        if (attackTime > currentSecondsForAttack)
        {
            explosionSpawner.CreateDangerZone(5000, 1f, attackPos, true, false, false, scale, square, rotation, 4, false);
            attackTime = 0f;

            //Audio Section
            //UpdateExplosionVoicePosition();
            AkSoundEngine.PostEvent("PlayBossSeedAttack", explosionSpawner.gameObject);
        }
               
    }

    public void KillNearEnemies()
    {
        Physics2D.OverlapCircle(this.transform.position, 1.8f, hitFilter, hitBuffer);

        foreach (Collider2D enemyCollider in hitBuffer)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die();
            }
        }
    }


    public void Die()
    {
        dying = true;

        // stop the player from inputting anything
        // invincible the player
        // stop timer
        // hide ui?
        // reverse enemies

        if (!deathSetup)
        {
            Vector3 pos = this.transform.position - player.transform.position;

            deathContainer.SetActive(true);

            deathAnimRotator.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(pos.x, pos.y, 0f)).eulerAngles;

            deathSetup = true;

        }

        // if sufficiently through death animation hide deathwall

       // mission success screen
       // loop or return to menu

    }

    public void DieUpdate()
    {
        float angle = Random.Range(5f, 30f);

        var bloodRad1 = bloodPSOne.shape;
        var bloodRad2 = bloodPSTwo.shape;

        bloodRad1.angle = angle;
        bloodRad2.angle = angle;

        AnimatorStateInfo animState = deathAnimator.GetCurrentAnimatorStateInfo(0);

        if (animState.normalizedTime >= .1f)
        {
            bossColorPSParent.SetActive(false);
            bossBody.SetActive(false);
        }

        if (!triggerDeathPS)
        {
            shakeMult = shakeMult + (shakerLoopMultperSecond * Time.deltaTime);

            shaker.GenerateImpulse(baseShake * shakeMult);
   
        }

        if (animState.normalizedTime >= 0.95f && !triggerDeathPS)
        {
            deathPS.Play();
            deathPS.Emit(3);
            triggerDeathPS = true;
            shaker.GenerateImpulse(13f);
        }
       
        if (triggerDeathPS)
        {
            var shape = deathPS.shape;
            
            deathPSRadius = deathPSRadius + (radiusPerSecond * Time.deltaTime);

            shape.radius = deathPSRadius;

            if(deathFinishedCurrent >= deathFinishedTime)
            {

                deathFinished = true;
            }
            else
            {
                deathFinishedCurrent = deathFinishedCurrent + Time.deltaTime;
            }

        }

    }

    public void SetDistance(int spawnX)
    {
        bossSpawnX = spawnX;
    }

    private void UpdateExplosionVoicePosition()
    {
        //Audio Section
        if (explosionSpawner != null)
        {
            //Sound is coming from Left of player
            if (explosionSpawner.gameObject.transform.position.x < Player.activePlayer.transform.position.x)
            {
                AkSoundEngine.SetRTPCValue("ExplosionSpeakerPan_LR", 0 - Vector3.Distance(Player.activePlayer.transform.position, explosionSpawner.gameObject.transform.position));
            }
            //Sound is coming from right of player
            else if (explosionSpawner.gameObject.transform.position.x > Player.activePlayer.transform.position.x)
            {
                AkSoundEngine.SetRTPCValue("ExplosionSpeakerPan_LR", Vector3.Distance(Player.activePlayer.transform.position, explosionSpawner.gameObject.transform.position));
            }
        }
        return;
    }

    //Loadable Interface Functions
    public string LoadLabel()
    {
        return "Great Evil";
    }

    public int LoadPriority()
    {
        return 1000;
    }

    public void Setup()
    {

    }

    public void Load(World world)
    {
        world.level.boss = this;
    }
}
