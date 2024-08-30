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
        CombinedStatBlock csb = new CombinedStatBlock();
        List<StatBlock> statBlocks = buff.GetStatBlocks(source.level);
        statBlocks.AddRange(baseContext.sourcesList);
        csb.UpdateSources(statBlocks);
        StatBlockContext statBlockContext = csb.GetCombinedContext();
        statBlockContext.AddGenericPrefixTooltip($"Applies {buff.buffName.AddColorToString(StatBlockContext.GoodColor)}.");
        return statBlockContext;
    }
}
