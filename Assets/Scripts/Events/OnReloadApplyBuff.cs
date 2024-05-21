using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnReloadApplyBuff", menuName = "NemesisShock/Events/OnReload/ApplyBuff")]
public class OnReloadApplyBuff : OnReloadAction
{
    public Buff buff;
    public override string GetTooltip(StatBlock stats)
    {
        return $"{(int)(chanceToTrigger * 100f)}% chance to gain {(int)(buff.stats.gunStats.fireSpeed * 100f)}% fire speed for {buff.duration} seconds. " +
            $"(New Fire Rate: {stats.gunStats.fireSpeed * (1f + buff.stats.gunStats.fireSpeed)}).";
    }

    public override void OnReload(Player player, Gun gun)
    {
        if (IsValidTrigger())
        {
            base.OnReload(player, gun);
            player.AddBuff(buff.GetInstance());
        }
    }
}
