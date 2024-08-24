using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeActivatable : Activatable
{
    public enum HitboxType
    {
        Circle,
        Box
    }
    public GameObject meleeWeaponParent;
    public Animator meleeWeaponVisual;
    public Collider2D hitbox;
    public HitboxType hitboxType = HitboxType.Circle;
    public bool isActive = false;
    public bool repeatable = false;
    public bool aimable = true;
    public float bonusRepeatsDurationThreshold = 5f;
    public int numRepeats = 1;
    private int modifiedRepeats
    {
        get { return numRepeats + (int)(duration / bonusRepeatsDurationThreshold); }
    }
    public float[] hitboxPulses = new float[] { 0.5f };
    private float playTime = 0f;
    private int currentPulse = 0;
    private float baseAnimTimer = 0f;
    private Vector2 mouseDirection = Vector2.right;
    private Vector2 modifiedMouseDirection = Vector2.right;
    private Vector3 nonRotatedExtents = Vector3.one;
    private ActiveItem source;
    private float damage;
    private float duration;
    private CombinedStatBlock stats;
    private ContactFilter2D hitFilter;
    private List<Collider2D> hitBuffer;

    private void Start()
    {
        if (meleeWeaponParent == null && this.transform.childCount > 0)
        {
            meleeWeaponParent = this.transform.GetChild(0).gameObject;
        }
        meleeWeaponParent.SetActive(false);
    }

    public override void Activate()
    {
        meleeWeaponParent.gameObject.SetActive(true);
        isActive = true;
        playTime = 0f;
        AimWeaponAtMouse();
    }

    public override void Setup(ActiveItem source)
    {
        this.source = source;
        if (meleeWeaponParent == null && this.transform.childCount > 0)
        {
            meleeWeaponParent = this.transform.GetChild(0).gameObject;
        }
        if (meleeWeaponVisual == null)
        {
            meleeWeaponVisual = GetComponentInChildren<Animator>();
        }
        if (hitbox == null)
        {
            hitbox = GetComponentInChildren<Collider2D>();
        }

        if (hitbox is CircleCollider2D)
        {
            hitboxType = HitboxType.Circle;
        }
        else if (hitbox is BoxCollider2D)
        {
            hitboxType = HitboxType.Box;
            nonRotatedExtents = (hitbox as BoxCollider2D).bounds.extents;
        }

        hitFilter = new ContactFilter2D
        {
            layerMask = 1 << LayerMask.NameToLayer("Enemy"),
            useTriggers = false,
            useLayerMask = true
        };
        hitBuffer = new List<Collider2D>();

        meleeWeaponParent.SetActive(false);
    }

    public override void ApplyStatBlock(CombinedStatBlock stats)
    {
        this.stats = stats;
        duration = stats.GetCombinedStatValue<ActiveItemDuration>();
    }

    public void Update()
    {
        if (isActive)
        {
            if (aimable)
            {
                AimWeaponAtMouse();
            }

            playTime += Time.deltaTime;
            float normalizedTime = meleeWeaponVisual.GetCurrentAnimatorStateInfo(0).normalizedTime;

            if (currentPulse < hitboxPulses.Length && normalizedTime > baseAnimTimer + hitboxPulses[currentPulse])
            {
                Hit();
                currentPulse = (currentPulse + 1) % hitboxPulses.Length;
                if (currentPulse == 0)
                {
                    baseAnimTimer += 1f;
                }
            }

            if (!repeatable && normalizedTime >= 1f)
            {
                Deactivate();
            }
            else if (repeatable && playTime >= meleeWeaponVisual.GetCurrentAnimatorStateInfo(0).length * modifiedRepeats)
            {
                Deactivate();
            }
        }
    }

    private void Hit()
    {
        int hits = Physics2D.OverlapCollider(hitbox, hitFilter, hitBuffer);
        if (hits > 0)
        {
            GameContext gameContext = World.activeWorld.worldStaticContext;
            gameContext.damageContext = new DamageContext
            {
                damageType = source.damageType,
                source = source,
                hitEnemies = new HashSet<Enemy>(),
                hitBoss = null
            };

            foreach (Collider2D hit in hitBuffer)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    gameContext.damageContext.hitEnemies.Add(enemy);
                    continue;
                }

                BossSeed boss = hit.GetComponent<BossSeed>();
                if (boss != null)
                {
                    gameContext.damageContext.hitBoss = boss;
                }
            }

            float damage = stats.GetCombinedStatValue<ActiveItemDamage>(gameContext);

            foreach (Enemy enemy in gameContext.damageContext.hitEnemies)
            {
                enemy.TakeDamage(damage);
            }

            if (gameContext.damageContext.hitBoss != null)
            {
                gameContext.damageContext.hitBoss.TakeDamage(damage);
            }
        }
    }

    private void Deactivate()
    {
        meleeWeaponParent.gameObject.SetActive(false);
        isActive = false;
        playTime = 0f;
        currentPulse = 0;
        baseAnimTimer = 0f;
    }

    private void AimWeaponAtMouse()
    {
        mouseDirection = Player.activePlayer.lookDirection;
        modifiedMouseDirection = mouseDirection;
        if (mouseDirection.x < 0)
        {
            modifiedMouseDirection.x = Mathf.Abs(mouseDirection.x);
            modifiedMouseDirection.y = mouseDirection.y * -1;
            meleeWeaponParent.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            meleeWeaponParent.transform.localScale = Vector3.one;
        }
        meleeWeaponParent.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(modifiedMouseDirection.x, modifiedMouseDirection.y, 0f)).eulerAngles;
    }

    public override StatBlockContext GetStatBlockContext(StatBlockContext baseContext, ActiveItem source)
    {
        StatBlockContext statBlockContext = baseContext;
        var attackString = (hitboxPulses.Length > 1) ? $"{StatBlockContext.GoodColor}{hitboxPulses.Length}</color> times " : "";
        statBlockContext.AddGenericTooltip($"Attacks {attackString}with a {StatBlockContext.GoodColor}{source.DisplayName}</color>." + 
            ((source.MaxCharges > 1) ? $" Charges: {source.MaxCharges}." : ""));
        return statBlockContext;
    }
}
