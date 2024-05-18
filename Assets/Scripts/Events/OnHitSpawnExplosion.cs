using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitSpawnExplosion", menuName = "NemesisShock/Events/OnHit/SpawnExplosion")]
public class OnHitSpawnExplosion : OnHitAction
{
    public override string GetTooltip(StatBlock stats)
    {
        return $"15% chance to spawn an explosion which deals {stats.gunStats.damage} damage.";
    }

    public override void OnHit(Player player, GunEmitter gun, ProjectileData projectile, Enemy enemy)
    {
        throw new System.NotImplementedException();
    }
}
