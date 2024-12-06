using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySubUi : MonoBehaviour
{
    public Inventory inventory;
    public InventoryUpgradeUi invUpgradeUi;

    public Animator stashOutlineAnimator;

    public bool isSetup;
    public bool levelUpMode;
    public bool recycleMode;
    public bool checkOutro;

    public ItemDataFrame[] allFrames = new ItemDataFrame[10];
    public ItemDataFrame[] topItemFrames = new ItemDataFrame[5];
    public ItemDataFrame[] bottomItemFrames = new ItemDataFrame[5];
    public ItemDataFrame[] weaponItemFrames = new ItemDataFrame[2];
    public ItemDataFrame[] activeItemFrames = new ItemDataFrame[1];
    public StashDataFrame[] stashItemFrames = new StashDataFrame[2];


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (checkOutro)
        {
            AnimatorStateInfo animState = stashOutlineAnimator.GetCurrentAnimatorStateInfo(0);

            if (animState.IsName("StashOutlineOutro") && animState.normalizedTime >= 1f)
            {
                this.gameObject.SetActive(false);
                checkOutro = false;
            }
        }
    }

    void Setup()
    {

        inventory = Player.activePlayer.inventory;

        foreach (ItemDataFrame itemFrame in weaponItemFrames)
        {
            itemFrame.slotType = Item.ItemType.Weapon;
        }

        foreach (ItemDataFrame itemFrame in activeItemFrames)
        {
            itemFrame.slotType = Item.ItemType.Active;
        }

        foreach (ItemDataFrame itemFrame in bottomItemFrames)
        {
            itemFrame.slotType = Item.ItemType.Item;
        }

        invUpgradeUi.UpdateScrapAmount();

        isSetup = true;
    }

    public void DisplayInventory(bool basicSet = false)
    {

        if (!isSetup)
        {
            Setup();
        }

        this.gameObject.SetActive(true);

        Gun[] weapons = inventory.guns;
        Item[] activeItem = inventory.activeItems;
        Item[] itemStash = inventory.itemStash;
        Item[] heldItems = inventory.items;

        for (int i = 0; i < weaponItemFrames.Length; i++)
        {
            weaponItemFrames[i].SetItem(weapons[i]);
        }
        for (int i = 0; i < activeItemFrames.Length; i++)
        {
            activeItemFrames[i].SetItem(activeItem[i]);
        }
        for (int i = 0; i < stashItemFrames.Length; i++)
        {
            stashItemFrames[i].SetItem(itemStash[i]);
        }
        for (int i = 0; i < bottomItemFrames.Length; i++)
        {
            bottomItemFrames[i].SetItem(heldItems[i]);
        }

        if (!basicSet)
        {
            //Audio Section
            AkSoundEngine.PostEvent("PlayMenuOpen", this.gameObject);

            FocusInventory();
        }
    }

    public void ToggleLevelUpMode()
    {
        if (levelUpMode)
        {
            levelUpMode = false;
        }
        else
        {
            levelUpMode = true;
            recycleMode = false;
        }
    }

    public void ToggleRecycleMode()
    {
        if (recycleMode)
        {
            recycleMode = false;
        }
        else
        {
            recycleMode = true;
            levelUpMode = false;
        }
    }

    public void AllModesOff()
    {
        recycleMode = false;
        levelUpMode = false;
    }

    public void DismissInventory()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].PlayCardOutro(-.2f);
        }
        for(int i =0; i < stashItemFrames.Length; i++)
        {
            stashItemFrames[i].PlayCardOutro(-0.03f * i);
        }

        //stashOutlineAnimator.SetBool("Outro", true);
        stashOutlineAnimator.Play("StashOutlineOutro");
        checkOutro = true;
    }

    public void FocusInventory()
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            allFrames[i].PlayCardIntro(-.2f, true);
        }
        for (int i = 0; i < stashItemFrames.Length; i++)
        {
            stashItemFrames[i].PlayCardIntro(-.2f);
        }

        //stashOutlineAnimator.SetBool("Outro", false);
        stashOutlineAnimator.Play("StashOutlineIntro");
    }

    public void CardClicked(IDataFrame itemFrame)
    {
        if (levelUpMode)
        {
            AttemptUpgradeItem(itemFrame);
        }
        else if (recycleMode)
        {
            AttemptRecycleItem(itemFrame);
        }
    }

    public void AttemptUpgradeItem(IDataFrame itemFrame)
    {
        if (itemFrame.GetItem().levelUpCost > inventory.scrap)
        {
            itemFrame.PlayContextMessage("NOT ENOUGH SCRAP");
            itemFrame.PlayCardShake();
        }
        else
        {
            inventory.LevelUp(itemFrame.GetItem());
            invUpgradeUi.UpdateScrapAmount();
            itemFrame.PlayUpgradeEffect();
            itemFrame.PlayCardShake();
            DisplayInventory(true);
        }
    }

    public void AttemptRecycleItem(IDataFrame frame)
    {
        if (frame.GetItem() == inventory.activeGun)
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

        inventory.DisassembleItem(frame.GetItem());
        invUpgradeUi.UpdateScrapAmount();
        frame.PlayRecycleEffect();
        frame.PlayCardShake();
        frame.SetItem(null);
    }
}
