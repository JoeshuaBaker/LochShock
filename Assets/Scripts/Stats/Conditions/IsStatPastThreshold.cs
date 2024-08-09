using UnityEngine;

[CreateAssetMenu(fileName = "IsStatPastThreshold", menuName = "NemesisShock/Conditions/Transformative/IsStatPastThreshold")]
public class IsStatPastThreshold : TransformativeCondition
{
    public enum HigherLowerThreshold
    {
        Higher_Than,
        Lower_Than
    }
    public float statThreshold = 1f;
    public HigherLowerThreshold overOrUnder = HigherLowerThreshold.Higher_Than;

    public override float CheckCondition(GameContext context)
    {
        if (base.CheckCondition(context) == 0f)
        {
            return 0f;
        }

        float statValue = context.player.Stats.GetCombinedStatValue(referenceStatType.GetType(), context);

        if(overOrUnder == HigherLowerThreshold.Higher_Than)
        {
            return statValue > statThreshold ? 1f : 0f;
        }
        else
        {
            return statValue < statThreshold ? 1f : 0f;
        }
    }

    public override string ConditionTooltipLabel => "when";
    public override string ConditionTooltipPostfix => ConditionTooltipLabel + $" {referenceStatType.GetType().Name} is {overOrUnder.ToString().Replace("_", " ")} {statThreshold}";
}
