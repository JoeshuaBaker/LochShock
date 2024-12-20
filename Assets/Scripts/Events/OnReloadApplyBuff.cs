using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnReloadApplyBuff", menuName = "NemesisShock/Events/OnReload/ApplyBuff")]
public class OnReloadApplyBuff : OnReloadAction
{
    public Buff buff;
    public override string GetTooltip(CombinedStatBlock stats, int level = 1)
    {
        return GetBuffTooltip(this, buff, level);
    }

    public override void OnReload(Item source, Player player, Gun gun)
    {
        if (IsValidTrigger())
        {
            base.OnReload(source, player, gun);
            source.AddBuff(buff.GetInstance(source));
        }
    }
}
