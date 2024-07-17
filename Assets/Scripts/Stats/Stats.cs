using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Stat
{
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

    public string Name()
    {
        return GetType().Name;
    }

    public virtual void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * stacks);
    }

    public virtual float Min => -1f;
    public virtual float Max => -1f;

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

public class Health : Stat 
{ 
    public override float Min => 0f; 

}

public class WalkSpeed : Stat 
{ 
    public override float Min => 0f; 
}

public class RunSpeed : Stat 
{
    public override float Min => 1f; 
}

public class Vision : Stat 
{ 
    public override float Min => 0f; 
    public override float Max => 1f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * stacks, isPercentage: true);
    }
}

public class MagazineSize : Stat 
{ 
    public override float Min => 1f; 
}

public class ReloadSpeed : Stat 
{ 
    public override float Min => 1f / 60f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * stacks, isPercentage: true, positiveIsGood: false, flipSign: true);
    }
}

public class FireSpeed : Stat 
{ 
    public override float Min => 1f / 60f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, "Fire Rate", value * stacks, isPercentage: false, positiveIsGood: false, flipSign: true);
    }
}

public class BulletStreams : Stat 
{ 
    public override float Min => 1f; 
    public override float Max => 10f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * stacks, baseValue: Min);
    }
}

public class BulletsPerShot : Stat 
{
    public override float Min => 1f;
    public override float Max => 25f;
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * stacks, baseValue: Min);
    }
}

public class SpreadAngle : Stat 
{ 
    public override float Min => 0f;
    public override float Max => 90f;
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * stacks, isPercentage: false, positiveIsGood: false, flipSign: false);
    }
}

public class Accuracy : Stat 
{ 
    public override float Min => 0f; 
    public override float Max => 1f;
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * stacks, isPercentage: true, baseValue: Max);
    }
}

public class Damage : Stat 
{ 
    public override float Min => 0f; 
}

public class Velocity : Stat 
{ 
    public override float Min => 1f; 
    public override float Max => 300f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if (!(combineType is BaseStat))
        {
            context.AddContext(Name(), combineType, "Bullet Velocity", value * stacks);
        }
    }
}

public class Size : Stat 
{ 
    public override float Min => 0.01f; 
    public override float Max => 5f;

    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if(!(combineType is BaseStat))
        {
            context.AddContext(Name(), combineType, "Bullet Size", value * stacks);
        }
    }
}

public class Knockback : Stat 
{ 
    public override float Min => 0f; 
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
