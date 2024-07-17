using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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

    public string displayName;
    public Sprite icon;
    public Rarity rarity;
    public ItemType itemType;
    public int level = 1;
    public int baseLevelUpCost;
    public float levelUpCostScalar = 1.25f;
    public int baseDisassembleValue;
    public float disassembleRefundRatio = 0.7f;
    public int disassembleValue
    {
        get
        {
            return (baseDisassembleValue == 0 ? 50 + (int)rarity * 50 : baseDisassembleValue) + (int)(baseLevelUpCost * (level - 1) * disassembleRefundRatio);
        }
    }

    public int levelUpCost
    {
        get
        {
            return (int)(baseLevelUpCost * Mathf.Pow(levelUpCostScalar, level - 1));
        }
    }

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

    public StatBlockContext GetStatBlockContext()
    {
        combinedStats.UpdateSources(newStatsList);
        return combinedStats.GetCombinedContext();
    }

    public IEnumerable<string> GetEventTooltips()
    {
        return stats.GetEventTooltips();
    }

    public void LevelUp()
    {
        level++;
        levelUpStats.Stacks = level - 1;
        combinedStats.UpdateSources(newStatsList);
    }

    public virtual void Start()
    {
        if(string.IsNullOrEmpty(displayName))
        {
            displayName = name;
        }

        combinedStats = new CombinedStatBlock();
        combinedStats.UpdateSources(newStatsList);
    }
}
