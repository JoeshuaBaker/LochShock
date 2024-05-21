using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;
using UnityEngine.UI;
using TMPro;

public class Gun : MonoBehaviour
{
    public StatBlock stats;
    public StatBlock combinedStats;
    public GunEmitter emitter;
    public Image reloadIndicator;
    public TextMeshProUGUI ammoIndicator;
    public TextMeshProUGUI ammoIndicatorShadow;

    private bool reloading;
    private float reloadSpeed;
    private float reloadTimer;
    private float fireSpeed;
    private float bulletSpread; //not implemented
    private float bulletCooldown;
    private int maxMagazine;
    private int magazine;

    public void Shoot()
    {
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)) && !reloading && magazine > 0 && bulletCooldown == 0)
        {
            //todo add bullet spread
            emitter.FireProjectile(emitter.Direction, 0f);

            //Audio Section
            AkSoundEngine.PostEvent("PlayShoot", this.gameObject);


            magazine -= 1;
            bulletCooldown = fireSpeed;
            if(magazine == 0)
            {
                reloadTimer = reloadSpeed;
                reloading = true;
            }
        }
        else
        {
            emitter.AutoFire = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ApplyStatBlock(stats);
        combinedStats = new StatBlock(StatBlock.BlockType.Additive);
        emitter.ApplyStatBlock(stats);
        magazine = maxMagazine;

        if(emitter == null)
        {
            emitter = GetComponent<GunEmitter>();
        }
    }

    public void ApplyStatBlock(StatBlock stats)
    {
        combinedStats = stats;
        reloadSpeed = stats.gunStats.reloadSpeed;
        fireSpeed = stats.gunStats.fireSpeed;
        maxMagazine = (int) stats.gunStats.magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        emitter.ApplyStatBlock(combinedStats);
        reloadTimer = Mathf.Max(reloadTimer - Time.deltaTime, 0);
        bulletCooldown = Mathf.Max(bulletCooldown - Time.deltaTime, 0);

        if(reloading && reloadTimer == 0)
        {
            reloading = false;
            magazine = maxMagazine;
            bulletCooldown = 0;
        }

        string displayAmmo = magazine.ToString();
        ammoIndicator.text = displayAmmo;
        ammoIndicatorShadow.text = displayAmmo;
        reloadIndicator.gameObject.SetActive(reloading);

        if (reloading)
        {
            reloadIndicator.fillAmount = reloadTimer / reloadSpeed;
        }
    }
}
