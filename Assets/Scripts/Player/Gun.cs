using System.Collections.Generic;
using UnityEngine;
using BulletHell;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

//Extend Item
public class Gun : Item
{
    public enum GunType
    {
        Emitter,
        GameObject
    }

    public GunType gunType = GunType.Emitter;
    public ReloadType reloadType = ReloadType.Magazine;
    public bool waitForFireSpeedBeforeReload = false;
    public GunEmitter emitter;
    public Projectile projectile;
    public ParticleSystem muzzleFlashMain;
    public ParticleSystem muzzleFlashFar;
    public ParticleSystem muzzleFlashClose;
    public ParticleSystem ejectedCasing;
    public Light2D visionCone;
    public Light2D beamLight;
    public Animator lightAnimator;
    public AK.Wwise.Event gunAudioEvent;

    private ProjectilePool projectilePool;
    public bool shooting = false;
    public bool setup = false;
    private float reloadSpeed;
    private float percentReloaded;
    public float fireSpeed;
    public float bulletCooldown;
    public float firePositionOffset = 0.25f;
    protected Vector2 direction;
    protected Vector2 lookPosition;
    public int maxMagazine;
    public int magazine;
    public float percentMagFilled;
    private bool reloadAudio = false;

    public void Shoot()
    {
        if (IsReady())
        {
            FireBullets(direction, lookPosition);

            bulletCooldown = fireSpeed;
            if (reloadType != ReloadType.Charge)
            {
                CancelReload();
                magazine -= 1;
                percentMagFilled = magazine / (float)maxMagazine;
            }
            else
            {
                float reloadFraction = 1f / maxMagazine;
                percentReloaded -= reloadFraction;
                magazine = (int)(percentReloaded * maxMagazine);
                percentMagFilled = magazine / (float)maxMagazine;
            }
            reloadAudio = true;

            //Audio Section
            if (magazine == 0 && reloadType == ReloadType.Magazine)
            {                
                AkSoundEngine.PostEvent("PlayOutOfAmmo", this.gameObject);
            }
        }
    }

    public void ShootIgnoreState(CombinedStatBlock externalStatBlock = null)
    {
        if (!setup && externalStatBlock == null)
        {
            return;
        }

        if(externalStatBlock != null)
        {
            ApplyNewStatBlock(externalStatBlock);
        }

        //get lookdirection here, since the gun wont be recieving updates as it's not the active weapon
        Vector2 mouseDirection = Player.activePlayer.lookDirection;
        Vector2 mousePosition = Player.activePlayer.lookPosition;
        transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(mouseDirection.x, mouseDirection.y, 0f)).eulerAngles;

