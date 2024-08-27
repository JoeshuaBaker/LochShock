using System;

#region PlayerStats
public class Health : Stat 
{ 
    public override float Min => 0f;
}

public class Armor : Stat
{
    public override float Min => 0f;
}

public class MoveSpeed : Stat 
{
    public override float Min => 1f; 
}

public class Vision : Stat 
{ 
    public override float Min => 0f; 
    public override float Max => 1f;
    public override StatValueType ValueType => StatValueType.Percentage;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, isPercentage: true, conditions: conditions);
    }
}

public class DisassembleMult : Stat
{
    public override float Min => 0.1f;
    public override float Max => 1.35f;
    public override StatValueType ValueType => StatValueType.Percentage;
    public override string DisplayName()
    {
        return "bonus disassemble scrap";
    }

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, isPercentage: true, conditions: conditions);
    }
}
#endregion

#region GunStats
public class MagazineSize : Stat 
{ 
    public override float Min => 1f;
}

public class ReloadSpeed : Stat
{
    public override float Min => 1f / 60f;
    public override StatValueType ValueType => StatValueType.Rate;

    public override string DisplayName()
    {
        return combineType is BaseStat ? "reload time" : "reload speed";
    }

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if(combineType is BaseStat)
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, baseValue: Min, conditions: conditions);
        }
        else
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, baseValue: Min, conditions: conditions);
        }
    }
}

public class FireSpeed : Stat 
{ 
    public override float Min => .1f;
    public override float Max => 60f;
    public override string DisplayName()
    {
        return "shots per second";
    }
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, baseValue: Min, conditions: conditions);
    }
}

public class BulletStreams : Stat 
{ 
    public override float Min => 1f; 
    public override float Max => 10f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, baseValue: Min, conditions: conditions);
    }
}

public class BulletsPerShot : Stat 
{
    public override float Min => 1f;
    public override float Max => 25f;
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, baseValue: Min, conditions: conditions);
    }
}

public class SpreadAngle : Stat 
{ 
    public override float Min => -90f;
    public override float Max => 0f;
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if(combineType is Additive)
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, conditions: conditions, flipSign:true);
        }
        else if(combineType is Mult)
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, conditions: conditions, flipSign: true, positiveIsGood:false);
        }
        if (!(combineType is BaseStat))
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, conditions: conditions);
        }
        
    }
}

public class Accuracy : Stat
{
    public override float Min => 0f;
    public override float Max => 1f;
    public override StatValueType ValueType => StatValueType.Percentage;
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if (!(combineType is BaseStat))
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, isPercentage: true, baseValue: Max, conditions: conditions);
        }
    }
}

public class Damage : Stat
{
    public override float Min => 0f;
    public override string DisplayName()
    {
        return "gun damage";
    }
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if (combineType is BaseStat)
        {
            string damageType = "";
            if (source != null && source.damageType > 0)
            {
                foreach (DamageType oneType in Enum.GetValues(typeof(DamageType)))
                {
                    if (oneType != DamageType.None && source.damageType.HasFlag(oneType))
                    {
                        damageType += $"{oneType} ";
                    }
                }
            }

            damageType.Trim();

            context.AddContext(Name(), combineType, damageType.ToLower() + DisplayName(), value * TooltipStacks, conditions: conditions);
        }
        else
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, conditions: conditions);
        }
    }
}

public class Velocity : Stat 
{ 
    public override float Min => 0.00001f; 
    public override float Max => 300f;

    public override string DisplayName()
    {
        return "projectile velocity";
    }

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if (!(combineType is BaseStat))
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, conditions: conditions);
        }
    }
}

public class Size : Stat 
{
    public override float Min => 0.01f; 
    public override float Max => 5f;
    public override string DisplayName()
    {
        return "projectile size";
    }

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if(!(combineType is BaseStat))
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, conditions: conditions);
        }
    }
}

public class Knockback : Stat
{
    public override float Min => 0f;
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if (!(combineType is BaseStat))
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, conditions: conditions);
        }
    }
}

public class Bounce : Stat 
{ 
    public override float Min => 0f; 
}

public class Pierce : Stat 
{ 
    public override float Min => 0f; 
}

public class Lifetime : Stat 
{ 
    public override float Min => 0.05f; 
    public override float Max => 5f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if (!(combineType is BaseStat))
        {
            base.UpdateStatBlockContext(ref context);
        }
    }
}

public class SecondaryHitDamage : Stat
{
    public override float Min => 0.1f;
    public override float Max => 2f;
    public float BaseValue = 0.75f;
    public override StatValueType ValueType => StatValueType.Percentage;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, isPercentage: true, baseValue: BaseValue, conditions: conditions);
    }
}
#endregion

#region ActiveItemStats
//damage, cooldown, charges, active time, size
public class ActiveItemDamage : Stat
{
    public override float Min => 0f;
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if (combineType is BaseStat)
        {
            string damageType = "";
            if (source != null && source.damageType > 0)
            {
                foreach (DamageType oneType in Enum.GetValues(typeof(DamageType)))
                {
                    if (oneType != DamageType.None && source.damageType.HasFlag(oneType))
                    {
                        damageType += $"{oneType} ";
                    }
                }
            }

            damageType.Trim();

            context.AddContext(Name(), combineType, damageType.ToLower() + DisplayName(), value * TooltipStacks, conditions: conditions);
        }
        else
        {
            context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, conditions: conditions);
        }
    }
}

public class ActiveItemCooldown : Stat
{
    public override float Min => .1f;
    public override StatValueType ValueType => StatValueType.Rate;
}

public class ActiveItemCharges : Stat
{
    public override float Min => 1f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, baseValue:Min, conditions: conditions);
    }
}

public class ActiveItemDuration : Stat
{
    public override float Min => 1f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, DisplayName(), value * TooltipStacks, baseValue: Min, conditions: conditions);
    }
}
#endregion