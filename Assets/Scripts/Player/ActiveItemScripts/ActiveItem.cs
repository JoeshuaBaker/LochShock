using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItem : Item
{
    public Activatable activatable;
    public int currentCharges = 1;
    [SerializeField] private int maxCharges = 1;

    public int MaxCharges
    {
        get
        {
            if(setup)
            {
                return (int)combinedStats.GetCombinedStatValue<ActiveItemCharges>();
            }
            else
            {
                return (int)baseItemCombinedStats.GetCombinedStatValue<ActiveItemCharges>();
            }
        }
    }

    [SerializeField] private float cooldown = 1f;

    public float Cooldown
    {
        get
        {
            if (setup)
            {
                return (int)combinedStats.GetCombinedStatValue<ActiveItemCooldown>();
            }
            else
            {
                return (int)baseItemCombinedStats.GetCombinedStatValue<ActiveItemCooldown>();
            }
        }
    }

    public float percentCooldownComplete = 0f;
    public float CooldownTimer => percentCooldownComplete * cooldown;
    public float CooldownCountdown => (1f - percentCooldownComplete) * cooldown;

    public bool setup = false;

    public void Activate()
    {
        if(IsReady())
        {
            activatable.Activate();
            currentCharges -= 1;
        }
    }

    public void Setup()
    {
        if(activatable == null)
        {
            activatable = GetComponent<Activatable>();
        }

        activatable.Setup(this);
    }

    public void ApplyStatBlock(CombinedStatBlock stats)
    {
        combinedStats = stats;
        int newCharges = (int)stats.GetCombinedStatValue<ActiveItemCharges>();
        cooldown = stats.GetCombinedStatValue<ActiveItemCooldown>();

        if (!setup)
        {
            setup = true;
            maxCharges = newCharges;
            currentCharges = maxCharges;
            percentCooldownComplete = 0f;
            return;
        }

        if (newCharges > maxCharges)
        {
            currentCharges += newCharges - maxCharges;
        }

        maxCharges = newCharges;
        currentCharges = Mathf.Min(currentCharges, maxCharges);
        activatable.ApplyStatBlock(stats);
    }

    public override StatBlockContext GetStatBlockContext()
    {
        return activatable.GetStatBlockContext(base.GetStatBlockContext(), this);
    }

    public bool IsReady()
    {
        return currentCharges >= 1;
    }

    public override void Update()
    {
        base.Update();
        if(currentCharges < maxCharges)
        {
            percentCooldownComplete = Mathf.Min(percentCooldownComplete + (Time.deltaTime / cooldown), 1f);
        }
        else
        {
            percentCooldownComplete = 0f;
        }

        if(percentCooldownComplete >= 1f)
        {
            currentCharges += 1;
            percentCooldownComplete = percentCooldownComplete % 1f;
        }
    }
}
