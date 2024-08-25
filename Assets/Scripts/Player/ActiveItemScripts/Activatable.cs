using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActiveItem))]
public abstract class Activatable : MonoBehaviour
{
    public abstract bool Activate();
    public abstract void Setup(ActiveItem source);
    public abstract void ApplyStatBlock(CombinedStatBlock stats);
    public virtual void OnLevelUp()
    {

    }
    public abstract StatBlockContext GetStatBlockContext(CombinedStatBlock baseStatBlockContext, ActiveItem source);
}
