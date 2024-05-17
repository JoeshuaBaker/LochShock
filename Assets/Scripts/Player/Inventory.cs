using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> Items;

    public List<StatBlock> GetItemStats()
    {
        List<StatBlock> statBlocks = new List<StatBlock>();
        foreach(Item item in Items)
        {
            if(item != null)
            {
                statBlocks.AddRange(item.itemStats);
            }
        }

        return statBlocks;
    }
}
