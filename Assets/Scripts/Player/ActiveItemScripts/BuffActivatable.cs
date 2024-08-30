using System.Collections.Generic;
using UnityEngine;

public class BuffActivatable : Activatable
{
    public Buff buff;
    private ActiveItem source;
    private float duration = 0f;
    public override bool Activate()
    {
        source.AddBuff(buff.GetInstance(source, duration));
        return true;
    }

    public override void Setup(ActiveItem source)
    {
        this.source = source;
    }

    public override void ApplyStatBlock(CombinedStatBlock stats)
    {
        duration = stats.GetCombinedStatValue<ActiveItemDuration>();
    }

    public override StatBlockContext GetStatBlockContext(CombinedStatBlock baseContext, ActiveItem source)
    {
        CombinedStatBlock csb = buff.GetCombinedStatBlock(source.level);
        StatBlockContext statBlockContext = csb.GetCombinedContext();
        statBlockContext.AddGenericPrefixTooltip($"Applies {buff.buffName.AddColorToString(StatBlockContext.GoodColor)} for " +
            $"{baseContext.GetCombinedStatValue<ActiveItemDuration>().ToString().AddColorToString(StatBlockContext.HighlightColor)}s. Cooldown: " +
            $"{baseContext.GetCombinedStatValue<ActiveItemCooldown>().ToString().AddColorToString(StatBlockContext.HighlightColor)}s.");
        return statBlockContext;
    }
}
