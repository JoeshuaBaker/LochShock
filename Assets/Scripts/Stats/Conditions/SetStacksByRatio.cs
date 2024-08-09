using UnityEngine;

[CreateAssetMenu(fileName = "SetStacksByRatio", menuName = "NemesisShock/Conditions/Transformative/SetStacksByRatio")]
public class SetStacksByRatio : TransformativeCondition
{
    public float statRatio = .1f;
    public bool allowFractionalStacks = false;

    public override float CheckCondition(GameContext context)
    {
        if (base.CheckCondition(context) == 0f)
        {
            return 0f;
        }

        float statValue = context.player.Stats.GetCombinedStatValue(referenceStatType.GetType(), context);

        return (allowFractionalStacks) ? (statValue * statRatio) : (int)(statValue * statRatio);
    }

    public override string ConditionTooltipLabel => "per";
    public override string ConditionTooltipPostfix => ConditionTooltipLabel + $" {statRatio * 100f} {referenceStatType.GetType().Name} you have.";
}
