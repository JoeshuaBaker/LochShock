using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public abstract class OnHitAction : ScriptableAction
{
    public abstract void OnHit(Player player, GunEmitter gun, ProjectileData projectile, Enemy enemy);

    public override string GetLabel()
    {
        return "On Hit";
    }
}
