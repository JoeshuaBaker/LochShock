using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffActiveItem : ActiveItem
{
    public Buff buff;
    public override void Activate()
    {
        Player.activePlayer.AddBuff(buff.GetInstance());
        cooldownTimer = cooldown;
    }

    public override void Setup()
    {

    }

    public override StatBlockContext GetStatBlockContext()
    {
        StatBlockContext statBlockContext = buff.newStats.GetStatBlockContext();
        statBlockContext.AddGenericTooltip($"Applies {StatBlockContext.GoodColor}{buff.name}</color> for " +
            $"{StatBlockContext.HighlightColor}{buff.duration}</color>s. " +
            $"Cooldown: {StatBlockContext.HighlightColor}{cooldown}</color>s.");
        return statBlockContext;
    }
}
