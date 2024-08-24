using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActiveItem))]
public abstract class Activatable : MonoBehaviour
{
    public abstract void Activate();
    public abstract void Setup(ActiveItem source);
    public abstract void ApplyStatBlock(CombinedStatBlock stats);
    public virtual void OnLevelUp()
    {

    }
    public abstract StatBlockContext GetStatBlockContext(StatBlockContext baseStatBlockContext, ActiveItem source);
}
