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
    public ActiveItem activeItem = null;
    public Gun[] guns = new Gun[2];
    public Item[] activeItems = new Item[1];
    public Item[] itemStash = new Item[2];
    public Item[] items = new Item[5];
    private Dictionary<Item.ItemType, Item[]> itemMap;
    private Dictionary<Item[], Transform> collectionParentMap;

    public Item[] allItems
    {
        get
        {
            return new Item[] { guns[0], guns[1], activeItems[0], itemStash[0], itemStash[1], items[0], items[1], items[2], items[3], items[4] };
        }
    }
    public int scrap = 0;
    public int scrapPerOrb = 25;
    public OrbItemPool[] orbItemPools;

    //External References
    public ItemResourcesAtlas itemResourcesAtlas;
    public InventoryUI inventoryUI;
    public GameplayUI gameplayUI;

    public void Setup()
    {
        itemMap = new Dictionary<Item.ItemType, Item[]>();
        itemMap.Add(Item.ItemType.Item, items);
        itemMap.Add(Item.ItemType.Weapon, guns);
        itemMap.Add(Item.ItemType.Active, activeItems);

        collectionParentMap = new Dictionary<Item[], Transform>();
        collectionParentMap.Add(guns, gunParent);
        collectionParentMap.Add(activeItems, activeItemParent);
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

    void Update()
    {

        if (gameplayUI == null)
        {
            Debug.Log("Inventory needs reference to GameplayUI");
        }
        gameplayUI.items = allItems;
        gameplayUI.activeGun = activeGun;
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

    public bool Orb(bool isSmall = false)
    {
        if (Player.activePlayer.isDead || Player.activePlayer.orbsHeld <= 0 || orbItemPools == null || orbItemPools.Length == 0 || World.activeWorld.paused)
            return false;

        if (!isSmall)
        {
            int orbsSpent = Player.activePlayer.orbsHeld > orbItemPools.Length ? orbItemPools.Length : (int)Player.activePlayer.orbsHeld;
            Player.activePlayer.orbsHeld -= orbsSpent;

            Item[] items = orbItemPools[orbsSpent - 1].GetItems(itemResourcesAtlas, allItems);

            scrap += scrapPerOrb;

            if (inventoryUI != null)
            {
                inventoryUI.TransitionState(InventoryUI.InventoryUIState.Orb, this, items);
                inventoryUI.OnClose = () => { Player.activePlayer.Bomb(false, true); };
            }
            else
            {
                Player.activePlayer.Bomb(false,true);
            }

            return true;
        }
        else
        {
            Item[] items = orbItemPools[0].GetItems(itemResourcesAtlas, allItems);

            if (inventoryUI != null)
            {
                inventoryUI.TransitionState(InventoryUI.InventoryUIState.Orb, this, items);
                inventoryUI.OnClose = () => { Player.activePlayer.Bomb(false,true); };
            }
            else
            {
                Player.activePlayer.Bomb(false,true);
            }

            return true;

        }

    }

    public void ActivateActiveItem()
    {
        if(activeItem != null)
        {
            activeItem.Activate();
        }
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

        activeGun.shooting = inactiveGun.shooting;
        inactiveGun.shooting = false;
        inactiveGun.CancelReload();
    }

    public List<StatBlock> GetNewItemStats()
    {
        List<StatBlock> newStatBlocks = new List<StatBlock>();
        newStatBlocks.AddRange(activeGun.newStatsList);
        if(activeItem != null)
        {
            newStatBlocks.AddRange(activeItem.newStatsList);
        }
        foreach (Item item in items)
        {
            if(item != null)
            {
                newStatBlocks.AddRange(item.newStatsList);
            }
        }

        return newStatBlocks;
    }

    public bool AddItem(Item item)
    {
        int index = FirstEmptySpace(item.itemType, out Item[] collection);
        if(index > -1)
        {
            collectionParentMap.TryGetValue(collection, out Transform parent);
            collection[index] = Instantiate(item, parent);
            collection[index].name = item.DisplayName;

            if(activeGun == null && item is Gun)
            {
                activeGun = collection[index] as Gun;
            }
            else if(inactiveGun == null && item is Gun)
            {
                inactiveGun = collection[index] as Gun;
            }

            if(activeItem == null && item is ActiveItem)
            {
                activeItem = collection[index] as ActiveItem;
                activeItem.Setup();
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

            if(emptyCollection[spaceIndex] is ActiveItem)
            {
                activeItem = emptyCollection[spaceIndex] as ActiveItem;
                activeItem.Setup();
            }
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
