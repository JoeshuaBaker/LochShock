using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SetStacksByRatio", menuName = "NemesisShock/Conditions/Transformative/SetStacksByRatio")]
public class SetStacksByRatio : TransformativeCondition
{
    public float statRatio = .1f;
    public bool allowFractionalStacks = false;
    public bool matchStatExactly;
    public float tooltipRatioMultiplier = 100f;

    public override float CheckCondition(GameContext context)
    {
        if (base.CheckCondition(context) == 0f)
        {
            return 0f;
        }

        if(!matchStatExactly)
        {
            float statValue = context.player.Stats.GetCombinedStatValue(referenceStatType.GetType(), context);

            return (allowFractionalStacks) ? (statValue * statRatio) : (int)(statValue * statRatio);
        }
        else
        {
            if(referenceStatType == null || referenceStatType.combineType == null)
            {
                Debug.Log("set stat and combine type or stacks will be 0");
                return 0;
            }
            HashSet<Stat> statBucket = context.player.Stats.GetStatBucketByTypeAndCombineType(referenceStatType);
            if (statBucket == null)
            {
                return 0;
            }
            float total = 0f;
            foreach (Stat stat in statBucket)
            {
                float value = 0;
                if (stat.conditions != null && stat.conditions.Count > 0)
                {
                    value = stat.value * (stat.stacks * stat.conditionStacks);
                }
                else
                {
                    value = stat.value * stat.stacks;
                }

                total += value;
            }


            //replace this with stack calculation
            return (allowFractionalStacks) ? (total * statRatio) : (int)(total * statRatio);
        }
    }

    public override string ConditionTooltipLabel => "For every";
    public override string ConditionTooltipPrefix => ConditionTooltipLabel + $" {statRatio * tooltipRatioMultiplier} {referenceStatType.GetType().Name} you have, gain";
}
