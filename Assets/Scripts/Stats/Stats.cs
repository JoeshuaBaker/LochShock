public class Health : Stat 
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
    public override StatValueType ValueType => StatValueType.Rate;
}

public class FireSpeed : Stat 
{ 
    public override float Min => 1f / 60f;
    public override StatValueType ValueType => StatValueType.Rate;
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
    public override float Min => -90f;
    public override float Max => 0f;
    public override void UpdateStatBlockContext(ref StatBlockContext context)
    {
        if (!(combineType is BaseStat))
        {
            context.AddContext(Name(), combineType, Name().SplitCamelCaseLower(), value * stacks);
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
