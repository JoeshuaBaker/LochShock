public class BuffActivatable : Activatable
{
    public Buff buff;
    private ActiveItem source;
    private float duration = 0f;
    public override void Activate()
    {
        Player.activePlayer.AddBuff(buff.GetInstance(source, duration));
    }

    public override void Setup(ActiveItem source)
    {
        this.source = source;
    }

    public override void ApplyStatBlock(CombinedStatBlock stats)
    {
        duration = stats.GetCombinedStatValue<ActiveItemDuration>();
    }

    public override StatBlockContext GetStatBlockContext(StatBlockContext baseContext)
    {
        StatBlockContext statBlockContext = buff.newStats.GetStatBlockContext();
        statBlockContext.AddGenericTooltip($"Applies {StatBlockContext.GoodColor}{buff.name}</color> for " +
            $"{StatBlockContext.HighlightColor}{duration}</color>s. " +
            $"Cooldown: {StatBlockContext.HighlightColor}{source.cooldown}</color>s.");
        return statBlockContext;
    }
}
