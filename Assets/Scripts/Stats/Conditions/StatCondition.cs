using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class StatCondition : ScriptableObject
{
    public enum BooleanCombineType
    {
        And,
        Or
    }

    public BooleanCombineType booleanCombineType = BooleanCombineType.And;
    public abstract float CheckCondition(GameContext context);

    public virtual string ConditionTooltipLabel => "";
    public virtual string ConditionTooltipPrefix => "";
    public bool HasPrefix => !string.IsNullOrEmpty(ConditionTooltipPrefix);
    public virtual string ConditionTooltipPostfix => "";
    public bool HasPostfix => !string.IsNullOrEmpty(ConditionTooltipPrefix);

    public static string FormatConditionTooltip(List<StatCondition> conditions, string value)
    {
        if(conditions == null || conditions.Count == 0)
        {
            return value;
        }
        else
        {
            StatCondition firstCondition = conditions[0];
            string prefixAcc = $"{firstCondition.ConditionTooltipPrefix}";
            string postfixAcc = $"{firstCondition.ConditionTooltipPostfix}";

            for (int i = 1; i < conditions.Count; i++)
            {
                StatCondition condition = conditions[i];
                string label = condition.ConditionTooltipLabel;

                if(condition.HasPrefix)
                {
                    prefixAcc += $" {condition.booleanCombineType.ToString().ToLower()} {condition.ConditionTooltipPrefix.Replace(label, "").Trim()}";
                }
                    
                if (condition.HasPostfix)
                {
                    postfixAcc += $" {condition.booleanCombineType.ToString().ToLower()} {condition.ConditionTooltipPostfix.Replace(label, "").Trim()}";
                }
            }

            prefixAcc = prefixAcc.Trim();
            postfixAcc = postfixAcc.Trim();

            string prefixSpace = !string.IsNullOrEmpty(prefixAcc) ? " " : "";
            string postfixSpace = !string.IsNullOrEmpty(postfixAcc) ? " " : "";

            return $"{prefixAcc}{prefixSpace}{value}{postfixSpace}{postfixAcc}";
        }
    }
}
