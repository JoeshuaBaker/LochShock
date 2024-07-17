using UnityEngine;
using BulletHell;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

//Extend Item
public class Gun : Item
{
    public StatBlock newCombinedStats;
    public GunEmitter emitter;
    public ParticleSystem muzzleFlashMain;
    public ParticleSystem muzzleFlashFar;
    public ParticleSystem muzzleFlashClose;
    public ParticleSystem ejectedCasing;
    public Light2D visionCone;
    public Animator lightAnimator;
    public AK.Wwise.Event gunAudioEvent;

    private float reloadSpeed;
    private float reloadTimer;
    private float fireSpeed;
    private float bulletCooldown;
    public int maxMagazine;
    public int magazine;
    private bool reloadAudio = false;

    public void Shoot()
    {
        if (Input.GetKey(KeyCode.Mouse0) && magazine > 0 && bulletCooldown == 0)
        {
            if(emitter == null)
            {
                emitter = GetComponent<GunEmitter>();
            }
            emitter.FireProjectile(emitter.Direction, 0f);

            //Particle Systems
            muzzleFlashMain.Emit(15);
            muzzleFlashFar.Emit(5);
            muzzleFlashClose.Emit(25);
            ejectedCasing.Emit(1);

            //light
            lightAnimator.Play("Base Layer.MuzzleFlashLight" , 0 , 0f);

            //Audio Section
            //AkSoundEngine.PostEvent("Play" + this.name.Replace(" ", string.Empty), this.gameObject);
            gunAudioEvent.Post(this.gameObject);

            foreach(OnFireAction onFire in newCombinedStats.events.OnFire)
            {
                onFire.OnFire(Player.activePlayer, this);
            }

            magazine -= 1;
            bulletCooldown = fireSpeed;
            reloadTimer = 0;
            reloadAudio = true;

            //Audio Section
            if (magazine == 0)
            {                
                AkSoundEngine.PostEvent("PlayOutOfAmmo", this.gameObject);
            }            
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

        if(stats.stats.Count > 0)
        {
            ApplyNewStatBlock(stats);
        }
        newCombinedStats = new StatBlock();
        magazine = maxMagazine;

        if(emitter == null)
        {
            emitter = GetComponent<GunEmitter>();
        }

        emitter.gun = this;
    }

    public void ApplyNewStatBlock(StatBlock stats)
    {
        newCombinedStats = stats;
        reloadSpeed = stats.GetStatValue<ReloadSpeed>();
        fireSpeed = stats.GetStatValue<FireSpeed>();
        maxMagazine = (int) stats.GetStatValue<MagazineSize>();
    }

    // Update is called once per frame
    private void Update()
    {
        bulletCooldown = Mathf.Max(bulletCooldown - Time.deltaTime, 0);
    }

    public void UpdateActiveGun()
    {
        emitter.ApplyStatBlock(newCombinedStats);

        if(magazine < maxMagazine)
        {
            reloadTimer = Mathf.Min(reloadTimer + Time.deltaTime, reloadSpeed);
        }

        if(reloadTimer >= reloadSpeed)
        {
            magazine = maxMagazine;
            reloadTimer = 0;

            //Audio Section
            if (reloadAudio)
            {
                AkSoundEngine.PostEvent("PlayReload", this.gameObject); reloadAudio = false;
            }

            foreach (OnReloadAction onReload in newCombinedStats.events.OnReload)
            {
                onReload.OnReload(Player.activePlayer, this);
            }
        }

        Crosshair.activeCrosshair.UpdateCrosshair(!Input.GetKey(KeyCode.Mouse0) || magazine == 0, reloadTimer / reloadSpeed, magazine.ToString());
    }
}
