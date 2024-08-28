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
    public virtual string ConditionTooltipValueLabelInsert => "";
    public bool HasValueLabelInsert => !string.IsNullOrEmpty(ConditionTooltipValueLabelInsert);
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
            int lastPercentIndex = value.LastIndexOf("%");
            string[] numberLabelPieces = { value.Substring(0, lastPercentIndex + 1), value.Substring(lastPercentIndex + 1) };
            string numberPiece = numberLabelPieces[0];
            string labelPiece = numberLabelPieces[1];
            StatCondition firstCondition = conditions[0];
            string prefixAcc = $"{firstCondition.ConditionTooltipPrefix}";
            string postfixAcc = $"{firstCondition.ConditionTooltipPostfix}";
            string afterNumberInsertAcc = $"{firstCondition.ConditionTooltipValueLabelInsert}";

            for (int i = 1; i < conditions.Count; i++)
            {
                StatCondition condition = conditions[i];

                if(condition.HasPrefix)
                {
                    prefixAcc += CalculateAppendString(prefixAcc, condition, condition.ConditionTooltipPrefix);
                }
                    
                if (condition.HasPostfix)
                {
                    postfixAcc += CalculateAppendString(postfixAcc, condition, condition.ConditionTooltipPostfix); ;
                }

                if(condition.HasValueLabelInsert)
                {
                    afterNumberInsertAcc += CalculateAppendString(afterNumberInsertAcc, condition, condition.ConditionTooltipValueLabelInsert); ;
                }
            }

            prefixAcc = prefixAcc.Trim();
            postfixAcc = postfixAcc.Trim();
            afterNumberInsertAcc = afterNumberInsertAcc.Trim();

            string prefixSpace = !string.IsNullOrEmpty(prefixAcc) ? " " : "";
            string postfixSpace = !string.IsNullOrEmpty(postfixAcc) ? " " : "";
            string numberLabelSpace = !string.IsNullOrEmpty(afterNumberInsertAcc) ? " " : "";

            return $"{prefixAcc}{prefixSpace}{numberPiece}{numberLabelSpace}{afterNumberInsertAcc}{labelPiece}{postfixSpace}{postfixAcc}";
        }
    }

    private static string CalculateAppendString(string currentAccumulator, StatCondition condition, string valueToAppend)
    {
        bool hasAccumulatorValue = !string.IsNullOrEmpty(currentAccumulator);
        string booleanCombineString = hasAccumulatorValue ? condition.booleanCombineType.ToString().ToLower() : "";

        string label = condition.ConditionTooltipLabel;
        bool hasLabel = !string.IsNullOrEmpty(label);
        string replaceLabelValue = hasLabel ? valueToAppend.Replace(label, valueToAppend).Trim() : valueToAppend;

        return $" {booleanCombineString} {replaceLabelValue}";
    }
}
