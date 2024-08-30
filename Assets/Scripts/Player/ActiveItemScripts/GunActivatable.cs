using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunActivatable : Activatable
{
    public Gun gun;
    public bool useItemStats = true;
    public bool convertActiveDamageToGunDamage = true;
    public float durationToRepeatFireRatio = 5.0f;
    public float repeatDelay = 0.4f;
    public int baseRepeats = 0;

    private List<StatBlock> statBlocks = new List<StatBlock>();
    private ActiveItem source;
    private CombinedStatBlock gunCombinedStats;
    private float duration;
    private float repeats;
    private float delayTimer;
    private bool setup = false;
    private HashSet<Type> excludeStatsFromTooltip = new HashSet<Type> { typeof(MagazineSize), typeof(ReloadSpeed), typeof(FireSpeed), typeof(Size), typeof(Velocity), typeof(Accuracy), typeof(ActiveItemDamage)};

    public override bool Activate()
    {
        if(delayTimer > 0f)
        {
            return false;
        }

        Vector2 mouseDirection = Player.activePlayer.lookDirection;
        gun.shooting = true;
        gun.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(mouseDirection.x, mouseDirection.y, 0f)).eulerAngles;

        repeats = baseRepeats + (int)(duration/durationToRepeatFireRatio);
        gun.ShootIgnoreState(gun.combinedStats);

        if(repeats > 0)
        {
            delayTimer = repeatDelay;
        }
        return true;
    }

    public override void ApplyStatBlock(CombinedStatBlock stats)
    {
        duration = stats.GetCombinedStatValue<ActiveItemDuration>() - 1f;

        if (!setup)
        {
            setup = true;
            gunCombinedStats = new CombinedStatBlock();
        }
        statBlocks.Clear();

        if (useItemStats)
        {
            //get item stats, excluding the weapons so we don't get a bunch of gun base stats we don't want
            if (useItemStats)
            {
                foreach (Item item in Player.activePlayer.inventory.items)
                {
                    if (item != null)
                    {
                        statBlocks.AddRange(item.newStatsList);
                    }
                }
            }
        }

        if (convertActiveDamageToGunDamage)
        {
            StatBlock gunDamageBlock = new StatBlock();
            foreach (Stat stat in stats.combinedStatBlock.stats)
            {
                if (stat is ActiveItemDamage)
                {
                    Damage gunDamage = new Damage();
                    gunDamage.combineType = stat.combineType;
                    gunDamage.value = stat.value;
                    gunDamage.conditions = stat.conditions;
                    gunDamage.stacks = stat.stacks;
                    gunDamage.source = source;
                    gunDamageBlock.stats.Add(gunDamage);
                }
            }

            statBlocks.Add(gunDamageBlock);
        }

        statBlocks.AddRange(gun.newStatsList);
        gunCombinedStats.UpdateSources(statBlocks);
        gun.ApplyNewStatBlock(gunCombinedStats);
    }

    public override void Setup(ActiveItem source)
    {
        this.source = source;
        gun = Instantiate(gun, this.transform);
        gun.name = gun.name.Replace("(Clone)", "");
        statBlocks = new List<StatBlock>();
    }

    public override void OnLevelUp()
    {
        gun.LevelUp();
    }

    public override StatBlockContext GetStatBlockContext(CombinedStatBlock baseContext, ActiveItem source)
    {
        if(statBlocks == null)
        {
            statBlocks = new List<StatBlock>();
        }
        else
        {
            statBlocks.Clear();
        }

        CombinedStatBlock csb = new CombinedStatBlock();
        statBlocks.AddRange(baseContext.sourcesList);
        statBlocks.AddRange(gun.newStatsList);
        csb.UpdateSources(statBlocks);
        StatBlockContext context = csb.GetCombinedContext();
        foreach(StatBlock statBlock in statBlocks)
        {
            foreach(Stat stat in statBlock.stats)
            {
                if (excludeStatsFromTooltip.Contains(stat.GetType()))
                {
                    context.RemoveAllMatchingStatContext(stat);
                }
            }
        }
        context.AddGenericPrefixTooltip($"Fires a {gun.DisplayName.AddColorToString(StatBlockContext.GoodColor)}.");
        context.AddGenericPostfixTooltip($"Repeats attack for every {durationToRepeatFireRatio * 100f}% active item duration.");
        return context;
    }

    private void Update()
    {
        if (delayTimer > 0f)
        {
            delayTimer = Mathf.Max(delayTimer - Time.deltaTime, 0);
        }

        if (repeats > 0)
        {
            if (delayTimer <= 0)
            {
                gun.ShootIgnoreState(gun.combinedStats);
                repeats--;
                delayTimer += repeatDelay;
            }
        }
        else
        {
            delayTimer = 0f;
        }
    }
}
