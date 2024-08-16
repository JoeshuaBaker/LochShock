using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BulletHell;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{
    public static Player activePlayer;

    //public StatBlock stats;
    //public StatBlock combinedStats;
    //public List<StatBlock> allStats;

    public StatBlock baseStats;
    public CombinedStatBlock Stats {
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
    public string[] limbSpriteName;
    public string[] limbSpriteUpName;
    public string[] limbSpriteDownName;
    public float limbTransitionTime = 0.5f;
    public World world;
    public GameplayUI gameplayUI;
    public int maxHp
    {
        get
        {
            return (int)Stats.GetCombinedStatValue<Health>(World.activeWorld.worldStaticContext);
        }
    }
    public int currentHp = 100;
    public float fastVel = 1.5f;
    public float slowVel = 0.5f;
    public float onHitKillRadius = 6f;
    public float onHitKillDistanceDelay = 0.1f;
    public Vector3 distSinceLastProjectileTick = Vector3.zero;
    private float slowFastRatio = 0.5f / 1.5f;
    public float leftRightDrift = 0f;
    private Vector2 currentVel = new Vector2(0f, 0f);
    public Sprite[] bodySprites;
    public Sprite[][] limbSprites;
    public Sprite[][] limbSpritesUp;
    public Sprite[][] limbSpritesDown;
    private Vector2[] vectors;
    public Vector2 mouseDirection;
    private Vector2 moveDirection;
    public Grapple grapplingHook;
    public float grappleCoolDownCurrent;
    public float grapplingCoolDownBase = 3f;
    public float invincibilityTime;
    public float invincibilityOnHit;

    public ParticleSystem trailPS;
    
    public Light2D playerVisionProximity;
    public float orbsHeld;
    public float timeSinceOrbUsed;
    public Inventory inventory;
    ContactFilter2D hitFilter;
    List<Collider2D> hitBuffer;
    private float secondTimer = 0;

    [Header("Death Animation Components")]
    public bool dying;
    public bool isDead;
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

    //vision variables
    public float totalVision = 1f;
    private float visionConeAngle = 20f;
    private float visionConeRadius = 7f;
    private float visionProximityRadius = 10f;


    private void Awake()
    {
        activePlayer = this;
    }
    
    private void Start() {
        inventory.Setup();
        world.SetupContext();
        UpdateStatBlocks();

        currentHp = (int) baseStats.GetStatValue<Health>();
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

        if(bodyRenderer == null || limbRenderer == null)
        {
            Debug.LogError("Setup spriterenderers on Player " + this.name);
        }

        string bodyPath = spriteSheetDirectory + bodySpriteName;
        bodySprites = Resources.LoadAll<Sprite>(bodyPath);

        limbSprites = new Sprite[limbSpriteName.Length][];
        limbSpritesUp = new Sprite[limbSpriteName.Length][];
        limbSpritesDown = new Sprite[limbSpriteName.Length][];

        for(int i = 0; i < limbSprites.Length; i++)
        {
            limbSprites[i] = Resources.LoadAll<Sprite>(spriteSheetDirectory + limbSpriteName[i]);
            limbSpritesUp[i] = Resources.LoadAll<Sprite>(spriteSheetDirectory + limbSpriteUpName[i]);
            limbSpritesDown[i] = Resources.LoadAll<Sprite>(spriteSheetDirectory + limbSpriteDownName[i]);
        }

        vectors = new Vector2[bodySprites.Length];
        float degrees = 360.0f/(float)vectors.Length;
        float currentDegrees = 0;
        for(int i = 0; i < vectors.Length; i++)
        {
            Vector3 v3 = (Quaternion.Euler(0, 0, currentDegrees) * Vector3.right);
            vectors[i].x = v3.x;
            vectors[i].y = v3.y;
            currentDegrees += degrees;
        }
    }

    private void Update()
    {
        if (World.activeWorld.paused || isDead)
        {
            return;
        }

        CheckDeath();
        DecayInvincible();
        UpdateStatBlocks();
        OnSecond();
        Move();
        Physics();
        MouseAim();
        DrawSprites();
        Shoot();
        SetVision();
        UpdateInventory();
        UpdateUI();
    }

    public void MoveEvent(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void LookEvent(InputAction.CallbackContext context)
    {
        
        if(context.action.activeControl.device is Mouse)
        {
            var value = context.ReadValue<Vector2>();
            var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(value.x, value.y, -Camera.main.transform.position.z));
            mouseDirection = (mousePos - this.transform.position).xy().normalized;
        }
        else if(context.action.activeControl.device is Gamepad)
        {
            var value = context.ReadValue<Vector2>();
            if(value != Vector2.zero && value.magnitude > 0.25f)
            {
                mouseDirection = value.normalized;
            }
        }
    }

    public void FireEvent(InputAction.CallbackContext context)
    {
        inventory.activeGun.shooting = context.ReadValueAsButton();
    }

    public void BombEvent(InputAction.CallbackContext context)
    {
        bool pressed = context.ReadValueAsButton();
        if (context.started && pressed && !dying)
        {
            if (inventory.Orb())
            {
                timeSinceOrbUsed = 0f;
            }
        }
    }

    public void SwitchWeaponsEvent(InputAction.CallbackContext context)
    {
        bool pressed = context.ReadValueAsButton();
        if (context.started && pressed)
        {
            inventory.SwitchWeapons();
        }
    }

    public void InventoryEvent(InputAction.CallbackContext context)
    {
        bool pressed = context.ReadValueAsButton();
        if (context.started && pressed)
        {
            inventory.OpenCloseInventory();
        }
    }

    public void ItemEvent(InputAction.CallbackContext context)
    {
        inventory.ActivateActiveItem();
    }

    public void GrappleEvent(InputAction.CallbackContext context)
    {

    }

    public void UpdateInventory()
    {
        timeSinceOrbUsed += Time.deltaTime;

        var psmain = trailPS.main;

        psmain.startLifetime = 0.5f + Mathf.Min((timeSinceOrbUsed * 0.025f ) , 1.5f);
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
        orbsHeld += 1;
    }

    public void AddBuff(Buff.Instance buffInstance)
    {
        IEnumerable<Buff.Instance> matchingBuffs = buffs.Where(x => x.buff.buffName.Equals(buffInstance.buff.buffName, StringComparison.CurrentCultureIgnoreCase));

        if (buffInstance.buff.stackType == Buff.StackType.Stackable && matchingBuffs.Count() > 0)
        {
            Buff.Instance matchingBuff = matchingBuffs.First();
            matchingBuff.currentDuration = matchingBuff.buff.duration;
            if (matchingBuff.newStats.Stacks < matchingBuff.buff.stackLimit)
            {
                matchingBuff.newStats.Stacks += 1;
            }

            return;
        }

        //replace lowest duration buff if the buff is copyable and we are at stack limit
        else if (buffInstance.buff.stackType == Buff.StackType.Copyable && matchingBuffs.Count() >= buffInstance.buff.stackLimit)
        {
            Buff.Instance lowestDuration = matchingBuffs.First();

            foreach (var matchingBuff in matchingBuffs)
            {
                if (matchingBuff.currentDuration <= lowestDuration.currentDuration)
                {
                    lowestDuration = matchingBuff;
                }
            }

            buffs.Remove(lowestDuration);
        }

        buffs.Add(buffInstance);
    }

    public void SetVision()
    {
        Tile tileUnderPlayer = world.TileUnderPlayer(this.transform.position);

        if(tileUnderPlayer != null)
        {
            onPath = tileUnderPlayer.collider2d.OverlapPoint(this.transform.position.xy());
        }
        else
        {
            onPath = false;
        }

        if (onPath)
        {
            totalVision = Mathf.Min (totalVision + 0.0005f, 1f);
            
            if ( offPathCounter > 0)
            {
                totalVision = Mathf.Min (totalVision + 0.04f, 1f);
                offPathCounter = offPathCounter - 1f;
            }
        } 
        else
        {
            totalVision = Mathf.Max (totalVision - 0.0025f, 0f);

            if (offPathCounter < 10)
            {
                if (totalVision > .2f)
                {
                    totalVision = Math.Max(totalVision - 0.04f, 0f);
                }
                offPathCounter = offPathCounter + 1f;
            }
        }

        if(inventory.activeGun != null)
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && !dying && !grapplingHook.onCD)
        {
            grapplingHook.fireGrappling = true;
            grapplingHook.grappleCD = grapplingCoolDownBase;
        }

        this.transform.position = new Vector3(
            this.transform.position.x + grapplingHook.grappleVectorToPlayer.x,
            this.transform.position.y + grapplingHook.grappleVectorToPlayer.y,
            this.transform.position.z
        ); 

        int result = Physics2D.OverlapCircle(this.transform.position.xy(), hitbox.bounds.size.x / 2f, hitFilter, hitBuffer);
        if (result > 0)
        {
            if(invincibilityTime == 0f)
            {
                TakeDamageFromEnemy(-1);
                
            }
            else
            {
                foreach (Collider2D enemyCollider in hitBuffer)
                {
                    Enemy enemy = enemyCollider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.Die();
                    }
                }
            }
            
        }



    }

    public void TakeDamageFromEnemy(int damage)
    {
        if(invincibilityTime > 0)
        {
            return;
        }

        UpdateHp(damage);

        damagePS.Stop();
        damagePS.Play();

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
        if(time > invincibilityTime)
        {
            invincibilityTime = time;
        }
    }

    public void DecayInvincible()
    {
        invincibilityTime = Mathf.Max(invincibilityTime - Time.deltaTime, 0f);
    }

    public void Bomb(bool isHit)
    {

        SetInvincible(invincibilityOnHit);

        if (isHit)
        {
            bombRingRedPS.Stop();
            bombRingRedPS.Play();

            //old damage ring
            //ringAnimator.Play("RingExpandExtraLargeRed");
        }
        else
        {

            bombRingPS.Stop();
            bombRingPS.Play();
            
           //old damage ring
           // ringAnimator.Play("RingExpandExtraLarge");
        }

        //Audio Section
        AkSoundEngine.PostEvent("PlayWipe", this.gameObject);

        ring.transform.position = this.transform.position;

        playerShake.GenerateImpulse(2f);

        Physics2D.OverlapCircle(this.transform.position.xy(), onHitKillRadius, hitFilter, hitBuffer);

        foreach (Collider2D enemyCollider in hitBuffer)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die((enemy.transform.position - this.transform.position).magnitude * onHitKillDistanceDelay);
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

    private void MouseAim()
    {
        inventory.activeGun.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(mouseDirection.x, mouseDirection.y, 0f)).eulerAngles;
        inventory.activeGun.emitter.Direction = mouseDirection;
    }

    private void DrawSprites()
    {
        float lowestDist = float.MaxValue;
        int lowestIndex = -1;
        for (int i = 0; i < vectors.Length; i++)
        {
            float dx = vectors[i].x - mouseDirection.x;
            float dy = vectors[i].y - mouseDirection.y;
            float dist = Mathf.Abs(dx) + Mathf.Abs(dy);
            if (dist < lowestDist)
            {
                lowestDist = dist;
                lowestIndex = i;
            }
        }

        //body sprite selection
        bodyRenderer.sprite = bodySprites[lowestIndex];

        //limb sprite direction
        float ceil = limbTransitionTime * (limbSprites.Length / 2) + 0.01f;
        float floor = -1 * ceil;
        leftRightDrift = Mathf.Clamp(leftRightDrift, floor, ceil);
        int regionsSearched = 0;
        float region = limbTransitionTime;
        while(Math.Abs(leftRightDrift) > Math.Abs(region))
        {
            region += limbTransitionTime;
            regionsSearched++;
        }

        int index = limbSprites.Length / 2;
        if(leftRightDrift > 0)
        {
            index += regionsSearched;
        }
        else
        {
            index -= regionsSearched;
        }

        if(index < 0 || index > limbSprites.Length)
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

    private void Shoot()
    {
        if(!dying)
        {
            inventory.activeGun.Shoot();
            inventory.activeGun.UpdateActiveGun();
        }
    }

    private void OnSecond()
    {
        secondTimer -= Time.deltaTime;

        if(secondTimer <= 0)
        {
            secondTimer = 1;
            foreach(OnSecondAction onSecondAction in combinedNewStats.combinedStatBlock.GetEvents<OnSecondAction>())
            {
                onSecondAction.OnSecond(this);
            }
        }
    }

    private void UpdateStatBlocks()
    {
        foreach (var buff in buffs)
        {
            buff.currentDuration -= Time.deltaTime;
        }

        buffs.RemoveAll(buff => buff.currentDuration <= 0f);

        //var allStats = inventory.GetItemStats();
        //allStats.Add(this.stats);
        //allStats.AddRange(inventory.activeGun.stats);
        //allStats.AddRange(buffs.Select(x => x.stats));

        //this.allStats = allStats;
        //var combinedStats = StatBlock.Combine(allStats);
        //this.combinedStats = combinedStats;
        //inventory.activeGun.ApplyStatBlock(combinedStats);

        var allNewStats = inventory.GetNewItemStats();
        allNewStats.Add(this.baseStats);
        allNewStats.AddRange(buffs.Select(x => x.newStats));
        combinedNewStats.UpdateSources(allNewStats);
        inventory.activeGun.ApplyNewStatBlock(combinedNewStats);
    }

    public void UpdateUI()
    {
        if(gameplayUI != null)
        {
            gameplayUI.SetAmmo(inventory.activeGun.magazine, inventory.activeGun.maxMagazine,
                inventory.inactiveGun == null ? -1 : inventory.inactiveGun.magazine, inventory.inactiveGun == null ? -1 : inventory.inactiveGun.maxMagazine);

            gameplayUI.SetHp(currentHp, maxHp);
            gameplayUI.SetOrbs((int)orbsHeld);

            if(isDead)
            {
                gameplayUI.ShowSignalLost();
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
}
