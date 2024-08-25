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
    public abstract string GetTooltip(CombinedStatBlock stats, int level = 1);

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

    public static string GetBuffTooltip(ScriptableAction action, Buff buff, int level = 1)
    {
        string tooltip = $"Gain {buff.buffName} {(action.chanceToTrigger < 1f ? $"({action.chanceToTrigger.ToString("P0")} chance)" :"")} for {buff.baseDuration} seconds.{Environment.NewLine}";
        CombinedStatBlock combinedStats = new CombinedStatBlock();
        var statBlocks = buff.GetStatBlocks(level);
        combinedStats.UpdateSources(statBlocks);
        StatBlockContext context = combinedStats.combinedStatBlock.GetStatBlockContext();
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
