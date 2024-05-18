using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitApplyBuff", menuName = "NemesisShock/Events/OnHit/ApplyBuff")]
public class OnHitApplyBuff : OnHitAction
{
    public Buff buff;

    public override string GetTooltip(StatBlock stats)
    {
        return $"{(int)(chanceToTrigger*100f)}% chance to gain {(int)(buff.stats.gunStats.fireSpeed*100f)}% fire speed for {buff.duration} seconds. " +
            $"(New Fire Rate: {stats.gunStats.fireSpeed * (1f + buff.stats.gunStats.fireSpeed)}).";
    }

    public override void OnHit(Player player, GunEmitter gun, ProjectileData projectile, Enemy enemy)
    {
        if(IsValidTrigger())
        {
            player.AddBuff(buff.GetInstance());
        }
    }
}
