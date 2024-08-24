public class BuffActivatable : Activatable
{
    public Buff buff;
    private ActiveItem source;
    private float duration = 0f;
    public override void Activate()
    {
        source.AddBuff(buff.GetInstance(source, duration));
    }

    public override void Setup(ActiveItem source)
    {
        this.source = source;
    }

    public override void ApplyStatBlock(CombinedStatBlock stats)
    {
        duration = stats.GetCombinedStatValue<ActiveItemDuration>();
    }

    public override StatBlockContext GetStatBlockContext(StatBlockContext baseContext, ActiveItem source)
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

        StatBlockContext statBlockContext = buff.newStats.GetStatBlockContext();
        statBlockContext.AddGenericTooltip($"Applies {StatBlockContext.GoodColor}{buff.buffName}</color> for " +
            $"{StatBlockContext.HighlightColor}{duration}</color>s. " +
            $"Cooldown: {StatBlockContext.HighlightColor}{source.Cooldown}</color>s.");
        return statBlockContext;
    }
}
