using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public class EnemyGun : Gun
{
    [SerializeField] private float powerRating = 0;
    public float PowerRating => (baseItemCombinedStats != null && powerRating == 0f) ? PowerRatingFormula() : powerRating;
    public Vector3 screenPositionOffset = Vector3.zero;
    private float doneFiringTimer = 0;
    private bool doneFiring = false;
    public bool ready = false;
    public override void Start()
    {
        base.Start();
        (emitter as EnemyGunEmitter).Awake();
        (emitter as EnemyGunEmitter).Start();
        if(powerRating == 0)
        {
            powerRating = PowerRatingFormula();
        }
        ready = true;
    }

    public override void Update()
    {
        if (doneFiring || ready)
        {
            doneFiringTimer -= Time.deltaTime;
            if(doneFiringTimer <= 0f)
            {
                doneFiring = false;
                ready = true;
            }
            return;
        }

        base.Update(); 
        lookPosition = Player.activePlayer.transform.position.xy();
        direction = (lookPosition - this.transform.position.xy()).normalized;
        ApplyNewStatBlock(baseItemCombinedStats);
        UpdateActiveGun(direction, lookPosition);
        Shoot();

        if(magazine == 0)
        {
            doneFiringTimer = stats.GetStatValue<Lifetime>();
            doneFiring = true;
            shooting = false;
        }
    }

    private float PowerRatingFormula()
    {
        if(baseItemCombinedStats != null)
        {
            return
            10f * baseItemCombinedStats.GetCombinedStatValue<Velocity>() +
            15f * baseItemCombinedStats.GetCombinedStatValue<BulletsPerShot>() +
            10f * baseItemCombinedStats.GetCombinedStatValue<FireSpeed>() +
            5f * baseItemCombinedStats.GetCombinedStatValue<MagazineSize>();
        }
        else
        {
            return powerRating;
        }
    }

    public void Setup(Vector3 position)
    {
        transform.position = position;
        screenPositionOffset = transform.position - Player.activePlayer.transform.position;
        magazine = maxMagazine;
        doneFiringTimer = 0f;
        shooting = true;
        doneFiring = false;
        ready = false;
        this.gameObject.SetActive(true);
    }

    public override void UpdateActiveGun(Vector2 lookDirection, Vector2 lookPosition)
    {
        direction = lookDirection;
        this.lookPosition = lookPosition;
        transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(lookDirection.x, lookDirection.y, 0f)).eulerAngles;
    }
}
