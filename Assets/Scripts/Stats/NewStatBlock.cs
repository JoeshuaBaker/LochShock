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

    [SerializeReference, SerializeReferenceMenu] public Stat[] stats;
    public Events events;
    public float testFloat;

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

        stats = fieldStats.ToArray();
    }
}
