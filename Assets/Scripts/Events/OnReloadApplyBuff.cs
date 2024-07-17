using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnReloadApplyBuff", menuName = "NemesisShock/Events/OnReload/ApplyBuff")]
public class OnReloadApplyBuff : OnReloadAction
{
    public Buff buff;
    public override string GetTooltip(StatBlock stats)
    {
        return GetBuffTooltip(this, buff);
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
