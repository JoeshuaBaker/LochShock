using System;
using System.Collections.Generic;

[Serializable]
public abstract class StatType
{
    public float combinedValue;
    public abstract float Combine(float baseValue, IEnumerable<ValueTuple<float, float>> values);
    public virtual string ModifyTooltip(string tooltip)
    {
        return tooltip;
    }

    public static StatType GetStatTypeByEnum(StatBlock.BlockType type)
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
                return new Base();
        }
    }
}

public class Additive : StatType
{
    public override float Combine(float baseValue, IEnumerable<ValueTuple<float, float>> values)
    {
        combinedValue = baseValue;
        foreach(ValueTuple<float, float> value in values)
        {
            combinedValue += value.Item1 * value.Item2;
        }

        return combinedValue;
    }
}

public class PlusMult : StatType
{
    public override float Combine(float baseValue, IEnumerable<ValueTuple<float, float>> values)
    {
        combinedValue = 0;
        foreach (ValueTuple<float, float> value in values)
        {
            combinedValue += value.Item1 * value.Item2;
        }

        combinedValue = baseValue * (1 + combinedValue);

        return combinedValue;
    }
}

public class XMult : StatType
{
    public override float Combine(float baseValue, IEnumerable<ValueTuple<float, float>> values)
    {
        combinedValue = 0;
        foreach (ValueTuple<float, float> value in values)
        {
            combinedValue += value.Item1 * value.Item2;
        }

        combinedValue = baseValue * (1 + combinedValue);

        return combinedValue;
    }
}

public class Set : StatType
{
    public override float Combine(float baseValue, IEnumerable<ValueTuple<float, float>> values)
    {
        return 0;
    }
}

public class Base : StatType
{
    public override float Combine(float baseValue, IEnumerable<ValueTuple<float, float>> values)
    {
        return 0;
    }
}
