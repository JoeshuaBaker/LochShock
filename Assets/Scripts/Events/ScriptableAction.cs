using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class ScriptableAction : ScriptableObject
{
    public AK.Wwise.Event onTriggerEvent;
    public float chanceToTrigger = 1.0f;
    public float cooldown = 0f;
    private double lastTriggerTime = -1;
    public abstract string GetLabel();
    public abstract string GetTooltip(CombinedStatBlock stats);

    public bool IsValidTrigger()
    {
        bool isValidTrigger = 
            UnityEngine.Random.Range(0f, 1f) <= chanceToTrigger && 
            (cooldown == 0 || lastTriggerTime == -1 || Time.timeAsDouble > lastTriggerTime + cooldown);
        if(isValidTrigger)
        {
            lastTriggerTime = Time.timeAsDouble;
        }

        return isValidTrigger;
    }

    public void PlayTriggerSfx(GameObject audioSource)
    {
        if(onTriggerEvent != null)
        {
            onTriggerEvent.Post(audioSource);
        }
    }

    public static string GetBuffTooltip(ScriptableAction action, Buff buff)
    {
        string tooltip = $"Gain {buff.buffName} {(action.chanceToTrigger < 1f ? $"({action.chanceToTrigger.ToString("P0")} chance)" :"")} for {buff.duration} seconds.{Environment.NewLine}";
        StatBlockContext context = buff.newStats.GetStatBlockContext();
        IEnumerable<string> buffContextStrings = context.GetStatContextStrings();
        foreach (var contextString in buffContextStrings)
        {
            tooltip += contextString + Environment.NewLine;
        }

        if(buff.stackLimit > 1)
        {
            tooltip += $"(Stacks to {buff.stackLimit})";
        }

        return tooltip.TrimEnd();

    }
}
