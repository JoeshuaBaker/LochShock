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

    private List<StatBlock> statBlocks = new List<StatBlock>();
    private ActiveItem source;
    private CombinedStatBlock stats;
    private float duration;
    private float repeats;
    private float delayTimer;

    public override void Activate()
    {
        Vector2 mouseDirection = Player.activePlayer.lookDirection;
        gun.shooting = true;
        gun.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(mouseDirection.x, mouseDirection.y, 0f)).eulerAngles;

        statBlocks.Clear();

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
        gun.combinedStats.UpdateSources(statBlocks);

        repeats = (int)(duration/durationToRepeatFireRatio);
        gun.ShootIgnoreState(gun.combinedStats);

        if(repeats > 0)
        {
            delayTimer = repeatDelay;
        }
    }

    public override void ApplyStatBlock(CombinedStatBlock stats)
    {
        this.stats = stats;
        duration = stats.GetCombinedStatValue<ActiveItemDuration>();
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

    public override StatBlockContext GetStatBlockContext(StatBlockContext baseContext)
    {
        StatBlockContext statBlockContext = new StatBlockContext();
        statBlockContext.AddGenericTooltip($"Fires a {StatBlockContext.GoodColor}{gun.name}</color>. Cooldown: {StatBlockContext.HighlightColor}{source.cooldown}</color>s.");
        return statBlockContext;
    }

    private void Update()
    {
        if(repeats > 0)
        {
            if(delayTimer > 0f)
            {
                delayTimer -= Time.deltaTime;
            }

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
