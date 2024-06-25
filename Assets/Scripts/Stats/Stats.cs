using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Stat
{
    [SerializeReference, SerializeReferenceButton] public StatType statType;
    public float value;
    public virtual float Min()
    {
        return 0;
    }
    public virtual float Max()
    {
        return 0;
    }
}

public class Health : Stat
{

}

public class WalkSpeed : Stat
{

}

public class RunSpeed : Stat
{

}

public class Vision : Stat 
{

}
