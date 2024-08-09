public class HitEnemyCondition : StatCondition
{
    public override float CheckCondition(GameContext context)
    {
        return (context.hitEnemies == null || context.hitEnemies.Count == 0) ? 0f : 1f;
    }
}