using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> Items;
    public List<Item> OrbItemPrefabs;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && Player.activePlayer.orbsHeld > 0 && OrbItemPrefabs != null && OrbItemPrefabs.Count > 0)
        {
            Items.Add(OrbItemPrefabs[Random.Range(0, OrbItemPrefabs.Count)]);
            Player.activePlayer.orbsHeld -= 1;
        }
    }

    public List<StatBlock> GetItemStats()
    {
        List<StatBlock> statBlocks = new List<StatBlock>();
        foreach(Item item in Items)
        {
            if(item != null)
            {
                statBlocks.AddRange(item.stats);
            }
        }

        return statBlocks;
    }
}
