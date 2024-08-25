using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitSpawnExplosion", menuName = "NemesisShock/Events/OnHit/SpawnExplosion")]
public class OnHitSpawnExplosion : OnHitAction
{
    public override string GetTooltip(CombinedStatBlock stats, int level = 1)
    {
        return $"15% chance to spawn an explosion which deals {stats.GetCombinedStatValue<Damage>(World.activeWorld.worldStaticContext)} damage.";
    }

    public override void OnHit(Item source, Player player, Gun gun, ProjectileData projectile, Enemy enemy)
    {
        throw new System.NotImplementedException();
    }
}
