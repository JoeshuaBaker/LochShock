using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySubUi : MonoBehaviour
{
    public Inventory inventory;
    public InventoryUpgradeUi invUpgradeUi;

    public bool isSetup;

    public ItemDataFrame[] allFrames = new ItemDataFrame[10];
    public ItemDataFrame[] topItemFrames = new ItemDataFrame[5];
    public ItemDataFrame[] bottomItemFrames = new ItemDataFrame[5];
    public ItemDataFrame[] weaponItemFrames = new ItemDataFrame[2];
    public ItemDataFrame[] activeItemFrames = new ItemDataFrame[1];
    public ItemDataFrame[] stashItemFrames = new ItemDataFrame[2];

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Setup()
    {
        for (int i = 0; i < topItemFrames.Length; i++)
        {
            allFrames[i] = topItemFrames[i];
            allFrames[i + topItemFrames.Length] = bottomItemFrames[i];
        }
        weaponItemFrames[0] = topItemFrames[0];
        weaponItemFrames[1] = topItemFrames[1];
        activeItemFrames[0] = topItemFrames[2];
        stashItemFrames[0] = topItemFrames[3];
        stashItemFrames[1] = topItemFrames[4];

        //Set item data frame slot types
        foreach (ItemDataFrame itemFrame in weaponItemFrames)
        {
            itemFrame.slotType = Item.ItemType.Weapon;
        }

        foreach (ItemDataFrame itemFrame in activeItemFrames)
        {
            itemFrame.slotType = Item.ItemType.Active;
        }

        foreach (ItemDataFrame itemFrame in stashItemFrames)
        {
            itemFrame.isStash = true;
            itemFrame.slotType = Item.ItemType.Item;
        }

        foreach (ItemDataFrame itemFrame in bottomItemFrames)
        {
            itemFrame.slotType = Item.ItemType.Item;
        }

        invUpgradeUi.UpdateScrapAmount();

        isSetup = true;
    }

    public void DismissInventory()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].PlayCardOutro(-.2f);
        }
    }

    public void FocusInventory()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].PlayCardIntro(-.2f, true);
        }
    }

    public void AttemptUpgradeItem(ItemDataFrame itemFrame)
    {
        //if(itemFrame.item.levelUpCost > inventory.scrap)
        //{
        //    itemFrame.PlayContextMessage("NOT ENOUGH SCRAP");
        //}
        //else
        //{
        //    inventory.LevelUp(itemFrame.item);
        //    invUpgradeUi.UpdateScrapAmount();
        //    itemFrame.PlayUpgradeEffect();
        //    itemFrame.PlayCardShake();
        //}

        itemFrame.PlayCardShake();
        itemFrame.PlayUpgradeEffect();

    }

    public void AttemptRecycleItem(ItemDataFrame frame)
    {
        if (frame.item == inventory.activeGun)
        {
            if (inventory.inactiveGun == null)
            {
                frame.PlayCardShake();
                frame.PlayContextMessage("EQUIPPED");
                return;
            }
            else
            {
                inventory.SwitchWeapons();
            }
        }

        inventory.DisassembleItem(frame.item);
        invUpgradeUi.UpdateScrapAmount();
    }

    public void Take(ItemDataFrame frame)
    {
        bool addedItem = inventory.AddItem(frame.item);
        if (addedItem)
        {
            //TransitionState(InventoryUIState.Close);

            //Audio Section
            AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);
        }
        else
        {
            frame.PlayContextMessage("FULL");
        }
    }
}
