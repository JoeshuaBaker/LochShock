using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;
using UnityEngine.Experimental.Rendering.Universal;

public class Player : MonoBehaviour
{
    public static Player activePlayer;

    public StatBlock stats;
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
    public Transform hpBar;
    public Gun[] guns;
    public World world;
    public int maxHp
    {
        get
        {
            return (int)stats.playerStats.health;
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
    private Vector2 mouseDirection;
    public Light2D playerVisionCone;
    public Light2D playerVisionProximity;
    public float orbsHeld;
    public Inventory inventory;
    ContactFilter2D hitFilter;
    List<Collider2D> hitBuffer;

    private void Awake()
    {
        activePlayer = this;
    }
    
    private void Start() {
        currentHp = maxHp;
        hitbox = GetComponent<Collider2D>();
        hitBuffer = new List<Collider2D>();
        hitFilter = new ContactFilter2D
        {
            useTriggers = false,
            layerMask = 1 << LayerMask.NameToLayer("Enemy"),
            useLayerMask = true
        };
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
        UpdateStatBlocks();
        Move();
        Physics();
        MouseAim();
        Shoot();
        SetVision();
    }

    public void SetVision()
    {
        float totalVis = stats.playerStats.totalVision;
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
            stats.playerStats.totalVision = Mathf.Min (stats.playerStats.totalVision + 0.0005f, 1f);
            
            if ( offPathCounter > 0)
            {
                stats.playerStats.totalVision = Mathf.Min (stats.playerStats.totalVision + 0.04f, 1f);
                offPathCounter = offPathCounter - 1f;
            }
        } 
        else
        {
            stats.playerStats.totalVision = Mathf.Max (stats.playerStats.totalVision - 0.0025f, 0f);

            if (offPathCounter < 10)
            {
                if (stats.playerStats.totalVision > .2f)
                {
                    stats.playerStats.totalVision = Math.Max(stats.playerStats.totalVision - 0.04f, 0f);
                }
                offPathCounter = offPathCounter + 1f;

            }
        }

        playerVisionCone.pointLightInnerRadius = Mathf.Max (totalVis * stats.playerStats.visionConeRadius, 4f);
        playerVisionCone.pointLightOuterRadius = playerVisionCone.pointLightInnerRadius * 4.3f;
        playerVisionCone.pointLightInnerAngle = Mathf.Max (totalVis * stats.playerStats.visionConeAngle, 10f);
        playerVisionCone.pointLightOuterAngle = playerVisionCone.pointLightInnerAngle * 4.3f;
        playerVisionCone.intensity = Mathf.Min(totalVis * 3.3f, 1f);

        playerVisionProximity.pointLightOuterRadius = Mathf.Max(totalVis * stats.playerStats.visionProximityRadius, 3.5f);
        playerVisionProximity.intensity = Mathf.Min(totalVis * 3.3f, 1f);

    }

    public void UpdateHp(int hpChange) 
    {
        currentHp += hpChange;
        if(currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        else if(currentHp < 0)
        {
            currentHp = maxHp;
        }
        hpBar.localScale = new Vector3((float)currentHp/(float)maxHp, 1f, 1f);
    }

    private void Move() 
    {
        Vector2 holdDirection = new Vector2(0,0);
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            holdDirection.y += 1;
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            holdDirection.y -= 1;
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            holdDirection.x -= 1;
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            holdDirection.x += 1;
        }

        holdDirection = holdDirection.normalized;

        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentVel = holdDirection * slowVel;
            leftRightDrift += holdDirection.x * slowFastRatio * Time.deltaTime;
        }
        else
        {
            currentVel = holdDirection * fastVel;
            leftRightDrift += holdDirection.x * Time.deltaTime;
        }

        if(holdDirection.x == 0 && leftRightDrift != 0f)
        {
            if(Math.Abs(leftRightDrift) < Time.deltaTime)
            {
                leftRightDrift = 0f;
            }
            else if(leftRightDrift > 0f)
            {
                leftRightDrift -= Time.deltaTime;
            }
            else
            {
                leftRightDrift += Time.deltaTime;
            }
        }

        this.transform.position = new Vector3(
            this.transform.position.x + currentVel.x * Time.deltaTime,
            this.transform.position.y + currentVel.y * Time.deltaTime,
            this.transform.position.z
        );
    }

    private void Physics()
    {
        int result = Physics2D.OverlapCircle(this.transform.position.xy(), hitbox.bounds.size.x / 2f, hitFilter, hitBuffer);
        if(result > 0)
        {
            TakeDamageFromEnemy(-1);
        }
    }

    private void TakeDamageFromEnemy(int damage)
    {
        UpdateHp(damage);
        Physics2D.OverlapCircle(this.transform.position.xy(), onHitKillRadius, hitFilter, hitBuffer);

        foreach (Collider2D enemyCollider in hitBuffer)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die((enemy.transform.position - this.transform.position).magnitude*onHitKillDistanceDelay);  
            }
        }
    }

    private void MouseAim()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 dirToMouse = worldMouse - this.transform.position;
        Vector2 xy = new Vector2(dirToMouse.x, dirToMouse.y);
        xy = xy.normalized;
        mouseDirection = xy;

        foreach (var gun in guns)
        {
            gun.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(xy.x, xy.y, 0f)).eulerAngles;
            gun.emitter.Direction = xy;
        }

        DrawSprites();

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

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Sprite[] limbDriftSpritesUp = limbSpritesUp[index];
            limbRenderer.sprite = limbDriftSpritesUp[lowestIndex];
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Sprite[] limbDriftSpritesDown = limbSpritesDown[index];
            limbRenderer.sprite = limbDriftSpritesDown[lowestIndex];
        }
    }

    private void Shoot()
    {
        foreach(var gun in guns) 
        {
            gun.Shoot();
        }
    }

    private void UpdateStatBlocks()
    {
        var allStats = inventory.GetItemStats();
        if(guns.Length > 0)
        {
            allStats.Add(guns[0].stats);
        }
        allStats.Add(this.stats);

        var combinedStats = StatBlock.Combine(allStats);
        if(guns.Length > 0)
        {
            guns[0].ApplyStatBlock(combinedStats);
        }
    }

}
