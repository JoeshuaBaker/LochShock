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

    private IEnumerable<StatBlock> _stats = null;
    public IEnumerable<StatBlock> stats
    {
        get
        {
            if (_stats == null)
            {
                _stats = itemStats.Concat(levelUpStats);
            }

            foreach(var levelUpStat in levelUpStats)
            {
                levelUpStat.stacks = level - 1;
            }

            return _stats;
        }
    }
    public NewStatBlock newStats;
    public NewStatBlock newLevelUpStats;
    public StatBlock[] itemStats = new StatBlock[]
    {
        new StatBlock(StatBlock.BlockType.xMult)
    };
    public StatBlock[] levelUpStats = new StatBlock[]
    {
        new StatBlock(StatBlock.BlockType.xMult)
    };

    public StatBlockContext GetStatBlockContext()
    {
        return StatBlock.GetCombinedStatBlockContext(stats);
    }

    public IEnumerable<string> GetEventTooltips()
    {
        return StatBlock.GetEventTooltips(itemStats.Concat(levelUpStats));
    }

    public void LevelUp()
    {
        level++;
        foreach(StatBlock levelStats in levelUpStats)
        {
            levelStats.stacks = level;
        }
    }

    public virtual void Start()
    {
        if(string.IsNullOrEmpty(displayName))
        {
            displayName = name;
        }
    }

    public void TransferStats()
    {
        newStats = new NewStatBlock();
        newStats.SetStatBlock(itemStats);

        newLevelUpStats = new NewStatBlock();
        newLevelUpStats.SetStatBlock(levelUpStats);
    }
}
