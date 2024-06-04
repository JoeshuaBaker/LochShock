using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Gun activeGun = null;
    public Gun[] guns = new Gun[2];
    public Item[] activeItem = new Item[1];
    public Item[] itemStash = new Item[2];
    public Item[] items = new Item[5];
    private Dictionary<Item.ItemType, Item[]> itemMap;
    public int scrap = 0;
    public List<Item> OrbItemPrefabs;
    public List<Item> TwoOrbItemPrefabs;
    public InventoryUI inventoryUI;

    private void Start()
    {
        activeGun = guns[0];
        itemMap = new Dictionary<Item.ItemType, Item[]>();
        itemMap.Add(Item.ItemType.Item, items);
        itemMap.Add(Item.ItemType.Weapon, guns);
        itemMap.Add(Item.ItemType.Active, activeItem);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Player.activePlayer.orbsHeld > 0 && OrbItemPrefabs != null && OrbItemPrefabs.Count > 0)
        {
            for(int i = 0; i < Player.activePlayer.orbsHeld; i++)
            {
                //ListOrbItemPrefabs[Random.Range(0, OrbItemPrefabs.Count)]);
            }
            
            if(inventoryUI != null)
            {
                inventoryUI.TransitionState(InventoryUI.InventoryUIState.Inventory);
            }

            Player.activePlayer.orbsHeld = 0;
        }
    }

    public List<StatBlock> GetItemStats()
    {
        List<StatBlock> statBlocks = new List<StatBlock>();
        statBlocks.AddRange(activeGun.stats);
        foreach (Item item in items)
        {
            if (item != null)
            {
                statBlocks.AddRange(item.stats);
            }
        }

        return statBlocks;
    }

    public bool AddItem(Item item)
    {
        int index = FirstEmptySpace(item.itemType, out Item[] collection);
        if(index > -1)
        {
            collection[index] = item;
        }

        return index > -1;
    }

    public bool DisassembleItem(Item item)
    {
        int index = Contains(item, out Item[] collection);

        if(index > -1)
        {
            scrap += item.disassembleValue;
            collection[index] = null;
        }

        return index > -1;
    }

    public bool HasSpaceFor(Item item)
    {
        return FirstEmptySpace(item.itemType, out Item[] collection) > -1;
    }

    public bool Contains(Item item)
    {
        return Contains(item, out Item[] itemCollection) > -1;
    }

    private int Contains(Item item, out Item[] collection)
    {
        int index = -1;
        if (itemMap.TryGetValue(item.itemType, out Item[] typeCollection))
        {
            index = IndexOf(typeCollection, item);
            if (index > -1)
            {
                collection = typeCollection;
                return index;
            }
        }

        index = IndexOf(itemStash, null);

        if (index > -1)
        {
            collection = itemStash;
            return index;
        }

        collection = null;
        return index;
    }

    private int FirstEmptySpace(Item.ItemType itemType, out Item[] collection)
    {
        int index = -1;
        if (itemMap.TryGetValue(itemType, out Item[] typeCollection))
        {
            index = IndexOf(typeCollection, null);
            if (index > -1)
            {
                collection = typeCollection;
                return index;
            }
        }

        index = IndexOf(itemStash, null);

        if (index > -1)
        {
            collection = itemStash;
            return index;
        }

        collection = null;
        return index;
    }

    private int IndexOf(Item[] collection, Item item)
    {
        if (collection == null)
            return -1;

        for (int i = 0; i < collection.Length; i++)
        {
            if (collection[i] == item)
            {
                return i;
            }
        }

        return -1;
    }
}
