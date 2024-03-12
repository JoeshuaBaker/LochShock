using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

public class Enemy : BulletCollidable
{
    public SpriteRenderer health;
    public int maxHp = 10;
    public int currentHp = 10;

    public override void ProcessCollision(ProjectileData projectile)
    {
        currentHp = Mathf.Max(currentHp - projectile.damage, 0);
        float hpRatio = ((float)currentHp / (float)maxHp);
        health.color = new Color(1.0f, hpRatio, hpRatio);
        if(currentHp == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
