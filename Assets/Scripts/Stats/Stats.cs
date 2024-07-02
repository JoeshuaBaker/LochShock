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

    public Stat(float value, StatBlock.BlockType statType)
    {
        this.value = value;
        this.combineType = StatCombineType.GetStatTypeByEnum(statType);
        stacks = 1f;
    }

    public virtual float Min => 0;
    public virtual float Max => 0;

    public override string ToString()
    {
        return "Stat Name: " + this.GetType().Name + ", Value: " + value + ", StatType: " + combineType.GetType().Name;
    }

    public static Stat CreateStatByString(string statName, float value, StatBlock.BlockType statType)
    {
        Stat stat;
        switch(statName)
        {
            default:
                Debug.LogError("Could not map switch for " + statName);
                return null;

            case "health":
                stat = new Health();
                break;

            case "walkSpeed":
                stat = new WalkSpeed();
                break;

            case "runSpeed":
                stat = new RunSpeed();
                break;

            case "totalVision":
                stat = new Vision();
                break;

            case "magazineSize":
                stat = new MagazineSize();
                break;

            case "reloadSpeed":
                stat = new ReloadSpeed();
                break;

            case "fireSpeed":
                stat = new FireSpeed();
                break;

            case "bulletStreams":
                stat = new BulletStreams();
                break;

            case "bulletsPerShot":
                stat = new BulletsPerShot();
                break;

            case "spreadAngle":
                stat = new SpreadAngle();
                break;

            case "accuracy":
                stat = new Accuracy();
                break;

            case "damage":
                stat = new Damage();
                break;

            case "velocity":
                stat = new Velocity();
                break;

            case "size":
                stat = new Size();
                break;

            case "knockback":
                stat = new Knockback();
                break;

            case "bounce":
                stat = new Bounce();
                break;

            case "pierce":
                stat = new Pierce();
                break;

            case "lifetime":
                stat = new Lifetime();
                break;
        }

        stat.value = value;
        stat.combineType = StatCombineType.GetStatTypeByEnum(statType);
        stat.stacks = 1f;
        return stat;
    }
}

public class Health : Stat { }
public class WalkSpeed : Stat { }
public class RunSpeed : Stat { }
public class Vision : Stat { }
public class MagazineSize : Stat { }
public class ReloadSpeed : Stat { }
public class FireSpeed : Stat { }
public class BulletStreams : Stat { }
public class BulletsPerShot : Stat { }
public class SpreadAngle : Stat { }
public class Accuracy : Stat { }
public class Damage : Stat { }
public class Velocity : Stat { }
public class Size : Stat { }
public class Knockback : Stat { }
public class Bounce : Stat { }
public class Pierce : Stat { }
public class Lifetime : Stat { }
