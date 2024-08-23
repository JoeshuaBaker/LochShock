using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Stat
{
    public enum StatValueType
    {
        Value,
        Percentage,
        Rate
    }

    [SerializeReference, SerializeReferenceButton] public StatCombineType combineType;
    public float value;
    public List<StatCondition> conditions;

    [NonSerialized]
    public float stacks = 1f;
    [NonSerialized]
    public float tempStacks = 0f;
    [NonSerialized]
    public Item source;

    public Stat()
    {
        stacks = 1f;
    }

    public Stat(float value)
    {
        this.value = value;
        stacks = 1f;
    }

    public virtual string Name()
    {
        return GetType().Name;
    }

    public virtual void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * TooltipStacks, conditions: conditions);
    }

    public virtual float Min => -1f;
    public virtual float Max => -1f;
    public virtual StatValueType ValueType => StatValueType.Value;
    public float TooltipStacks => (conditions == null || conditions.Count == 0) ? stacks : 1f;

    public float Clamp(float value)
    {
        float clampedValue = value;
        if (Min > -1f && value < Min)
            clampedValue = Min;
        if (Max > -1f && value > Max)
            clampedValue = Max;

        return clampedValue;
    }

    public override string ToString()
    {
        return "Stat Name: " + this.GetType().Name + ", Value: " + value + ", StatType: " + combineType.GetType().Name;
    }

    public void CheckSetConditionStacks(GameContext context)
    {
        if (conditions == null || conditions.Count == 0)
            return;

        stacks = 0f;
        tempStacks = 0f;
        foreach (StatCondition condition in conditions)
        {
            float newStacks = condition.CheckCondition(context);
            if (condition.booleanCombineType == StatCondition.BooleanCombineType.And)
            {
                if (newStacks == 0f)
                {
                    tempStacks = 0f;
                    return;
                }
            }

            tempStacks = Mathf.Max(tempStacks, newStacks);
        }
    }
}
