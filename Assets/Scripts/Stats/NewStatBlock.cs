using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NewStatBlock
{
    public class Events
    {
        public List<OnFireAction> OnFire = new List<OnFireAction>();
        public List<OnHitAction> OnHit = new List<OnHitAction>();
        public List<OnKillAction> OnKill = new List<OnKillAction>();
        public List<OnReloadAction> OnReload = new List<OnReloadAction>();
        public List<OnSecondAction> OnSecond = new List<OnSecondAction>();

        public Events()
        {

        }

        public void AddEvents(StatBlock.Events events)
        {
            if (events.OnFire != null) OnFire.AddRange(events.OnFire);
            if (events.OnHit != null) OnHit.AddRange(events.OnHit);
            if (events.OnKill != null) OnKill.AddRange(events.OnKill);
            if (events.OnReload != null) OnReload.AddRange(events.OnReload);
            if (events.OnSecond != null) OnSecond.AddRange(events.OnSecond);
        }
    }

    [SerializeReference, SerializeReferenceMenu] public List<Stat> stats = new List<Stat>();
    public Events events;
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

    public void SetStatBlock(IEnumerable<StatBlock> statBlocks)
    {
        List<Stat> fieldStats = new List<Stat>();
        events = new Events();

        var playerStatFields = typeof(StatBlock.PlayerStats).GetFields()
                            .Select(field => field.Name)
                            .ToList();

        playerStatFields.Remove("visionConeAngle");
        playerStatFields.Remove("visionConeRadius");
        playerStatFields.Remove("visionProximityRadius");

        var gunStatFields = typeof(StatBlock.GunStats).GetFields()
                            .Select(field => field.Name)
                            .ToList();

        foreach (var block in statBlocks)
        {
            foreach(var field in playerStatFields)
            {
                float value = (float) block.playerStats.GetType().GetField(field).GetValue(block.playerStats);
                if(value != 0)
                {
                    fieldStats.Add(Stat.CreateStatByString(field, value, block.blockType));
                }
            }

            foreach (var field in gunStatFields)
            {
                float value = (float)block.gunStats.GetType().GetField(field).GetValue(block.gunStats);
                if (value != 0)
                {
                    fieldStats.Add(Stat.CreateStatByString(field, value, block.blockType));
                }
            }

            events.AddEvents(block.events);
        }

        stats = fieldStats.ToList();
    }

    public IEnumerable<T> GetStats<T>() where T : Stat
    {
        return stats.Where(x => x is T) as IEnumerable<T>;
    }

    public T GetFirstStat<T>() where T : Stat
    {
        return stats.FirstOrDefault(x => x is T) as T;
    }

    public Stat GetFirstStat(Type type)
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

    public T GetFirstStatByType<T, S>() where T : Stat where S : StatCombineType
    {
        return stats.FirstOrDefault(x => x is T && x.combineType is S) as T;
    }
}
