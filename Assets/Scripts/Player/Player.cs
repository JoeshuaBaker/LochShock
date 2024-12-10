 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BulletHell;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : BulletCollidable
{
    public static Player activePlayer;

    //public StatBlock stats;
    //public StatBlock combinedStats;
    //public List<StatBlock> allStats;

    public StatBlock baseStats;
    public StatBlock scalingStats;
    public CombinedStatBlock Stats
    {
        get { return combinedNewStats; }
    }
    public CombinedStatBlock combinedNewStats;
    public List<Buff.Instance> buffs;
    public bool onPath;
    public float offPathCounter;
    public Collider2D hitbox;
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer limbRenderer;
    public string spriteSheetDirectory;
    public string bodySpriteName;
    public string bodySpriteRecoilName;
    public string[] limbSpriteName;
    public string[] limbSpriteUpName;
    public string[] limbSpriteDownName;
    public float limbTransitionTime = 0.5f;
    public World world;
    public GameplayUI gameplayUI;
    public int maxHp = 5;
    public int currentHp = 5;
    public float fastVel = 1.5f;
    public float slowVel = 0.5f;
    public float onHitKillRadius = 6f;
    public float bombKillDistanceDelay = 0.035f;
    public Vector3 distSinceLastProjectileTick = Vector3.zero;
    private float slowFastRatio = 0.5f / 1.5f;
    public float leftRightDrift = 0f;
    private Vector2 currentVel = new Vector2(0f, 0f);
    public Sprite[] bodySprites;
    public Sprite[] bodyRecoilSprites;
    public bool recoilCoolDown;
    public Sprite[][] limbSprites;
    public Sprite[][] limbSpritesUp;
    public Sprite[][] limbSpritesDown;
    private Vector2[] vectors;
    public Vector2 lookDirection;
    public Vector2 lookPosition;
    private float gamepadLookRingSize;
    private Vector2 lookDiff;
    private float lastLookTime;
    private Vector2 moveDirection;
    public Grapple grapplingHook;
    public float grappleCoolDownCurrent;
    public float grapplingCoolDownBase = 3f;
    public float invincibilityTime;
    public float invincibilityOnHit;
    public GameObject invincibleShield;
    public Animator invincibleShieldAnimator;
    public float smallBombSize;

    public ParticleSystem trailPS;

    public Light2D playerVisionProximity;
    public float orbsHeld;
    public float timeSinceOrbUsed;
    public bool orbCharged;
    public int orbsChargedNumber;
    public Inventory inventory;
    ContactFilter2D hitFilter;
    List<Collider2D> hitBuffer;
    private float secondTimer = 0;

    [Header("Death Animation Components")]
    public bool dying;
    public bool isDead;
    public float isDeadTimer;
    public ParticleSystem damagePS;
    public ParticleSystem diePS;
    public ParticleSystem dieWallPS;
    public ParticleSystem gunDropPS;
    public ParticleSystem bombRingPS;
    public ParticleSystem bombRingRedPS;
    public GameObject ring;
    public Animator ringAnimator;
    public GameObject slashParent;
    public GameObject slashOne;
    public GameObject slashTwo;
    public GameObject mechDying;
    public SpriteRenderer mechShadow;
    public SpriteRenderer hitboxSprite;
    public CinemachineImpulseSource playerShake;
    public ExplosionSpawner explosionSpawner;

    [Header("Lost Arrow")]
    public GameObject arrowParent;
    public Animator arrowAnimator;
    public SpriteRenderer arrowPart1;
    public SpriteRenderer arrowPart2;
    public float arrowVisTimer;
    public float arrowVisTimerPrevFrame;
    public float arrowVisTimerMax;
    public float arrowPulseTimer;
    public float arrowAlpha;
    public float arrowAlphaMax;
    public float arrowAlphaPulse;
    public float arrowAlphaPulseMax;
    public float pulseSpeedBonus;
    public float arrowSecondsToDisplay;
    public bool pulseUp = true;
    public bool pathAbove;
    public Tile tiletest;
    public bool posForArrowChecked;
    public bool decayVisTimerToZero;

    [Header("Boss Info")]
    public bool bossDead;

    //vision variables
    public float totalVision = 1f;
    private float visionConeAngle = 20f;
    private float visionConeRadius = 7f;
    private float visionProximityRadius = 10f;
    private InputDevice lastUsedInputDevice;

    //Audio Variables
    private bool canPlay = true;


    private void Awake()
    {
        activePlayer = this;
    }

    private void Start()
    {
        inventory.Setup();
        world.SetupContext();
        UpdateStatBlocks();

        gamepadLookRingSize = Camera.main.ScreenToWorldPoint(new Vector3(0f, 1f, -Camera.main.transform.position.z)).magnitude / 2.25f;
        Debug.Log(gamepadLookRingSize);
        maxHp = (int)baseStats.GetStatValue<Health>();
        currentHp = maxHp;
        hitbox = GetComponent<Collider2D>();
        hitBuffer = new List<Collider2D>();
        hitFilter = new ContactFilter2D
        {
            useTriggers = false,
            layerMask = 1 << LayerMask.NameToLayer("Enemy"),
            useLayerMask = true
        };
        buffs = new List<Buff.Instance>();
        combinedNewStats = new CombinedStatBlock();

        slowFastRatio = slowVel / fastVel;

        if (bodyRenderer == null || limbRenderer == null)
        {
            Debug.LogError("Setup spriterenderers on Player " + this.name);
        }

        string bodyPath = spriteSheetDirectory + bodySpriteName;
        bodySprites = Resources.LoadAll<Sprite>(bodyPath);

        string bodyRecoilPath = spriteSheetDirectory + bodySpriteRecoilName;
        bodyRecoilSprites = Resources.LoadAll<Sprite>(bodyRecoilPath);

        limbSprites = new Sprite[limbSpriteName.Length][];
        limbSpritesUp = new Sprite[limbSpriteName.Length][];
        limbSpritesDown = new Sprite[limbSpriteName.Length][];

        for (int i = 0; i < limbSprites.Length; i++)
        {
            limbSprites[i] = Resources.LoadAll<Sprite>(spriteSheetDirectory + limbSpriteName[i]);
            limbSpritesUp[i] = Resources.LoadAll<Sprite>(spriteSheetDirectory + limbSpriteUpName[i]);
            limbSpritesDown[i] = Resources.LoadAll<Sprite>(spriteSheetDirectory + limbSpriteDownName[i]);
        }

        vectors = new Vector2[bodySprites.Length];
        float degrees = 360.0f / (float)vectors.Length;
        float currentDegrees = 0;
        for (int i = 0; i < vectors.Length; i++)
        {
            Vector3 v3 = (Quaternion.Euler(0, 0, currentDegrees) * Vector3.right);
            vectors[i].x = v3.x;
            vectors[i].y = v3.y;
            currentDegrees += degrees;
        }

        //Audio Section
        AkSoundEngine.SetState("ArrowVisibility", "ArrowInvisible");
    }

    private void Update()
    {
        if (World.activeWorld.paused || isDead)
        {
            if (isDead)
            {
                UpdateUI();
            }
            return;
        }
        if (!bossDead)
        {
            CheckDeath();
            DecayInvincible();
            UpdateStatBlocks();
            OnSecond();
            Move();
            Physics();
            DrawSprites();
            LookAndShoot();
            SetVision();
        }
        else
        {
            SetInvincible(1f);
        }

        UpdateInventory();
        UpdateUI();

        //Debug.Log(this.Stats.GetCombinedStatValue<Pierce>(World.activeWorld.worldStaticContext));
    }

    public void MoveEvent(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        lastUsedInputDevice = context.action.activeControl.device;
    }

    public void LookEvent(InputAction.CallbackContext context)
    {
        if (context.action.activeControl.device is Mouse)
        {
            lastUsedInputDevice = Mouse.current;
            var value = context.ReadValue<Vector2>();
            Vector3 mousePos = Vector3.zero;
            if (Camera.main != null)
            {
               mousePos = Camera.main.ScreenToWorldPoint(new Vector3(value.x, value.y, -Camera.main.transform.position.z));
            }
            lookPosition = mousePos.xy();
            lookDiff = (mousePos - this.transform.position).xy();
            lookDirection = lookDiff.normalized;
            lastLookTime = Time.realtimeSinceStartup;

            if (world.paused || isDead || bossDead)
            {
                Crosshair.activeCrosshair.UpdateCrosshair(lookPosition, false, 0, "");
            }
        }
        else if (context.action.activeControl.device is Gamepad)
        {
            lastUsedInputDevice = Gamepad.current;
            var value = context.ReadValue<Vector2>();
            var magnitude = value.magnitude;
            if (magnitude > 0.40f)
            {
                lookDirection = value.normalized;
                lookPosition = transform.position.xy() + lookDirection * gamepadLookRingSize;
                lookDiff = (lookPosition.xyz() - this.transform.position).xy();
                lastLookTime = Time.realtimeSinceStartup;
            }
        }
    }

    public void FireEvent(InputAction.CallbackContext context)
    {
        if (world.paused || isDead)
        {
            return;
        }

        bool buttonValue = context.ReadValueAsButton();
        if(inventory.activeGun.shooting && context.canceled)
        {
            inventory.activeGun.shooting = false;
        }
        else if(!inventory.activeGun.shooting && context.started)
        {
            inventory.activeGun.shooting = true;
        }

        lastUsedInputDevice = context.action.activeControl.device;
    }

    public void BombEvent(InputAction.CallbackContext context)
    {
        if (world.paused || isDead)
        {
            return;
        }

        lastUsedInputDevice = context.action.activeControl.device;
        bool pressed = context.ReadValueAsButton();
        if (context.started && pressed && !dying)
        {
            if (!orbCharged && orbsHeld > 0)
            {
                orbCharged = true;
                orbsChargedNumber = Mathf.Min((int)orbsHeld, 5);

                Bomb(false);
            }
        }
    }

    public void SwitchWeaponsEvent(InputAction.CallbackContext context)
    {
        if (world.paused || isDead)
        {
            return;
        }

        lastUsedInputDevice = context.action.activeControl.device;
        bool pressed = context.ReadValueAsButton();
        if (context.started && pressed)
        {
            inventory.SwitchWeapons();
        }
    }

    public void InventoryEvent(InputAction.CallbackContext context)
    {
        if (isDead)
        {
            return;
        }

        lastUsedInputDevice = context.action.activeControl.device;
        bool pressed = context.ReadValueAsButton();
        if (context.started && pressed)
        {
            inventory.OpenCloseInventory();
            inventory.activeGun.shooting = false;
        }
    }

    public void ItemEvent(InputAction.CallbackContext context)
    {
        if (world.paused || isDead)
        {
            return;
        }

        lastUsedInputDevice = context.action.activeControl.device;
        bool pressed = context.ReadValueAsButton();
        if (context.started && pressed)
        {
            bool wasActivated = inventory.ActivateActiveItem();

            //Audio Section
            if (wasActivated)
            {
                AkSoundEngine.PostEvent("PlayItemUse", this.gameObject);
            }
            else if(inventory.activeItem != null)
            {
                AkSoundEngine.PostEvent("PlayItemOnCD", this.gameObject);
            }
        }
    }

    public void GrappleEvent(InputAction.CallbackContext context)
    {
        if (world.paused || isDead)
        {
            return;
        }

        lastUsedInputDevice = context.action.activeControl.device;
        bool pressed = context.ReadValueAsButton();
        if (context.started && pressed && !dying && !bossDead)
        {
            grapplingHook.StartGrapple(grapplingCoolDownBase);
        }
    }

    public void UpdateInventory()
    {
        timeSinceOrbUsed += Time.deltaTime;

        var psmain = trailPS.main;

        psmain.startLifetime = 0.5f + Mathf.Min((timeSinceOrbUsed * 0.025f), 1.5f);
    }

    public void UpdateHp(int hpChange)
    {
        currentHp += hpChange;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        else if (currentHp < 0)
        {
            currentHp = 0;
        }
    }

    public void CollectOrb()
    {
        if(orbsHeld >= 4 && !orbCharged)
        {
            inventory.Orb(false,true);
        }
        else if (orbCharged)
        {
            inventory.Orb();
            orbCharged = false;
        }
        else
        {
            inventory.Orb(true);
        }
        orbsHeld += 1;

    }

    public void SetVision()
    {
        Tile tileUnderPlayer = world.TileUnderPlayer(this.transform.position);

        if (tileUnderPlayer != null)
        {
            onPath = tileUnderPlayer.collider2d.OverlapPoint(this.transform.position.xy());
        }
        else
        {
            onPath = false;
        }

        if (onPath)
        {
            totalVision = Mathf.Min(totalVision + (0.1f * Time.deltaTime), 1f);

            if (offPathCounter > 0)
            {
                totalVision = Mathf.Min(totalVision + (1f * Time.deltaTime), 1f);
                offPathCounter = offPathCounter - Time.deltaTime;
            }

            arrowVisTimer = Mathf.Max(arrowVisTimer -= Time.deltaTime * 15f, 0f);
        }
        else
        {
            totalVision = Mathf.Max(totalVision - (0.2f * Time.deltaTime), 0f);

            if (offPathCounter < 0.2f)
            {
                if (totalVision > .2f)
                {
                    totalVision = Math.Max(totalVision - (1f * Time.deltaTime), 0f);
                }
                offPathCounter = offPathCounter + Time.deltaTime;
            }

            if (decayVisTimerToZero)
            {
                arrowVisTimer = Mathf.Max(arrowVisTimer -= Time.deltaTime * 15f, 0f);
                if(arrowVisTimer <= 0f)
                {
                    decayVisTimerToZero = false;
                }
            }
            else
            {
                arrowVisTimer = Mathf.Min(arrowVisTimer += Time.deltaTime, arrowVisTimerMax);
            }
        }

        //lost arrow section

        float arrowTimeScale = Mathf.Max((arrowVisTimer - arrowSecondsToDisplay) / (arrowVisTimerMax - arrowSecondsToDisplay) , 0f );

        if(pulseUp)
        {
            arrowPulseTimer += Time.deltaTime * (pulseSpeedBonus * arrowTimeScale);

            if(arrowPulseTimer >= 1f)
            {
                arrowPulseTimer = 1f - (arrowPulseTimer - 1f);
                pulseUp = false;
            }

        }
        else
        {
            arrowPulseTimer -= Time.deltaTime * (pulseSpeedBonus * arrowTimeScale);

            if(arrowPulseTimer <= 0f)
            {
                arrowPulseTimer = 0f - arrowPulseTimer;
                pulseUp = true;
            }

        }

        arrowAlphaPulse = 1f + ((arrowAlphaPulseMax - 1f) * arrowPulseTimer);

        arrowAlpha = arrowTimeScale * arrowAlphaMax * arrowAlphaPulse;

        var aP1c = arrowPart1.color;
        var aP2c = arrowPart2.color;

        aP1c.a = arrowAlpha;
        aP2c.a = arrowAlpha;

        arrowPart1.color = aP1c;
        arrowPart2.color = aP2c;

        arrowAnimator.speed = Mathf.Max(arrowTimeScale * 2f, 1f);

        if(arrowTimeScale > 0f && arrowVisTimer > arrowVisTimerPrevFrame)
        {
            //Audio Section
            AkSoundEngine.SetState("ArrowVisibility", "ArrowVisible");
            
            int playerX = (int)this.transform.position.x;

            Tile tileOnPlayerX = world.RandomPathTileAtXPosition(playerX);

            tiletest = tileOnPlayerX;

            if (tileOnPlayerX != null && !posForArrowChecked)
            {
                if (this.transform.position.y > tileOnPlayerX.transform.position.y)
                {
                    arrowParent.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                {
                    arrowParent.transform.localScale = new Vector3(1f, -1f, 1f);
                }
            }

            posForArrowChecked = true;

        }
        if( arrowVisTimer < arrowVisTimerPrevFrame)
        {
            posForArrowChecked = false;
            decayVisTimerToZero = true;
            //Audio Section
            AkSoundEngine.SetState("ArrowVisibility", "ArrowInvisible");
        }

        arrowVisTimerPrevFrame = arrowVisTimer;
      
      

        //arrow section end

        if (inventory.activeGun != null)
        {
            inventory.activeGun.visionCone.gameObject.SetActive(true);
            inventory.activeGun.beamLight.gameObject.SetActive(true);
            inventory.activeGun.UpdateVisionCone(totalVision, visionConeRadius, visionConeAngle);
        }

        if (inventory.inactiveGun != null)
        {
            inventory.inactiveGun.visionCone.gameObject.SetActive(false);
            inventory.inactiveGun.beamLight.gameObject.SetActive(false);
        }

        playerVisionProximity.pointLightOuterRadius = Mathf.Max(totalVision * visionProximityRadius, 3.5f);
        playerVisionProximity.intensity = Mathf.Min(totalVision * 3.3f, 1f);

    }

    private void Move()
    {
        if (dying)
        {
            slowVel *= 0.98f;
            fastVel *= 0.98f;
        }

        currentVel = moveDirection * fastVel;
        leftRightDrift += moveDirection.x * Time.deltaTime;

        if (Mathf.Abs(moveDirection.x) < 0.1f && leftRightDrift != 0f)
        {
            if (Math.Abs(leftRightDrift) < Time.deltaTime)
            {
                leftRightDrift = 0f;
            }
            else if (leftRightDrift > 0f)
            {
                leftRightDrift -= Time.deltaTime;
            }
            else
            {
                leftRightDrift += Time.deltaTime;
            }
        }

        currentVel = currentVel * grapplingHook.playerSlow;

        this.transform.position = new Vector3(
            this.transform.position.x + currentVel.x * Time.deltaTime,
            this.transform.position.y + currentVel.y * Time.deltaTime,
            this.transform.position.z
        );
    }

    private void Physics()
    {
        grappleCoolDownCurrent = grapplingHook.grappleCD;

        this.transform.position = new Vector3(
            this.transform.position.x + grapplingHook.grappleVectorToPlayer.x,
            this.transform.position.y + grapplingHook.grappleVectorToPlayer.y,
            this.transform.position.z
        );

        int result = Physics2D.OverlapCircle(this.transform.position.xy(), hitbox.bounds.size.x / 2f, hitFilter, hitBuffer);
        if (result > 0)
        {
            int touchDamage = 0;
            foreach (Collider2D enemyCollider in hitBuffer)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TouchPlayer();
                    if(enemy.dealsTouchDamage)
                    {
                        touchDamage = Mathf.Max(touchDamage, enemy.touchDamage);
                    }
                }
            }

            if(invincibilityTime == 0f && touchDamage > 0)
            {
                TakeDamageFromEnemy(touchDamage*-1);
            }
        }
    }

    public void TakeDamageFromEnemy(int damage)
    {
        if (invincibilityTime > 0 || dying)
        {
            return;
        }

        UpdateHp(damage);

        damagePS.Stop();
        damagePS.Play();

        //Audio Section
        AkSoundEngine.PostEvent("PlayBombHit", this.gameObject);

        if (currentHp == 0)
        {
            KillSelf();

            diePS.Play();

            mechDying.SetActive(true);

            //Audio Section
            AkSoundEngine.PostEvent("PlayDeathStart", this.gameObject);

        }
        else
        {
            slashParent.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
            slashOne.SetActive(false);
            slashOne.SetActive(true);
        }

        Bomb(true);
    }

    public void SetInvincible(float time)
    {
        if (time > invincibilityTime)
        {
            invincibilityTime = time;
            invincibleShield.SetActive(true);
            invincibleShieldAnimator.speed = 1f;
        }
    }

    public void DecayInvincible()
    {
        invincibilityTime = Mathf.Max(invincibilityTime - Time.deltaTime, 0f);
        if (invincibilityTime <= .5f)
        {
            invincibleShieldAnimator.speed = 2f;
            if( invincibilityTime <= 0f)
            {
                invincibleShield.SetActive(false);
            }
        }
        
    }

    public void Bomb(bool isHit, bool isSmall = false)
    {
        float mult = 1f;

        if (isSmall)
        {
            mult = smallBombSize;
        }
        if(!isSmall)
        {
            timeSinceOrbUsed = 0f;
        }


        SetInvincible(invincibilityOnHit * mult);

        if (isHit)
        {

            bombRingRedPS.Stop();
            bombRingRedPS.Play();
            world.ClearAllEnemyBullets();

            //old damage ring
            //ringAnimator.Play("RingExpandExtraLargeRed");
        }
        else
        {
            var brPSm = bombRingPS.main;
            var brrPSm = bombRingRedPS.main;

            if (isSmall)
            {
                brPSm.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.6f);
                brPSm.startSpeed = new ParticleSystem.MinMaxCurve(70f, 100f);
            }
            else
            {
                brPSm.startLifetime = brrPSm.startLifetime;
                brPSm.startSpeed = brrPSm.startSpeed;
                world.ClearAllEnemyBullets();
            }

            bombRingPS.Stop();
            bombRingPS.Play();

            //Audio Section
            AkSoundEngine.PostEvent("PlayBombAttack", this.gameObject);

            //old damage ring
            // ringAnimator.Play("RingExpandExtraLarge");
        }

        ring.transform.position = this.transform.position;

        playerShake.GenerateImpulse(2f * mult);

        Physics2D.OverlapCircle(this.transform.position.xy(), onHitKillRadius * mult, hitFilter, hitBuffer);

        foreach (Collider2D enemyCollider in hitBuffer)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.BombHit(bombKillDistanceDelay * mult);
            }
        }
    }

    private void CheckDeath()
    {
        if (damagePS.isStopped == true && currentHp == 0 && isDead == false)
        {
            Execute();
        }
    }

    private void DrawSprites()
    {
        float lowestDist = float.MaxValue;
        int lowestIndex = -1;
        for (int i = 0; i < vectors.Length; i++)
        {
            float dx = vectors[i].x - lookDirection.x;
            float dy = vectors[i].y - lookDirection.y;
            float dist = Mathf.Abs(dx) + Mathf.Abs(dy);
            if (dist < lowestDist)
            {
                lowestDist = dist;
                lowestIndex = i;
            }
        }

        //body sprite selection
        if(inventory.activeGun.IsReady() && inventory.activeGun.shooting && !recoilCoolDown)
        {
            bodyRenderer.sprite = bodyRecoilSprites[lowestIndex];
            recoilCoolDown = true;
        }
        else
        {
            bodyRenderer.sprite = bodySprites[lowestIndex];
            recoilCoolDown = false;
        }

        //limb sprite direction
        float ceil = limbTransitionTime * (limbSprites.Length / 2) + 0.01f;
        float floor = -1 * ceil;
        leftRightDrift = Mathf.Clamp(leftRightDrift, floor, ceil);
        int regionsSearched = 0;
        float region = limbTransitionTime;
        while (Math.Abs(leftRightDrift) > Math.Abs(region))
        {
            region += limbTransitionTime;
            regionsSearched++;
        }

        int index = limbSprites.Length / 2;
        if (leftRightDrift > 0)
        {
            index += regionsSearched;
        }
        else
        {
            index -= regionsSearched;
        }

        if (index < 0 || index > limbSprites.Length)
        {
            Debug.LogError("Could not find limb lookup for leftRightDrift value of " + leftRightDrift);
            return;
        }

        Sprite[] limbDriftSprites = limbSprites[index];
        limbRenderer.sprite = limbDriftSprites[lowestIndex];

        if (moveDirection.y > 0)
        {
            Sprite[] limbDriftSpritesUp = limbSpritesUp[index];
            limbRenderer.sprite = limbDriftSpritesUp[lowestIndex];
        }
        else if (moveDirection.y < 0)
        {
            Sprite[] limbDriftSpritesDown = limbSpritesDown[index];
            limbRenderer.sprite = limbDriftSpritesDown[lowestIndex];
        }
    }

    private void LookAndShoot()
    {
        if (!dying)
        {
            if (lastUsedInputDevice is Mouse || lastUsedInputDevice is Keyboard)
            {
                Vector2 value = Mouse.current.position.ReadValue();
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(value.x, value.y, -Camera.main.transform.position.z));
                lookPosition = worldPoint;
                lookDiff = (worldPoint - this.transform.position).xy();
                lookDirection = lookDiff.normalized;
            }
            else
            {
                if (Time.realtimeSinceStartup - lastLookTime > Time.deltaTime)
                {
                    lookPosition = transform.position.xy() + lookDirection * gamepadLookRingSize;
                    lookDiff = (lookPosition.xyz() - this.transform.position).xy();
                }
            }

            inventory.activeGun.UpdateActiveGun(lookDirection, lookPosition);
            inventory.activeGun.Shoot();
        }
    }

    private void OnSecond()
    {
        secondTimer -= Time.deltaTime;

        if (secondTimer <= 0)
        {
            secondTimer = 1;
            foreach (OnSecondAction onSecondAction in combinedNewStats.combinedStatBlock.GetEvents<OnSecondAction>())
            {
                Item source = inventory.FindEventSource(onSecondAction);
                onSecondAction.OnSecond(source, this);
            }
        }
    }

    private void UpdateStatBlocks()
    {
        buffs.Clear();
        inventory.AggregateBuffs(buffs);

        var allNewStats = inventory.GetNewItemStats();
        allNewStats.Add(this.baseStats);
        allNewStats.Add(this.scalingStats);
        allNewStats.AddRange(buffs.Select(x => x.newStats));
        combinedNewStats.UpdateSources(allNewStats);
        inventory.activeGun.ApplyNewStatBlock(combinedNewStats);
        if(inventory.activeItem != null)
        {
            inventory.activeItem.ApplyStatBlock(combinedNewStats);
        }

        int newHp = (int) combinedNewStats.GetCombinedStatValue<Health>();
        if(newHp != maxHp)
        {
            if(newHp > maxHp)
            {
                int diff = newHp - maxHp;
                currentHp += diff;
                maxHp = newHp;
            }
            else
            {
                maxHp = newHp;
                currentHp = Mathf.Min(currentHp, maxHp);
            }
        }
    }

    public void UpdateUI()
    {
        if (gameplayUI != null)
        {
            //gameplayUI.SetAmmo(inventory.activeGun.magazine, inventory.activeGun.maxMagazine,
            //    inventory.inactiveGun == null ? -1 : inventory.inactiveGun.magazine, inventory.inactiveGun == null ? -1 : inventory.inactiveGun.maxMagazine);

            gameplayUI.SetHp(currentHp, maxHp);
            gameplayUI.SetOrbs((int)orbsHeld);
            gameplayUI.SetGrapple(grappleCoolDownCurrent, grapplingCoolDownBase);
            gameplayUI.SetMoney(inventory.scrap);

            if (isDead)
            {
                isDeadTimer = isDeadTimer + Time.deltaTime;
                if (isDeadTimer >= 2.33f)
                {
                    gameplayUI.ShowSignalLost();
                }

            }
        }
    }

    public void KillSelf()
    {
        slashParent.transform.localScale = new Vector3(1f, 1f, 1f);
        slashOne.SetActive(false);
        slashOne.SetActive(true);
        slashTwo.SetActive(false);
        slashTwo.SetActive(true);

        gunDropPS.Play();



        bodyRenderer.enabled = false;
        limbRenderer.enabled = false;
        mechShadow.enabled = false;

        // old damage ring
        //ring.transform.position = this.transform.position;
        //ringAnimator.Play("RingExpandExtraLargeRed");

        bombRingRedPS.Stop();
        bombRingRedPS.Play();

        playerShake.GenerateImpulse(3f);

        inventory.activeGun.visionCone.enabled = false;
        inventory.activeGun.beamLight.enabled = false;

        dying = true;
        return;
    }

    public void Execute()
    {
        explosionSpawner.CreateExplosionWithCrater(new Vector3(this.transform.position.x, (this.transform.position.y - 0.5f), this.transform.position.z), 3f);

        //Audio Section
        AkSoundEngine.PostEvent("PlayPlayerDeathExplosion", this.gameObject);

        ring.transform.position = this.transform.position;

        //old damage ring
        //ringAnimator.Play("RingExpandExtraLarge");

        bombRingPS.Stop();
        bombRingPS.Play();

        mechDying.SetActive(false);
        hitboxSprite.enabled = false;
        isDead = true;

        UpdateUI();
        return;
    }

    public override void ProcessCollision(ProjectileData projectile, RaycastHit2D hitInfo)
    {
        TakeDamageFromEnemy(-1);
    }
}
