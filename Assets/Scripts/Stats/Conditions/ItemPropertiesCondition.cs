public class ItemPropertiesCondition : StatCondition
{
    public enum GunOrActiveItem
    {
        Gun,
        ActiveItem,
        Either
    }
    public GunOrActiveItem mode = GunOrActiveItem.Gun;

    public override float CheckCondition(GameContext context)
    {
        if (mode == GunOrActiveItem.Gun || mode == GunOrActiveItem.Either)
        {
            return context.player.inventory.activeGun != null ? 1f : 0f;
        }
        else if (mode == GunOrActiveItem.ActiveItem || mode == GunOrActiveItem.Either)
        {
            return context.player.inventory.activeItem != null ? 1f : 0f;
        }

        return 0;
    }
}
