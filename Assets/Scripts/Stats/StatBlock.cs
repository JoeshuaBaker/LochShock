using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class StatBlock
{
    [FormerlySerializedAs("stats")] [SerializeReference, SerializeReferenceMenu] public List<Stat> stats;
    [FormerlySerializedAs("events")] [SerializeField] public Events events;
    [SerializeField]
    private float stacks = 1f;

    public float Stacks
    {
        get { return stacks; }
        set
        {
            foreach(Stat stat in stats)
            {
                stat.stacks = value;
            }
            stacks = value;
        }
    }
    public bool active = false;

    public StatBlock()
    {
        if(stats == null)
        {
            stats = new List<Stat>();
        }

        if(events == null)
        {
            events = new Events();
        }
    }

    public IEnumerable<T> GetStats<T>() where T : Stat
    {
        return stats.Where(x => x is T) as IEnumerable<T>;
    }

    public T GetStat<T>() where T : Stat
    {
        return stats.FirstOrDefault(x => x is T) as T;
    }

    public float GetStatValue<T>() where T : Stat
    {
        return stats.FirstOrDefault(x => x is T)?.value ?? 0;
    }

    public Stat GetStat(Type type)
    {
        if(type != null)
        {
            return stats.FirstOrDefault(x => x != null && x.GetType() == type);
        }

        return null;
    }

    public IEnumerable<T> GetStatsByType<T, S>() where T : Stat where S : StatCombineType
    {
        return stats.Where(x => x is T && x.combineType is S) as IEnumerable<T>;
    }

    public T GetStat<T, S>() where T : Stat where S : StatCombineType
    {
        return stats.FirstOrDefault(x => x is T && x.combineType is S) as T;
    }

    public StatBlock DeepCopy()
    {
        StatBlock newStatBlock = new StatBlock();
        foreach(Stat stat in this.stats)
        {
            Type statType = stat.GetType();
            Type combineType = stat.combineType.GetType();
            Stat statCopy = Activator.CreateInstance(statType) as Stat;
            statCopy.combineType = Activator.CreateInstance(combineType) as StatCombineType;
            statCopy.value = stat.value;
            newStatBlock.stats.Add(statCopy);
        }

        newStatBlock.Stacks = this.Stacks;
        newStatBlock.events = this.events.Copy();

        return newStatBlock;
    }

    public IEnumerable<string> GetEventTooltips()
    {
        List<string> eventTooltips = new List<string>();
        eventTooltips.AddRange(events.OnFire.Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));
        eventTooltips.AddRange(events.OnHit.Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));
        eventTooltips.AddRange(events.OnKill.Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));
        eventTooltips.AddRange(events.OnReload.Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));
        eventTooltips.AddRange(events.OnSecond.Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));

        return eventTooltips;
    }

    public void UpdateStatBlockContext(ref StatBlockContext statBlockContext)
    {
        foreach(Stat stat in stats)
        {
            stat.UpdateStatBlockContext(ref statBlockContext);
        }
    }

    public StatBlockContext GetStatBlockContext()
    {
        StatBlockContext statBlockContext = new StatBlockContext();
        UpdateStatBlockContext(ref statBlockContext);

        return statBlockContext;
    }
}