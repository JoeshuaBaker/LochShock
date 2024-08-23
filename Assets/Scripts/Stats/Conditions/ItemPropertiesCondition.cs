public class ItemPropertiesCondition : StatCondition
{
    public override float CheckCondition(GameContext context)
    {
        return context.player.inventory.activeGun == null ? 0f : 1f;
    }
}