        FireBullets(mouseDirection, mousePosition);
    }

    protected void FireBullets(Vector2 direction, Vector2 mousePosition)
    {
        if(gunType == GunType.Emitter)
        {
            emitter.FireProjectile(direction, 0f);
        }
        else if(gunType == GunType.GameObject)
        {
            GameContext context = World.activeWorld.worldStaticContext;
            int bulletStreams = (int) combinedStats.GetCombinedStatValue<BulletStreams>(context);
            int bulletsPerShot = (int)combinedStats.GetCombinedStatValue<BulletsPerShot>(context);
            float accuracy = combinedStats.GetCombinedStatValue<Accuracy>(context);
            float spreadAngle = combinedStats.GetCombinedStatValue<SpreadAngle>(context);

            if(bulletStreams == 1 && bulletsPerShot == 1)
            {
                Projectile projectile = projectilePool.GetProjectile();
                Vector2 bulletDirection = ProjectileEmitterAdvanced.GetDirectionByStreamsAndShots(direction, 0, bulletStreams, 0, bulletsPerShot, accuracy, spreadAngle);
                projectile.SetupProjectile(this, combinedStats, transform.position + direction.xyz() * firePositionOffset, bulletDirection, mousePosition);
            }
            else
            {
                int i = 0;
                Projectile[] projectiles = projectilePool.GetProjectiles(bulletStreams, bulletsPerShot);
                for(int stream = 0; stream < bulletStreams; stream++)
                {
                    for(int bullet = 0; bullet < bulletsPerShot; bullet++)
                    {
                        Vector2 bulletDirection = ProjectileEmitterAdvanced.GetDirectionByStreamsAndShots(direction, stream, bulletStreams, bullet, bulletsPerShot, accuracy, spreadAngle);
                        projectiles[i++].SetupProjectile(this, combinedStats, transform.position + bulletDirection.xyz() * firePositionOffset, bulletDirection, mousePosition, stream, bullet);
                    }
                }
            }
        }

        //Particle Systems
        if(muzzleFlashMain != null)
        {
            muzzleFlashMain.Emit(15);
        }
        if(muzzleFlashFar != null)
        {
            muzzleFlashFar.Emit(5);
        }
        if(muzzleFlashClose != null)
        {
            muzzleFlashClose.Emit(25);
        }
        if(ejectedCasing != null)
        {
            ejectedCasing.Emit(1);
        }

        //light
        if(lightAnimator != null)
        {
            lightAnimator.Play("Base Layer.MuzzleFlashLight", 0, 0f);
        }

        if(gunAudioEvent != null)
        {
            //Audio Section
            //AkSoundEngine.PostEvent("Play" + this.name.Replace(" ", string.Empty), this.gameObject);
            gunAudioEvent.Post(this.gameObject);
        }
        

        foreach (OnFireAction onFire in combinedStats.combinedStatBlock.GetEvents<OnFireAction>())
        {
            Item source = Player.activePlayer.inventory.FindEventSource(onFire);
            onFire.OnFire(source, Player.activePlayer, this);
        }
    }

    public void UpdateVisionCone(float totalVis, float coneRadius, float coneAngle)
    {
        if(visionCone != null)
        {
            visionCone.pointLightInnerRadius = Mathf.Max(totalVis * coneRadius, 4f);
            visionCone.pointLightOuterRadius = visionCone.pointLightInnerRadius * 4.3f;
            visionCone.pointLightInnerAngle = Mathf.Max(totalVis * coneAngle, 10f);
            visionCone.pointLightOuterAngle = visionCone.pointLightInnerAngle * 4.3f;
            visionCone.intensity = Mathf.Min(totalVis * 3.3f, 1f);
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        setup = false;
        maxMagazine = (int) baseItemCombinedStats.GetCombinedStatValue<MagazineSize>(World.activeWorld.worldStaticContext);
        magazine = maxMagazine;
        percentMagFilled = 1f;
        percentReloaded = reloadType == ReloadType.Charge ? 1f : 0f;

        if(gunType == GunType.Emitter)
        {
            if(emitter == null)
            {
                Debug.LogWarning("Gun " + name + " is set to emitter type, but doesn't have an emitter reference. Please set it up in the inspector on the prefab. Trying GetComponent.");
                emitter = GetComponent<GunEmitter>();
                if(emitter == null)
                {
                    Debug.LogError("Gun " + name + " is set to emitter type, but doesn't have an emitter component attached. Game will probably crash when you try to fire :)");
                    return;
                }
            }
            emitter.gun = this;
        }
        else if(gunType == GunType.GameObject)
        {
            if(projectile == null)
            {
                Debug.LogError("Gun " + name + " is set to GameObject type, but doesn't have a projectile prefab. Please set it up in the inspector on the prefab. Game will probably crash when you try to fire :)");
                return;
            }
        }
    }

    public void ApplyNewStatBlock(CombinedStatBlock stats)
    {
        if (!setup)
        {
            setup = true;
            magazine = maxMagazine;
            percentReloaded = reloadType == ReloadType.Charge ? 1f : 0f;
            percentMagFilled = 1f;
        }

        combinedStats = stats;
        reloadSpeed = stats.GetCombinedStatValue<ReloadSpeed>(World.activeWorld.worldStaticContext);
        fireSpeed = 1f / stats.GetCombinedStatValue<FireSpeed>(World.activeWorld.worldStaticContext);
        int newMagSize = (int)stats.GetCombinedStatValue<MagazineSize>(World.activeWorld.worldStaticContext);
        if(newMagSize != maxMagazine)
        {
            maxMagazine = newMagSize;
            magazine = (int)(percentMagFilled * maxMagazine);
        }


        if (gunType == GunType.Emitter)
        {
            emitter.ApplyStatBlock(combinedStats);
        }
        else if(gunType == GunType.GameObject)
        {
            if(projectilePool == null)
            {
                projectilePool = new ProjectilePool(projectile, stats);
            }
            else
            {
                projectilePool.UpdateProjectilePool(stats);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        bulletCooldown = Mathf.Max(bulletCooldown - Time.deltaTime, 0);
        if(reloadType == ReloadType.Charge)
        {
            IncrementReloadPercentage();
            magazine = (int)(percentReloaded * maxMagazine);
            percentMagFilled = magazine / (float)maxMagazine;
        }
    }

    protected bool IsReady()
    {
        if(gunType == GunType.Emitter && emitter == null)
        {
            return false;
        }
        else if(gunType == GunType.GameObject && (projectile == null || projectilePool == null) )
        {
            return false;
        }
        return setup && shooting && magazine > 0 && bulletCooldown == 0;
    }

    public virtual void UpdateActiveGun(Vector2 lookDirection, Vector2 lookPosition)
    {
        if (!setup)
            return;

        direction = lookDirection;
        this.lookPosition = lookPosition;
        transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(lookDirection.x, lookDirection.y, 0f)).eulerAngles;

        float lastReloadPercentage = percentReloaded;

        if (reloadType != ReloadType.Charge && magazine < maxMagazine)
        {
            IncrementReloadPercentage();
        }

        switch (reloadType)
        {
            case ReloadType.Magazine:
                if (percentReloaded >= 1f)
                {
                    magazine = maxMagazine;
                    percentReloaded = 0;
                    percentMagFilled = magazine / (float)maxMagazine;

                    //Audio Section
                    if (reloadAudio)
                    {
                        AkSoundEngine.PostEvent("PlayReload", this.gameObject); reloadAudio = false;
                    }
                }

                Crosshair.activeCrosshair.UpdateCrosshair(lookPosition, !shooting || magazine == 0, percentReloaded, magazine.ToString());
                break;

            case ReloadType.Charge:
                Crosshair.activeCrosshair.UpdateCrosshair(lookPosition, true, percentReloaded, magazine.ToString());
                break;

            case ReloadType.Incremental:
                if (percentReloaded >= 1f)
                {
                    percentReloaded = percentReloaded % 1f;
                    magazine = Mathf.Min(magazine + 1, maxMagazine);
                    percentMagFilled = magazine / (float)maxMagazine;
                }
                Crosshair.activeCrosshair.UpdateCrosshair(lookPosition, !shooting || magazine == 0, percentReloaded, magazine.ToString());
                break;
        }

        if(lastReloadPercentage < 1f && percentReloaded >= 1f)
        {
            foreach (OnReloadAction onReload in combinedStats.combinedStatBlock.GetEvents<OnReloadAction>())
            {
                Item source = Player.activePlayer.inventory.FindEventSource(onReload);
                onReload.OnReload(source, Player.activePlayer, this);
            }
        }
    }

    public void CancelReload()
    {
        if(reloadType != ReloadType.Charge)
        {
            percentReloaded = 0;
        }
    }

    private void IncrementReloadPercentage()
    {
        if(waitForFireSpeedBeforeReload && bulletCooldown > 0)
        {
            return;
        }
        percentReloaded += Time.deltaTime / reloadSpeed;

        if(reloadType != ReloadType.Incremental)
        {
            percentReloaded = Mathf.Min(percentReloaded, 1f);
        }
    }

    public override StatBlockContext GetStatBlockContext()
    {
        StatBlockContext baseContext = base.GetStatBlockContext();
        bool hasNonBaseStats = false;
        foreach(StatBlock statBlock in newStatsList)
        {
            foreach(Stat stat in statBlock.stats)
            {
                if(!(stat.combineType is BaseStat) && stat.stacks > 0)
                {
                    hasNonBaseStats = true;
                    break;
                }
            }

            if (hasNonBaseStats)
                break;
        }
        
        if(hasNonBaseStats)
        {
            baseContext.AddGenericBaseStatSeperatorTooltip("While Equipped:".AddColorToString(StatBlockContext.HighlightColor));
        }
        return baseContext;
    }

    protected void OnDestroy()
    {
        if(gunType == GunType.GameObject)
        {
            projectilePool.Cleanup();
        }
    }
}
