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

    public static StatCombineType GetStatTypeByEnum(StatBlock.BlockType type)
    {
        switch(type)
        {
            case StatBlock.BlockType.Additive:
                return new Additive();

            case StatBlock.BlockType.PlusMult:
                return new PlusMult();

            case StatBlock.BlockType.xMult:
                return new XMult();

            case StatBlock.BlockType.Set:
                return new Set();

            default:
            case StatBlock.BlockType.Base:
                return new BaseStat();
        }
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
        }

        return combinedValue;
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
        }

        combinedValue = baseValue * (1 + combinedValue);

        return combinedValue;
    }
}

public class XMult : StatCombineType
{
    public override int CombinePriority => 102;
    public override float Combine(float baseValue, IEnumerable<Stat> stats)
    {
        combinedValue = 0;
        foreach (Stat stat in stats)
        {
            combinedValue += stat.value * stat.stacks;
        }

        combinedValue = baseValue * (1 + combinedValue);

        return combinedValue;
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
            float value = stat.value;
            if(value > 0 && value < combinedValue)
            {
                combinedValue = value;
            }
        }

        return combinedValue;
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
            float value = stat.value;
            if (limitType == LimitType.Upper && combinedValue > value)
            {
                combinedValue = value;
            }
            else if(limitType == LimitType.Lower && combinedValue < value)
            {
                combinedValue = value;
            }
        }

        return combinedValue;
    }
}