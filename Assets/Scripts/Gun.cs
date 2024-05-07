using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public StatBlock statBlock;
    public GunEmitter emitter;
    public Image reloadIndicator;
    public Image ammoIndicator;

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
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0) && !reloading && magazine > 0 && bulletCooldown == 0)
        {
            //todo add bullet spread
            emitter.FireProjectile(emitter.Direction, 0f);
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
        ApplyStatBlock(statBlock);
        emitter.ApplyStatBlock(statBlock);
        magazine = maxMagazine;
    }

    void ApplyStatBlock(StatBlock stats)
    {
        reloadSpeed = stats.gunStats.reloadSpeed;
        fireSpeed = stats.gunStats.fireSpeed;
        maxMagazine = (int) stats.gunStats.magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyStatBlock(statBlock);
        emitter.ApplyStatBlock(statBlock);
        reloadTimer = Mathf.Max(reloadTimer - Time.deltaTime, 0);
        bulletCooldown = Mathf.Max(bulletCooldown - Time.deltaTime, 0);

        if(reloading && reloadTimer == 0)
        {
            reloading = false;
            magazine = maxMagazine;
            bulletCooldown = 0;
        }

        ammoIndicator.fillAmount = (float)magazine / (float)maxMagazine;
        reloadIndicator.gameObject.SetActive(reloading);

        if (reloading)
        {
            reloadIndicator.fillAmount = reloadTimer / reloadSpeed;
        }
    }
}
