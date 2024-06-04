using UnityEngine;

public class OrbItemPool
{
    //global ratios for type of item drop
    public static float weaponChance = 0.3f;
    public static float activeItemChance = 0.2f;
    public static float itemChance = 0.5f;

    //ratios for rarity specific to this item pool
    public int numOrbsSpent = 1;
    [Range(1, 10)] public int numDrops = 3;
    [Range(0, 100)] public int legendaryChance = 0;
    [Range(0, 100)] public int epicChance = 0;
    [Range(0, 100)] public int rareChance = 0;
    [Range(0, 100)] public int uncommonChance = 0;

    public Item[] GetItems()
    {
        Item[] drops = new Item[numDrops];
        
        for(int i = 0; i < numDrops; i++)
        {

        }

        return drops;
    }
}
