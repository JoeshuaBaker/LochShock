using UnityEngine;
using BulletHell;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

//Extend Item
public class Gun : Item
{
    public DamageType damageType = DamageType.Kinetic;
    public ReloadType reloadType = ReloadType.Magazine;
    public GunEmitter emitter;
    public ParticleSystem muzzleFlashMain;
    public ParticleSystem muzzleFlashFar;
    public ParticleSystem muzzleFlashClose;
    public ParticleSystem ejectedCasing;
    public Light2D visionCone;
    public Light2D beamLight;
    public Animator lightAnimator;
    public AK.Wwise.Event gunAudioEvent;

    public bool shooting = false;
    public bool setup = false;
    private float reloadSpeed;
    private float reloadTimer;
    public float fireSpeed;
    public float bulletCooldown;
    public int maxMagazine;
    public int magazine;
    private bool reloadAudio = false;

    public void Shoot()
    {
        if (setup && shooting && magazine > 0 && bulletCooldown == 0)
        {
            if(emitter == null)
            {
                emitter = GetComponent<GunEmitter>();
            }

            FireBullets(emitter.Direction);

            bulletCooldown = fireSpeed;
            if (reloadType != ReloadType.Charge)
            {
                reloadTimer = 0;
                magazine -= 1;
            }
            else
            {
                float reloadFraction = reloadSpeed / maxMagazine;
                reloadTimer -= reloadFraction;
                magazine = (int)((reloadTimer / reloadSpeed) * maxMagazine);
            }
            reloadAudio = true;

            //Audio Section
            if (magazine == 0)
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

        if (emitter == null)
        {
            emitter = GetComponent<GunEmitter>();
        }

        if(externalStatBlock != null)
        {
            ApplyNewStatBlock(externalStatBlock);
        }

        Vector2 mouseDirection = Player.activePlayer.mouseDirection;
        transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(mouseDirection.x, mouseDirection.y, 0f)).eulerAngles;

        FireBullets(mouseDirection);
    }

    private void FireBullets(Vector2 direction)
    {
        emitter.FireProjectile(direction, 0f);

        //Particle Systems
        muzzleFlashMain.Emit(15);
        muzzleFlashFar.Emit(5);
        muzzleFlashClose.Emit(25);
        ejectedCasing.Emit(1);

        //light
        lightAnimator.Play("Base Layer.MuzzleFlashLight", 0, 0f);

        //Audio Section
        //AkSoundEngine.PostEvent("Play" + this.name.Replace(" ", string.Empty), this.gameObject);
        gunAudioEvent.Post(this.gameObject);

        foreach (OnFireAction onFire in combinedStats.combinedStatBlock.GetEvents<OnFireAction>())
        {
            onFire.OnFire(Player.activePlayer, this);
        }
    }

    public void UpdateVisionCone(float totalVis, float coneRadius, float coneAngle)
    {
        visionCone.pointLightInnerRadius = Mathf.Max(totalVis * coneRadius, 4f);
        visionCone.pointLightOuterRadius = visionCone.pointLightInnerRadius * 4.3f;
        visionCone.pointLightInnerAngle = Mathf.Max(totalVis * coneAngle, 10f);
        visionCone.pointLightOuterAngle = visionCone.pointLightInnerAngle * 4.3f;
        visionCone.intensity = Mathf.Min(totalVis * 3.3f, 1f);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        setup = false;
        magazine = maxMagazine;

        if(emitter == null)
        {
            emitter = GetComponent<GunEmitter>();
        }

        emitter.gun = this;
    }

    public void ApplyNewStatBlock(CombinedStatBlock stats, bool resetAmmo = false)
    {
        setup = true;
        combinedStats = stats;
        reloadSpeed = stats.GetCombinedStatValue<ReloadSpeed>(World.activeWorld.worldStaticContext);
        fireSpeed = stats.GetCombinedStatValue<FireSpeed>(World.activeWorld.worldStaticContext);
        maxMagazine = (int)stats.GetCombinedStatValue<MagazineSize>(World.activeWorld.worldStaticContext);
        if(resetAmmo)
        {
            magazine = maxMagazine;
            reloadTimer = 0;
        }
        emitter.ApplyStatBlock(combinedStats);
    }

    // Update is called once per frame
    private void Update()
    {
        bulletCooldown = Mathf.Max(bulletCooldown - Time.deltaTime, 0);
    }

    public void UpdateActiveGun()
    {
        if (!setup)
            return;

        emitter.ApplyStatBlock(combinedStats);

        if (magazine < maxMagazine)
        {
            reloadTimer = Mathf.Min(reloadTimer + Time.deltaTime, reloadSpeed);
        }

        switch (reloadType)
        {
            case ReloadType.Magazine:
                if (reloadTimer >= reloadSpeed)
                {
                    magazine = maxMagazine;
                    reloadTimer = 0;

                    //Audio Section
                    if (reloadAudio)
                    {
                        AkSoundEngine.PostEvent("PlayReload", this.gameObject); reloadAudio = false;
                    }

                    foreach (OnReloadAction onReload in combinedStats.combinedStatBlock.GetEvents<OnReloadAction>())
                    {
                        onReload.OnReload(Player.activePlayer, this);
                    }
                }

                Crosshair.activeCrosshair.UpdateCrosshair(!Input.GetKey(KeyCode.Mouse0) || magazine == 0, reloadTimer / reloadSpeed, magazine.ToString());
                break;

            case ReloadType.Charge:
                magazine = (int)((reloadTimer / reloadSpeed) * maxMagazine);
                Crosshair.activeCrosshair.UpdateCrosshair(true, reloadTimer / reloadSpeed, magazine.ToString());
                break;

            case ReloadType.Incremental:
                if (reloadTimer >= reloadSpeed)
                {
                    reloadTimer -= reloadSpeed;
                    magazine = Mathf.Min(magazine + 1, maxMagazine);
                }
                Crosshair.activeCrosshair.UpdateCrosshair(!Input.GetKey(KeyCode.Mouse0) || magazine == 0, reloadTimer / reloadSpeed, magazine.ToString());
                break;
        }
    }
}
