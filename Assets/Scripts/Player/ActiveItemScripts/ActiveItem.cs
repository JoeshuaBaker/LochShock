using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveItem : Item
{
    public float cooldown = 1f;
    public float cooldownTimer = 0f;
    public abstract void Activate();
    public abstract void Setup();

    public virtual void Update()
    {
        cooldownTimer = Mathf.Max(cooldownTimer - Time.deltaTime, 0f);
    }
}
