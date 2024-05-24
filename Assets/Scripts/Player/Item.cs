using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Sprite icon;
    public Rarity rarity;
    public int level = 1;
    public int levelUpCost;
    public int baseDisassembleValue;
    public float disassembleRefundRatio = 0.7f;
    public int disassembleValue
    {
        get
        {
            return (baseDisassembleValue == 0 ? (int)rarity*50 : baseDisassembleValue) + (int)(levelUpCost * (level - 1) * disassembleRefundRatio);
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
}
