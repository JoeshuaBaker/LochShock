using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //external references
    public Transform gunParent;
    public Transform activeItemParent;
    public Transform itemParent;
    public Transform stashParent;

    //internal data structures
    public Gun activeGun = null;
    public Gun inactiveGun = null;
    public Gun[] guns = new Gun[2];
    public Item[] activeItem = new Item[1];
    public Item[] itemStash = new Item[2];
    public Item[] items = new Item[5];
    private Dictionary<Item.ItemType, Item[]> itemMap;
    private Dictionary<Item[], Transform> collectionParentMap;

    public Item[] allItems
    {
        get
        {
            return new Item[] { guns[0], guns[1], activeItem[0], itemStash[0], itemStash[1], items[0], items[1], items[2], items[3], items[4] };
        }
    }
    public int scrap = 0;
    public int scrapPerOrb = 25;
    public OrbItemPool[] orbItemPools;

    //External References
    public ItemResourcesAtlas itemResourcesAtlas;
    public InventoryUI inventoryUI;

    private void Start()
    {
        itemMap = new Dictionary<Item.ItemType, Item[]>();
        itemMap.Add(Item.ItemType.Item, items);
        itemMap.Add(Item.ItemType.Weapon, guns);
        itemMap.Add(Item.ItemType.Active, activeItem);

        collectionParentMap = new Dictionary<Item[], Transform>();
        collectionParentMap.Add(guns, gunParent);
        collectionParentMap.Add(activeItem, activeItemParent);
        collectionParentMap.Add(items, itemParent);
        collectionParentMap.Add(itemStash, stashParent);

        itemResourcesAtlas.Setup();

        foreach (Item item in allItems)
        {
            if (item != null)
            {
                int index = Contains(item, out Item[] collection);
                collection[index] = null;

                AddItem(item);
            }
        }
    }

    public void OpenCloseInventory()
    {
        if(inventoryUI != null)
        {
            if (inventoryUI.state == InventoryUI.InventoryUIState.Close)
            {
                inventoryUI.TransitionState(InventoryUI.InventoryUIState.Inventory, this);
            }
            else
            {
                inventoryUI.TransitionState(InventoryUI.InventoryUIState.Close, this);
            }
        }
    }

    public bool Orb()
    {
        if (Player.activePlayer.isDead || Player.activePlayer.orbsHeld <= 0 || orbItemPools == null || orbItemPools.Length == 0 || World.activeWorld.paused)
            return false;

        int orbsSpent = Player.activePlayer.orbsHeld > orbItemPools.Length ? orbItemPools.Length : (int)Player.activePlayer.orbsHeld;
        Player.activePlayer.orbsHeld -= orbsSpent;

        Item[] items = orbItemPools[orbsSpent - 1].GetItems(itemResourcesAtlas, allItems);

        scrap += scrapPerOrb * orbsSpent;

        if (inventoryUI != null)
        {
            inventoryUI.TransitionState(InventoryUI.InventoryUIState.Orb, this, items);
            inventoryUI.OnClose = () => { Player.activePlayer.Bomb(false); };
        }
        else
        {
            Player.activePlayer.Bomb(false);
        }

        return true;
    }

    public void SwitchWeapons()
    {
        if (guns[0] == null || guns[1] == null)
            return;

        if(activeGun == guns[0])
        {
            activeGun = guns[1];
            inactiveGun = guns[0];
        }
        else
        {
            activeGun = guns[0];
            inactiveGun = guns[1];
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
            collectionParentMap.TryGetValue(collection, out Transform parent);
            collection[index] = Instantiate(item, parent);
            collection[index].name = item.name;

            if(activeGun == null && item is Gun)
            {
                activeGun = collection[index] as Gun;
            }
            else if(inactiveGun == null && item is Gun)
            {
                inactiveGun = collection[index] as Gun;
            }
        }

        return index > -1;
    }

    private bool DisassembleInventoryItem(Item item)
    {
        int index = Contains(item, out Item[] collection);

        if(index > -1)
        {
            scrap += item.disassembleValue;
            Destroy(collection[index].gameObject);
            collection[index] = null;
        }

        return index > -1;
    }

    public int UnstashItem(Item item)
    {
        int index = Contains(item, out Item[] collection);
        int spaceIndex = FirstEmptySpace(item.itemType, out Item[] emptyCollection);

        if (collection == itemStash && collection != emptyCollection)
        {
            emptyCollection[spaceIndex] = collection[index];
            collection[index] = null;
            return spaceIndex;
        }

        return -1;
    }

    public void DisassembleItem(Item item)
    {
        if(Contains(item))
        {
            DisassembleInventoryItem(item);
        }
        else
        {
            scrap += item.disassembleValue;
        }
    }

    public void LevelUp(Item item)
    {
        if(scrap >= item.levelUpCost)
        {
            scrap -= item.levelUpCost;
            item.LevelUp();
        }
    }

    public bool HasNonStashSpaceFor(Item item)
    {
        return FirstEmptySpace(item.itemType, out Item[] collection) > -1 && collection != itemStash;
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

        index = IndexOf(itemStash, item);

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
