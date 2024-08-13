using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponActiveItem : ActiveItem
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
    public int numRepeats = 1;
    public float[] hitboxPulses = new float[] { 0.5f };
    private float playTime = 0f;
    private int currentPulse = 0;
    private float baseAnimTimer = 0f;
    private Vector2 mouseDirection = Vector2.right;
    private Vector2 modifiedMouseDirection = Vector2.right;
    private Vector3 nonRotatedExtents = Vector3.one;
    public override void Activate()
    {
        if (cooldownTimer <= 0f)
        {
            meleeWeaponParent.gameObject.SetActive(true);
            isActive = true;
            playTime = 0f;
            cooldownTimer = cooldown;
            mouseDirection = Player.activePlayer.mouseDirection;
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
    }

    public override void Setup()
    {
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

        meleeWeaponParent.SetActive(false);
    }

    public override void Update()
    {
        base.Update();
        if (isActive)
        {
            playTime += Time.deltaTime;
            float normalizedTime = meleeWeaponVisual.GetCurrentAnimatorStateInfo(0).normalizedTime;

            if (currentPulse < hitboxPulses.Length && normalizedTime > baseAnimTimer + hitboxPulses[currentPulse])
            {
                currentPulse = (currentPulse + 1) % hitboxPulses.Length;
                if (currentPulse == 0)
                {
                    baseAnimTimer += 1f;
                }

                switch (hitboxType)
                {
                    case HitboxType.Circle:
                        float radius = (hitbox as CircleCollider2D).radius;
                        Vector3 circleScale = new Vector3(radius, radius, 1f);
                        World.activeWorld.explosionSpawner.CreateDangerZone(9999, 0f, hitbox.transform.position, dealsDamage: true, safeOnPlayer: true, noPS: true, circleScale, false, Quaternion.identity, 3);
                        break;

                    case HitboxType.Box:
                        Vector3 dangerZoneScale = nonRotatedExtents;
                        Vector3 dangerZoneEulers = Quaternion.FromToRotation(Vector3.right, new Vector3(mouseDirection.x, mouseDirection.y, 0f)).eulerAngles;
                        World.activeWorld.explosionSpawner.CreateDangerZone(9999, 0f, hitbox.transform.position, true, true, true, dangerZoneScale, true, Quaternion.Euler(dangerZoneEulers), 3);
                        break;
                }
            }

            if (!repeatable && normalizedTime >= 1f)
            {
                Deactivate();
            }
            else if (repeatable && playTime >= meleeWeaponVisual.GetCurrentAnimatorStateInfo(0).length * numRepeats)
            {
                Deactivate();
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

    public override StatBlockContext GetStatBlockContext()
    {
        StatBlockContext statBlockContext = new StatBlockContext();
        var attackString = (hitboxPulses.Length > 1) ? $"{StatBlockContext.GoodColor}{hitboxPulses.Length}</color> times " : "";
        statBlockContext.AddGenericTooltip($"Attacks {attackString}with a {StatBlockContext.GoodColor}{name}</color>. Cooldown: {StatBlockContext.HighlightColor}{cooldown}</color>s.");
        return statBlockContext;
    }
}
