using System;
using UnityEngine;

[Serializable]
public abstract class Stat
{
    public enum StatValueType
    {
        Value,
        Percentage,
        RelativeToBase
    }

    [SerializeReference, SerializeReferenceButton] public StatCombineType combineType;
    public float value;
    [NonSerialized]
    public float stacks = 1f;
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
        context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * stacks);
    }

    public virtual float Min => -1f;
    public virtual float Max => -1f;
    public virtual StatValueType ValueType => StatValueType.Value;

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
}
