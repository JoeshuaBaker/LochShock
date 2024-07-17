using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class StatCombineType : IComparer<StatCombineType>, IComparable<StatCombineType>
{
    //float that stores last combined value, good for debugging!
    protected float combinedValue;

    //Determines what order stats are combined in. Lower priorities are combined first, higher priorities last.
    public abstract int CombinePriority { get; }

    //Tuple is designed to be passed in as "value, stacks"
    public abstract float Combine(float baseValue, IEnumerable<Stat> stats);

    public virtual string ModifyTooltip(string tooltip, float value)
    {
        return tooltip;
    }

    public virtual string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        return "";
    }

    public virtual string GetTooltipPostfix(string valueName)
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
}

public class Additive : StatCombineType
{
    public override int CombinePriority => 100;
    public override float Combine(float baseValue, IEnumerable<Stat> stats)
    {
        combinedValue = baseValue;
        foreach(Stat stat in stats)
        {
            combinedValue += stat.value * stat.stacks;
            combinedValue = stat.Clamp(combinedValue);
        }

        return combinedValue;
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

public class PlusMult : StatCombineType
{
    public override int CombinePriority => 101;
    public override float Combine(float baseValue, IEnumerable<Stat> stats)
    {
        combinedValue = 0;
        foreach (Stat stat in stats)
        {
            combinedValue += stat.value * stat.stacks;
            combinedValue = stat.Clamp(combinedValue);
        }

        combinedValue = baseValue * (1 + combinedValue);

        return combinedValue;
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

public class XMult : StatCombineType
{
    public override int CombinePriority => 102;
    public override float Combine(float baseValue, IEnumerable<Stat> stats)
    {
        combinedValue = baseValue;
        Func<float, float, float> xMultCombine = (float stat, float stacks) =>
        {
            float mult = 1f + stat;
            if (mult > 0 && mult < 1f)
            {
                return Mathf.Pow(mult, stacks);
            }
            else
            {
                return Mathf.Pow(mult, stacks);
            }
        };

        foreach (Stat stat in stats)
        {
            combinedValue *= xMultCombine(stat.value, stat.stacks);
            combinedValue = stat.Clamp(combinedValue);
        }

        return combinedValue;
    }

    public override string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        bool plusMinus = value >= 0;
        if (flipSign)
        {
            plusMinus = !plusMinus;
        }

        return plusMinus ? "x" : "-x";
    }
}

public class Set : StatCombineType
{
    public override int CombinePriority => 10000;
    public override float Combine(float baseValue, IEnumerable<Stat> stats)
    {
        combinedValue = float.MaxValue;

        foreach (Stat stat in stats)
        {
            if (stat.stacks < 1)
                continue;

            float value = stat.value;
            if (value > 0 && value < combinedValue)
            {
                combinedValue = value;
            }

            combinedValue = stat.Clamp(combinedValue);
        }

        if(combinedValue == float.MaxValue)
        {
            combinedValue = baseValue;
        }

        return combinedValue;
    }

    public override string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        return $"{valueName.SplitCamelCaseLower()} is always ";
    }

    public override string GetTooltipPostfix(string valueName)
    {
        return "";
    }
}

public class BaseStat : Additive
{
    public override int CombinePriority => 0;
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
    public override float Combine(float baseValue, IEnumerable<Stat> stats)
    {
        combinedValue = baseValue;

        foreach (Stat stat in stats)
        {
            if (stat.stacks < 1)
                continue;

            float value = stat.value;
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

        return combinedValue;
    }

    public override string GetTooltipPrefix(string valueName, bool flipSign, float value)
    {
        return $"{valueName.SplitCamelCaseLower()} can't go {((limitType == LimitType.Upper) ? "above" : "below")} {value}.";
    }

    public override string GetTooltipPostfix(string valueName)
    {
        return "";
    }
}