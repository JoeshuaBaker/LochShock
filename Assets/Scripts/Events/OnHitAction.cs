using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public abstract class OnHitAction : ScriptableAction
{
    public virtual void OnHit(Item source, Player player, Gun gun, ProjectileData projectile, Enemy enemy)
    {
        PlayTriggerSfx(player.gameObject);
    }

    public override string GetLabel()
    {
        return "On Hit";
    }
}
