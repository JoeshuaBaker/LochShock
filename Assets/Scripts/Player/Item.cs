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
        return StatBlock.GetCombinedStatBlockContext(itemStats.Concat(levelUpStats));
    }

    public IEnumerable<string> GetEventTooltips()
    {
        var debugConcatStats = itemStats.Concat(levelUpStats);
        Debug.Log("Concat count" + debugConcatStats.Count());
        foreach(var stat in debugConcatStats)
        {
            Debug.Log(stat.events.OnHit.Count());
        }
        return StatBlock.GetEventTooltips(itemStats.Concat(levelUpStats));
    }
}