using UnityEngine;

[CreateAssetMenu(fileName = "IsHitEnemyNearPlayer", menuName = "NemesisShock/Conditions/HitEnemy/IsHitEnemyNearPlayer")]
public class IsHitEnemyNearPlayer : HitEnemyCondition
{
    public float distanceThreshold = 3f;

    public override float CheckCondition(GameContext context)
    {
        if (base.CheckCondition(context) == 0f)
        {
            return 0f;
        }

        foreach (Enemy enemy in context.damageContext.hitEnemies)
        {
            if ((enemy.transform.position - context.player.transform.position).magnitude <= distanceThreshold)
            {
                return 1f;
            }
        }

        return 0f;
    }

    public override string ConditionTooltipLabel => "to";
    public override string ConditionTooltipPostfix => ConditionTooltipLabel + $" enemies within {distanceThreshold}m";
}
