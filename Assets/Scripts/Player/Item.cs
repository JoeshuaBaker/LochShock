using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Item : MonoBehaviour
{
    public enum Rarity
    {
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Epic = 4,
        Legendary = 5
    }

    public enum ItemType
    {
        Item,
        Weapon,
        Active
    }

    [SerializeField] private string displayName;
    public string DisplayName
    {
        get
        {
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = name;
            }
            return displayName;
        }
    }
    public Sprite icon;
    public Rarity rarity;
    public ItemType itemType;
    public DamageType damageType = DamageType.None;
    public int level = 1;
    public int baseLevelUpCost = 100;
    public float levelUpCostScalar = 1.25f;
    public int baseDisassembleValue;
    public float disassembleRefundRatio = 0.7f;
    public int disassembleValue
    {
        get
        {
            float disassembleMult = Player.activePlayer.combinedNewStats.GetCombinedStatValue<DisassembleMult>();
            var actualDisassembleValue = (baseDisassembleValue == 0 ? 50 + (int)rarity * 50 : baseDisassembleValue);
            return (int)((actualDisassembleValue + (int)(actualDisassembleValue * (level - 1) * disassembleRefundRatio))
                * (disassembleMult == 0 ? 1f : disassembleMult));
        }
    }

    public int disassembleKitValue
    {
        get
        {
            if(level == 1)
            {
                return 0;//(int)rarity * 100; this is for scrap
            }
            else
            {
                return (int)(rarity) * (int) Mathf.Pow(2 , (level - 2));
            }
        }
    }

    public int levelUpCost
    {
        get
        {
            bool hasLevelUpStats = levelUpStats.stats.Count > 0;
            if(!hasLevelUpStats)
            {
                return 0;
            }

            return (int)(baseLevelUpCost + ((level > 1) ? (baseLevelUpCost * Mathf.Pow(levelUpCostScalar, level - 1)) : 0f));
        }
    }

    public int levelUpKitCost
    {
        get
        {
            return (int)(rarity + 1) * (int)Mathf.Pow(2 , level-1);  
        }

    }

    public bool lockCombine = false;

    private IEnumerable<StatBlock> _newStats = null;
    public IEnumerable<StatBlock> newStatsList
    {
        get
        {
            if (_newStats == null)
            {
                var newStatList = new List<StatBlock>();
                newStatList.Add(stats);
                newStatList.Add(levelUpStats);
                _newStats = newStatList;
            }

            levelUpStats.Stacks = level - 1;

            return _newStats;
        }
    }

    public StatBlock stats;
    public StatBlock levelUpStats;
    public CombinedStatBlock combinedStats;
    public CombinedStatBlock baseItemCombinedStats;
    public List<Buff.Instance> buffs;

    public virtual StatBlockContext GetStatBlockContext()
    {
        foreach(StatBlock statblock in newStatsList)
        {
            statblock.AddSource(this);
        }
        baseItemCombinedStats.UpdateSources(newStatsList);
        return baseItemCombinedStats.GetCombinedContext();
    }

    public IEnumerable<string> GetEventTooltips()
    {
        return stats.GetEventTooltips(level);
    }

    public virtual void LevelUp()
    {
        level++;
        levelUpStats.Stacks = level - 1;
        baseItemCombinedStats.UpdateSources(newStatsList);
    }

    public virtual void Start()
    {
        buffs = new List<Buff.Instance>();
        baseItemCombinedStats = new CombinedStatBlock();
        baseItemCombinedStats.UpdateSources(newStatsList);
        stats.AddSource(this);
        levelUpStats.AddSource(this);
    }

    public virtual void Update()
    {
        foreach (var buff in buffs)
        {
            buff.currentDuration -= Time.deltaTime;
        }

        buffs.RemoveAll(buff => buff.currentDuration <= 0f);
    }

    public virtual void AddBuff(Buff.Instance buffInstance)
    {
        IEnumerable<Buff.Instance> matchingBuffs = buffs.Where(x => x.buff.buffName.Equals(buffInstance.buff.buffName, StringComparison.CurrentCultureIgnoreCase));

        if (buffInstance.buff.stackType == Buff.StackType.Stackable && matchingBuffs.Count() > 0)
        {
            Buff.Instance matchingBuff = matchingBuffs.First();
            matchingBuff.currentDuration = matchingBuff.buff.baseDuration;
            if (matchingBuff.newStats.Stacks < matchingBuff.buff.stackLimit)
            {
                matchingBuff.newStats.Stacks += 1;
            }

            return;
        }

        //replace lowest duration buff if the buff is copyable and we are at stack limit
        else if (buffInstance.buff.stackType == Buff.StackType.Copyable && matchingBuffs.Count() >= buffInstance.buff.stackLimit)
        {
            Buff.Instance lowestDuration = matchingBuffs.First();

            foreach (var matchingBuff in matchingBuffs)
            {
                if (matchingBuff.currentDuration <= lowestDuration.currentDuration)
                {
                    lowestDuration = matchingBuff;
                }
            }

            buffs.Remove(lowestDuration);
        }

        buffs.Add(buffInstance);
    }

    public void RemoveBuff(Buff buff)
    {
        buffs.RemoveAll(x => x.buff.buffName.Equals(buff.buffName, StringComparison.CurrentCultureIgnoreCase));
    }

    public void RemoveStacksFromBuff(int stacks)
    {
        foreach(var buff in buffs)
        {
            buff.newStats.Stacks -= stacks;
        }

        buffs.RemoveAll(x => x.buff.newStats.Stacks <= 0);
    }

    public void ClearBuffs()
    {
        buffs.Clear();
    }
}
