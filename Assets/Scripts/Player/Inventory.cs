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
    public Item[] itemStash = new Item[8];
    public Item[] items = new Item[5];
    private Dictionary<Item.ItemType, Item[]> itemMap;
    private Dictionary<Item[], Transform> collectionParentMap;
    private Item[] allItemsInternal;

    public Item[] allItems
    {
        get
        {
            if(allItemsInternal == null)
            {
                allItemsInternal = new Item[16];
            }
            allItemsInternal[0] = guns[0];
            allItemsInternal[1] = guns[1];
            allItemsInternal[2] = activeItems[0];
            allItemsInternal[3] = items[0];
            allItemsInternal[4] = items[1];
            allItemsInternal[5] = items[2];
            allItemsInternal[6] = items[3];
            allItemsInternal[7] = items[4];
            allItemsInternal[8] = itemStash[0];
            allItemsInternal[9] = itemStash[1];
            allItemsInternal[10] = itemStash[2];
            allItemsInternal[11] = itemStash[3];
            allItemsInternal[12] = itemStash[4];
            allItemsInternal[13] = itemStash[5];
            allItemsInternal[14] = itemStash[6];
            allItemsInternal[15] = itemStash[7];

            return allItemsInternal;
        }
    }
    public float orbValueIntrest = 0.1f;
    public int scrap = 0;
    public int upgradeKits = 0;
    public int scrapPerOrb = 25;
    public int scrapBonusMaxOrb = 100;
    public int currentItemPool;
    public OrbItemPool currentPool;
    public OrbItemPool[] orbItemPools;

    //External References
    public ItemResourcesAtlas itemResourcesAtlas;
    public InventoryUI inventoryUI;
    public GameplayUI gameplayUI;
    public InventoryUpgradeUi invUpgradeUi;

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
        if (invUpgradeUi != null)
        {
            invUpgradeUi.InteractInventory();
        }
    }

    public bool Orb(bool reroll = false, int rerollCost = 0, Item[] rerolledItems = null)
    {

        if (Player.activePlayer.isDead || orbItemPools == null || orbItemPools.Length == 0)
            return false;

        if (World.activeWorld.paused && !reroll)
            return false;



        if (reroll)
        {
            Item[] items = orbItemPools[currentItemPool].GetItems(itemResourcesAtlas, rerolledItems, true);

            scrap -= rerollCost;
            invUpgradeUi.upgradeUi.SetUpgradeItems(items, 0, true);
            return true;
        }
        else
        {
            currentItemPool = Mathf.Min((orbItemPools.Length - 1), (int)Player.activePlayer.Stats.GetCombinedStatValue<ShopLevel>());
            currentPool = orbItemPools[currentItemPool];

            var orbNonBaseValue = (Player.activePlayer.Stats.GetCombinedStatValue<OrbValue>() - Player.activePlayer.baseStats.GetStatValue<OrbValue>()) * Player.activePlayer.orbsHeld;
            int orbIntrest = (int)Mathf.Ceil(orbNonBaseValue * orbValueIntrest);
            Player.activePlayer.interestStats.Stacks += orbIntrest;

            int gainedScrap = (int)(Player.activePlayer.orbsHeld * Player.activePlayer.Stats.GetCombinedStatValue<OrbValue>());
            scrap += gainedScrap;
            Player.activePlayer.orbsHeld = 0;

            Player.activePlayer.gameplayUI.SetOrbs(0, 1);

            Item[] items = orbItemPools[currentItemPool].GetItems(itemResourcesAtlas);

            invUpgradeUi.EnterUpgrade(items,gainedScrap);
            invUpgradeUi.OnClose = () => 
            {
                Player.activePlayer.Bomb(false, true);
                Player.activePlayer.rarityStats.Stacks += 1;
            };

            return true;

        }
    }

    public bool ActivateActiveItem()
    {
        if(activeItem != null)
        {
            return activeItem.Activate();
        }

        return false;
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
        activeGun.transform.SetAsFirstSibling();
        inactiveGun.CancelReload();
    }

    public void AggregateBuffs(List<Buff.Instance> buffList)
    {
        buffList.AddRange(activeGun.buffs);
        if(activeItem != null)
        {
            buffList.AddRange(activeItem.buffs);
        }

        foreach(Item item in items)
        {
            if(item != null)
            {
                buffList.AddRange(item.buffs);
            }
        }
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
    
    //depreciated garbage that im not sure new functionality will be derived from
    public int UnstashItem(Item item)
    {
        int index = Contains(item, out Item[] collection);
        int spaceIndex = FirstEmptySpace(item.itemType, out Item[] emptyCollection);

        if (spaceIndex != -1 && collection == itemStash && collection != emptyCollection)
        {
            emptyCollection[spaceIndex] = collection[index];
            collection[index] = null;
            collectionParentMap.TryGetValue(emptyCollection, out Transform parent);
            if(parent != null)
            {
                emptyCollection[spaceIndex].transform.parent = parent;
            }

            if(emptyCollection[spaceIndex] is ActiveItem)
            {
                activeItem = emptyCollection[spaceIndex] as ActiveItem;
                activeItem.Setup();
            }
            return spaceIndex;
        }

        return -1;
    }

    public int DisassembleItem(Item item, bool gainNoResources = false)
    {
        int indexReturn = IndexOf(allItems, item, true);

        int index = Contains(item, out Item[] collection);

        if (index > -1)
        {
            if (!gainNoResources)
            {
                upgradeKits += item.disassembleKitValue;
                scrap += item.disassembleValue;
            }
            Destroy(collection[index].gameObject);
            collection[index] = null;
        }

        return indexReturn;
    }

    public int LevelUp(Item item, bool free = false)
    {
        int upgradeIndex = IndexOf(allItems, item, true);

        if(scrap >= item.levelUpCost && !free) //&& upgradeKits >= item.levelUpKitCost && !free)
        {
            upgradeKits -= item.levelUpKitCost;
            scrap -= item.levelUpCost;
            item.LevelUp();
        }
        else if(free)
        {
            item.LevelUp();
        }

        return upgradeIndex;
    }

    public bool TryCombineItem(Item item)
    {
        //checks an item to se if it is combineable with inv ie trying to buy while inv is full
        if (item != null)
        {
            foreach (Item comparedItem in allItems)
            {
                if (comparedItem != null && item.name == comparedItem.name && item.level == comparedItem.level && !comparedItem.lockCombine)
                {
                    CombineItems(comparedItem);
                    return true;
                }
            }
        }
        return false;
    }

    public void CombineDuplicateItems()
    {
        //check levels of all items
        // check for duplicates of lowest level item
        //if no duplicates, check next lowest level
        //continue untill all levels checked
        // give ui levelup/destroy event with coresponding items
        //ui plays event and then reruns CombineDuplicateItems 

        int checkLevelMax = 1;
        for (int i = 0; i < allItems.Length; i++)
        {
            if (allItems[i] != null)
            {
                checkLevelMax = Mathf.Max(checkLevelMax, allItems[i].level);
            }
        }

        for(int level = 1; level <= checkLevelMax; level++)
        {
            for (int i = 0; i < allItems.Length; i++)
            {
                // i is setting each item in all items as the item to be checked for
                Item checkedItem = allItems[i];

                if (checkedItem != null && checkedItem.level == level && !checkedItem.lockCombine)
                {
                    //l is being checked to see if matches i
                    for (int l = 0; l < allItems.Length; l++)
                    {
                        if (l != i && allItems[l] != null && !allItems[l].lockCombine && checkedItem.name == allItems[l].name && checkedItem.level == allItems[l].level)
                        {
                            CombineItems(checkedItem, allItems[l]);
                            return;
                        }
                    }
                }
            }
        }
    }

    public void CombineItems(Item upgradedItem, Item destroyedItem = null)
    {
        int destroyIndex = -1;

        if(destroyedItem != null)
        {
            destroyIndex = DisassembleItem(destroyedItem, true);
        }
        invUpgradeUi.PlayCardEffects(LevelUp(upgradedItem, true), destroyIndex); // give ints of upgraded and destroyed array positoion
    }

    public bool IsItemInStash(Item item)
    {
        return IndexOf(itemStash, item) > -1;
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

    private int IndexOf(Item[] collection, Item item, bool checkCombinable = false)
    {
        if (collection == null)
            return -1;

        for (int i = 0; i < collection.Length; i++)
        {
            if (item != null)
            {
                if (collection[i] != null && collection[i] == item && checkCombinable && !collection[i].lockCombine)
                {
                    return i;
                }
            }
            if (collection[i] == item )
            {
                return i;
            }
        }

        return -1;
    }

    public Item FindEventSource(ScriptableAction scriptEvent)
    {
        foreach(Item item in allItems)
        {
            if(item == null)
                continue;

            if(item.stats.events.events.Contains(scriptEvent))
            {
                return item;
            }
        }
        foreach(Item item in allItems)
        {
            if(item == null)
                continue;

            if(item.levelUpStats.events.events.Contains(scriptEvent))
            {
                return item;
            }
        }

        return null;
    }
    
    public void AddScrap(int amount)
    {
        scrap += amount;
    }

    public void AddKits(int amount)
    {
        upgradeKits += amount;
    }
}
