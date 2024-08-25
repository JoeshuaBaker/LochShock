using System.Collections.Generic;

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
        float duration;
        if (source.setup)
        {
            duration = source.combinedStats.GetCombinedStatValue<ActiveItemDuration>();
        }
        else
        {
            duration = source.baseItemCombinedStats.GetCombinedStatValue<ActiveItemDuration>();
        }

        CombinedStatBlock csb = new CombinedStatBlock();
        List<StatBlock> statBlocks = buff.GetStatBlocks(source.level);
        statBlocks.AddRange(baseContext.sourcesList);
        csb.UpdateSources(statBlocks);

        StatBlockContext statBlockContext = csb.GetCombinedContext();
        statBlockContext.AddGenericTooltip($"Applies {StatBlockContext.GoodColor}{buff.buffName}</color> for " +
            $"{StatBlockContext.HighlightColor}{duration}</color>s. " +
            $"Cooldown: {StatBlockContext.HighlightColor}{source.Cooldown}</color>s.");
        return statBlockContext;
    }
}
