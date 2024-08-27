using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class StatCombineType : IComparer<StatCombineType>, IComparable<StatCombineType>
{
    //Determines what order stats are combined in. Lower priorities are combined first, higher priorities last.
    public abstract int CombinePriority { get; }

    //Tuple is designed to be passed in as "value, stacks"
    public abstract void Combine(ref float baseValue, ref float aggregate, IEnumerable<Stat> stats);

    public virtual string ModifyTooltip(string tooltip, float value)
    {
        return tooltip;
    }

    public virtual string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        return "";
    }

    public virtual string StatNameTooltip(string valueName)
    {
        return valueName.SplitCamelCaseLower();
    }

    public int Compare(StatCombineType x, StatCombineType y)
    {
        return x.CombinePriority.CompareTo(y.CombinePriority);
    }

    public override int GetHashCode()
    {
        return CombinePriority;
    }

    public override bool Equals(object obj)
    {
        var item = obj as StatCombineType;

        if (item == null)
        {
            return false;
        }
        return item.CombinePriority == this.CombinePriority;
    }

    public int CompareTo(StatCombineType other)
    {
        return this.CombinePriority.CompareTo(other.CombinePriority);
    }

    protected float GetStatValue(Stat stat)
    {
        if (stat.conditions != null && stat.conditions.Count > 0)
        {
            float value = stat.value * (stat.stacks * stat.conditionStacks);
            stat.conditionStacks = 0f;
            return value;
        }
        else
        {
            float value = stat.value * stat.stacks;
            return value;
        }
    }
}

public class BaseStat : StatCombineType
{
    public override int CombinePriority => 0;

    public override void Combine(ref float baseValue, ref float aggregate, IEnumerable<Stat> stats)
    {
        Stat first = stats.FirstOrDefault();
        if (first == null)
        {
            return;
        }

        float combinedValue = 0;
        foreach (Stat stat in stats)
        {
            combinedValue += GetStatValue(stat);
            stat.conditionStacks = 0f;
        }

        combinedValue = first.Clamp(combinedValue);
        baseValue = combinedValue;
    }

    public override string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        return "";
    }
}

public class BasePower : Additive
{
    public override int CombinePriority => 1;

    public override string StatNameTooltip(string valueName)
    {
        return "Base " + valueName.SplitCamelCaseLower();
    }
}

public class Additive : StatCombineType
{
    public override int CombinePriority => 101;
    public override void Combine(ref float baseValue, ref float aggregate, IEnumerable<Stat> stats)
    {
        Stat first = stats.FirstOrDefault();
        if (first == null)
        {
            return;
        }

        float combinedValue = baseValue;
        foreach (Stat stat in stats)
        {
            combinedValue += GetStatValue(stat);
        }

        aggregate += first.ValueType == Stat.StatValueType.Rate ? -combinedValue : combinedValue;
        aggregate = first.Clamp(aggregate);
    }

    public override string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        bool plusMinus = value >= 0;
        if (flipSign)
        {
            plusMinus = !plusMinus;
        }

        return plusMinus ? "+" : "-";
    }

    public override string StatNameTooltip(string valueName)
    {
        return valueName.SplitCamelCaseLower();
    }
}

public class Mult : StatCombineType
{
    public override int CombinePriority => 100;
    public override void Combine(ref float baseValue, ref float aggregate, IEnumerable<Stat> stats)
    {
        Stat first = stats.FirstOrDefault();
        if (first == null)
        {
            return;
        }

        float combinedValue = 0;
        foreach (Stat stat in stats)
        {
            combinedValue += GetStatValue(stat);
        }

        if (first.ValueType == Stat.StatValueType.Rate)
        {
            if (combinedValue < 0f)
            {
                aggregate += baseValue * (1 + Mathf.Abs(combinedValue));
            }
            else
            {
                aggregate += baseValue / (1 + combinedValue);
            }
        }
        else
        {
            aggregate += baseValue * (1 + combinedValue);
        }

        aggregate = first.Clamp(aggregate);
    }

    public override string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        bool plusMinus = value >= 0;
        if (flipSign)
        {
            plusMinus = !plusMinus;
        }

        return plusMinus ? "+" : "-";
    }
}

public class Set : StatCombineType
{
    public override int CombinePriority => 10000;
    public override void Combine(ref float baseValue, ref float aggregate, IEnumerable<Stat> stats)
    {
        float combinedValue = float.MaxValue;

        foreach (Stat stat in stats)
        {
            float value = GetStatValue(stat);
            if (value == 0)
                continue;

            if (value > 0 && value < combinedValue)
            {
                combinedValue = value;
            }

            combinedValue = stat.Clamp(combinedValue);
        }

        if(combinedValue != float.MaxValue)
        {
            aggregate = combinedValue;
        }
    }

    public override string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        return $"{valueName.SplitCamelCaseLower()} is always ";
    }

    public override string StatNameTooltip(string valueName)
    {
        return "";
    }
}

public class Limit : StatCombineType
{
    public enum LimitType
    {
        Upper,
        Lower
    }
    public LimitType limitType;
    public override int CombinePriority => 9999;
    public override void Combine(ref float baseValue, ref float aggregate, IEnumerable<Stat> stats)
    {
        float combinedValue = aggregate;

        foreach (Stat stat in stats)
        {
            float value = GetStatValue(stat);
            if (value == 0)
                continue;

            if (limitType == LimitType.Upper && combinedValue > value)
            {
                combinedValue = value;
            }
            else if (limitType == LimitType.Lower && combinedValue < value)
            {
                combinedValue = value;
            }

            combinedValue = stat.Clamp(combinedValue);
        }

        aggregate = combinedValue;
    }

    public override string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        return $"{valueName.SplitCamelCaseLower()} can't be {((limitType == LimitType.Upper) ? "above" : "below")} ";
    }

    public override string StatNameTooltip(string valueName)
    {
        return "";
    }
}