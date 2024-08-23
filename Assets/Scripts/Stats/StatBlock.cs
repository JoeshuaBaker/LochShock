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
    public Item source;

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

    public void AddSource(Item item)
    {
        source = item;
        events.AddSource(item);
        foreach(Stat stat in stats)
        {
            stat.source = item;
        }
    }

    public void Add(StatBlock other)
    {
        this.stats.AddRange(other.stats);
        this.events.Add(other.events);
    }

    public void Remove(StatBlock other)
    {
        this.stats.RemoveRange(other.stats);
        this.events.Remove(other.events);
    }

    public IEnumerable<T> GetEvents<T>() where T : ScriptableAction
    {
        return events.GetEvents<T>();
    }

    public List<T> GetEventsAsList<T>() where T : ScriptableAction
    {
        return events.GetEventsAsList<T>();
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

        newStatBlock.AddSource(source);

        return newStatBlock;
    }

    public IEnumerable<string> GetEventTooltips()
    {
        List<string> eventTooltips = new List<string>();
        eventTooltips.AddRange(events.GetEvents<OnFireAction>().Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));
        eventTooltips.AddRange(events.GetEvents<OnHitAction>().Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));
        eventTooltips.AddRange(events.GetEvents<OnKillAction>().Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));
        eventTooltips.AddRange(events.GetEvents<OnReloadAction>().Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));
        eventTooltips.AddRange(events.GetEvents<OnSecondAction>().Select(x => StatBlockContext.HighlightColor + x.GetLabel() + ": </color>" + x.GetTooltip(Player.activePlayer.Stats)));

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
