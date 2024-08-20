public class HitEnemyCondition : StatCondition
{
    public override float CheckCondition(GameContext context)
    {
        return (context.damageContext.hitEnemies == null || context.damageContext.hitEnemies.Count == 0) ? 0f : 1f;
    }
}