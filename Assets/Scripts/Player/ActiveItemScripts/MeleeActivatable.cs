using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeActivatable : Activatable
{
    public GameObject meleeWeaponParent;
    public Animator meleeWeaponVisual;
    public Collider2D hitbox;
    public bool isActive = false;
    public bool repeatable = false;
    public bool aimable = true;
    public float bonusRepeatsDurationThreshold = 5f;
    public int baseExtraRepeats = 0;
    public float hitEffectAngleOffset;
    private int modifiedRepeats
    {
        get {return baseExtraRepeats + (int)(repeatable ? Mathf.Max((duration - 1f + bonusRepeatsDurationThreshold + 0.01f) / bonusRepeatsDurationThreshold, 1f) : 1); }
    }
    public float[] hitboxPulses = new float[] { 0.5f };
    private float playTime = 0f;
    private int currentPulse = 0;
    private float baseAnimTimer = 0f;
    private Vector2 mouseDirection = Vector2.right;
    private Vector2 modifiedMouseDirection = Vector2.right;
    private ActiveItem source;
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

    public override bool Activate()
    {
        if(!meleeWeaponParent.gameObject.activeSelf)
        {
            meleeWeaponParent.gameObject.SetActive(true);
            isActive = true;
            playTime = 0f;
            AimWeaponAtMouse();
            return true;
        }

        return false;
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

        hitFilter = new ContactFilter2D
        {
            layerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Doodads"),
            useTriggers = true,
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
                    World.activeWorld.hitEffect.EmitZoneHit(enemy.transform.position, (enemy.transform.position - this.transform.position), true, (hitEffectAngleOffset * meleeWeaponParent.transform.localScale.x));
                    continue;
                }

                AdvancedDoodad doodad = hit.GetComponentInParent<AdvancedDoodad>();
                if (doodad != null)
                {
                    doodad.Destruct();
                    World.activeWorld.hitEffect.EmitTreeHit(doodad.transform.position, (doodad.transform.position - this.transform.position), (hitEffectAngleOffset * meleeWeaponParent.transform.localScale.x));
                    continue;
                }

                BossSeed boss = hit.GetComponent<BossSeed>();
                if (boss != null)
                {
                    gameContext.damageContext.hitBoss = boss;
                    World.activeWorld.hitEffect.EmitZoneHit(boss.transform.position, (boss.transform.position - this.transform.position), true, (hitEffectAngleOffset * meleeWeaponParent.transform.localScale.x));
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

    public override StatBlockContext GetStatBlockContext(CombinedStatBlock baseContext, ActiveItem source)
    {
        StatBlockContext statBlockContext = baseContext.GetCombinedContext();

        if(stats != null)
        {
            duration = stats.GetCombinedStatValue<ActiveItemDuration>();
        }
        else
        {
            duration = 0;
        }

        var numAttacks = hitboxPulses.Length * modifiedRepeats;
        var attackString = (numAttacks > 1) ? $"{numAttacks.ToString().AddColorToString(StatBlockContext.GoodColor)} times " : "";
        statBlockContext.AddGenericPrefixTooltip($"Attacks {attackString}with a {source.DisplayName.AddColorToString(StatBlockContext.GoodColor)}.");

        if (repeatable)
        {
            string extraAttackString = (hitboxPulses.Length > 1) ? $"{hitboxPulses.Length} attacks" : "attack";
            statBlockContext.AddGenericPostfixTooltip($"{System.Environment.NewLine}Gains an extra {extraAttackString.AddColorToString(StatBlockContext.GoodColor)} for every {(bonusRepeatsDurationThreshold * 100f).ToString().AddColorToString(StatBlockContext.HighlightColor)}% active item duration.");
        }

        return statBlockContext;
    }
}
