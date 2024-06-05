using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
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

    public Item[] GetItems(ItemResourcesAtlas atlas, Item[] ignoreList = null)
    {
        Item[] drops = new Item[numDrops];
        HashSet<string> ignoreListNames = new HashSet<string>();
        foreach(Item item in ignoreList)
        {
            if(item != null)
            {
                ignoreListNames.Add(item.name);
            }
        }
        
        for(int i = 0; i < numDrops; i++)
        {
            Item.Rarity rarity = Item.Rarity.Common;
            Item.ItemType type = Item.ItemType.Item;
            int rarityRoll = Random.Range(0, 100);
            float itemRoll = Random.Range(0f, 1f);
            int cumulativeRarity = 0;
            float cumulativeType = 0;

            if(rarityRoll < (cumulativeRarity += legendaryChance))
            {
                rarity = Item.Rarity.Legendary;
            }
            else if(rarityRoll < (cumulativeRarity += epicChance))
            {
                rarity = Item.Rarity.Epic;
            }
            else if(rarityRoll < (cumulativeRarity += rareChance))
            {
                rarity = Item.Rarity.Rare;
            }
            else if(rarityRoll < (cumulativeRarity += uncommonChance))
            {
                rarity = Item.Rarity.Uncommon;
            }

            if(itemRoll < (cumulativeType += weaponChance))
            {
                type = Item.ItemType.Weapon;
            }
            else if(itemRoll < (cumulativeType += activeItemChance))
            {
                type = Item.ItemType.Active;
            }

            Item[] list = atlas.GetItemList(type, rarity).Where(x => !drops.Contains(x) && !ignoreListNames.Contains(x.name)).ToArray();
            if (list == null || list.Length == 0)
            {
                i--;
                continue;
            }

            drops[i] = list[Random.Range(0, list.Length)];
        }

        return drops;
    }
}
