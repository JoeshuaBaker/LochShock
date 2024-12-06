using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public enum InventoryUIState
    {
        Inventory,
        Orb,
        Stats,
        Close
    }

    //Internal State Variables
    public InventoryUIState state = InventoryUIState.Close;
    bool isSetup = false;
    public Inventory inventory;
    private InventoryUIState lastState = InventoryUIState.Close;
    private Item[] lastItems = null;
    private Item[] items = null;

    //Item Frames
    public ItemDataFrame[] allFrames = new ItemDataFrame[10];
    public ItemDataFrame[] topItemFrames = new ItemDataFrame[5];
    public ItemDataFrame[] bottomItemFrames = new ItemDataFrame[5];
    public ItemDataFrame[] weaponItemFrames = new ItemDataFrame[2];
    public ItemDataFrame[] activeItemFrames = new ItemDataFrame[1];
    public ItemDataFrame[] stashItemFrames = new ItemDataFrame[2];

    //Prefab References
    public Transform bottomFrameParent;
    public Button statsButton;
    public TMP_Text statsButtonText;
    public Button inventoryButton;
    public TMP_Text inventoryButtonText;
    public Button continueButton;
    public TMP_Text continueButtonText;
    public TMP_Text scrapText;

    public Action OnClose;
    public EventSystem eventSystem;

    // Start is called before the first frame update
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

        scrapText.text = inventory.scrap.ToString();

        isSetup = true;
    }

    public void TransitionState(InventoryUIState newState, Inventory inventory = null, Item[] items = null)
    {
        if (inventory != null)
        {
            this.inventory = inventory;
        }

        if (!isSetup)
        {
            Setup();
        }
        if (!gameObject.activeSelf && newState != InventoryUIState.Close)
        {
            gameObject.SetActive(true);
        }

        World.activeWorld.Pause(newState != InventoryUIState.Close);

        switch (newState)
        {
            case InventoryUIState.Inventory:
                Inventory(newState, items);

                //AudioSection
                AkSoundEngine.SetState("MenuStatus", "MenuOpen");
                break;

            case InventoryUIState.Orb:
                Orb(newState, items);

                //AudioSection
                AkSoundEngine.SetState("MenuStatus", "MenuOpen");
                break;

            case InventoryUIState.Stats:
                //TODO, NOT IMPLEMENTED
                return;

            case InventoryUIState.Close:
                //Audio Section
                AkSoundEngine.PostEvent("PlayMenuClose", this.gameObject);
                AkSoundEngine.SetState("MenuStatus", "MenuClose");

                gameObject.SetActive(false);
                OnClose?.Invoke();
                OnClose = null;
                break;
        }

        lastState = state;
        lastItems = this.items;
        state = newState;
        this.items = items;
    }

    public void TransitionToLastState()
    {
        TransitionState(lastState, inventory, lastItems);
    }

    private void Inventory(InventoryUIState newState, Item[] items)
    {
        //Audio Section
        AkSoundEngine.PostEvent("PlayMenuOpen", this.gameObject);

        bottomFrameParent.gameObject.SetActive(true);
        inventoryButton.interactable = true;
        inventoryButtonText.text = "Return";
        eventSystem.SetSelectedGameObject(allFrames[0].topButton.gameObject);

        Gun[] weapons = inventory.guns;
        Item[] activeItem = inventory.activeItems;
        Item[] itemStash = inventory.itemStash;
        Item[] heldItems = inventory.items;

        //Set buttons to level up and disassemble
        foreach (ItemDataFrame frame in allFrames)
        {
            if (frame.isStash)
            {
                frame.SetupButton(frame.topButton, frame.leftTabText, Unstash, nameof(Unstash).SplitCamelCase(), false);
            }
            else
            {
                frame.SetupButton(frame.topButton, frame.leftTabText, LevelUp, nameof(LevelUp).SplitCamelCase(), true);
            }
            frame.SetupButton(frame.bottomButton, frame.rightTabText, Disassemble, nameof(Disassemble).SplitCamelCase(), true);
        }

        for (int i = 0; i < weaponItemFrames.Length; i++)
        {
            weaponItemFrames[i].ReflectInventoryState(newState, weapons[i]);
        }
        for (int i = 0; i < activeItemFrames.Length; i++)
        {
            activeItemFrames[i].ReflectInventoryState(newState, activeItem[i]);
        }
        for (int i = 0; i < stashItemFrames.Length; i++)
        {
            stashItemFrames[i].ReflectInventoryState(newState, itemStash[i]);
        }
        for (int i = 0; i < bottomItemFrames.Length; i++)
        {
            bottomItemFrames[i].ReflectInventoryState(newState, heldItems[i]);
        }
    }

    private void Orb(InventoryUIState newState, Item[] items)
    {
        //Audio Section
        AkSoundEngine.PostEvent("PlayOrbGet", this.gameObject);

        inventoryButton.interactable = true;
        inventoryButtonText.text = "Inventory";
        scrapText.text = inventory.scrap.ToString();

        if (items.Length <= 5)
        {
            bottomFrameParent.gameObject.SetActive(false);
        }

        for (int i = 0; i < allFrames.Length; i++)
        {
            bool inRange = i < items.Length;
            ItemDataFrame frame = allFrames[i];
            Item item = i < items.Length ? items[i] : null;

            //Set buttons to Take and Disassemble
            frame.SetupButton(frame.topButton, frame.leftTabText, Take, nameof(Take).SplitCamelCase(), false);
            frame.SetupButton(frame.bottomButton, frame.rightTabText, Disassemble, nameof(Disassemble).SplitCamelCase(), true);

            if (item != null && inventory.HasSpaceFor(item) && !inventory.HasNonStashSpaceFor(item))
            {
                frame.leftTabText.text = "Take (Stash)";
            }

            frame.ReflectInventoryState(newState, inRange ? item : null);
        }
    }

    public void OnStatsButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");

        //Audio Section
        AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);
    }

    public void OnInventoryButtonPressed()
    {
        if (state == InventoryUIState.Inventory)
        {
            TransitionToLastState();
        }
        else
        {
            TransitionState(InventoryUIState.Inventory, inventory, lastItems);
        }
    }

    public void OnContinueButtonPressed()
    {
        TransitionState(InventoryUIState.Close);

        //Audio Section
        AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);
    }

    public void Take(ItemDataFrame frame)
    {
        bool addedItem = inventory.AddItem(frame.item);
        if (addedItem)
        {
            if(inventory.IsItemInStash(frame.item))
            {
                TransitionState(InventoryUIState.Inventory);
            }
            else
            {
                TransitionState(InventoryUIState.Close);
            }
            //Audio Section
            AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);
        }
        else
        {
            if (!frame.leftTabText.text.Contains("(Make Room)"))
            {
                frame.leftTabText.text += "(Make Room)";
            }
        }
    }

    public void Unstash(ItemDataFrame frame)
    {
        Item item = frame.item;
        int unstashIndex = inventory.UnstashItem(item);

        if(unstashIndex > -1)
        {
            frame.ReflectInventoryState(state, null);
            ItemDataFrame unstashedFrame = null;
            switch(item.itemType)
            {
                case Item.ItemType.Item:
                    unstashedFrame = bottomItemFrames[unstashIndex];
                    break;

                case Item.ItemType.Weapon:
                    unstashedFrame = weaponItemFrames[unstashIndex];
                    break;

                case Item.ItemType.Active:
                    unstashedFrame = activeItemFrames[unstashIndex];
                    break;
            }

            if(unstashedFrame != null)
            {
                unstashedFrame.ReflectInventoryState(state, item);
                if (frame.topButton.interactable)
                {
                    eventSystem.SetSelectedGameObject(unstashedFrame.topButton.gameObject);
                }
                else
                {
                    eventSystem.SetSelectedGameObject(continueButton.gameObject);
                }
            }
        }
        else
        {
            if(!frame.leftTabText.text.Contains("(Make Room)"))
            {
                frame.leftTabText.text += "(Make Room)";
            }
        }
    }

    public void UpdateAllItemFrames(InventoryUIState state)
    {
        foreach(ItemDataFrame frame in allFrames)
        {
            frame.ReflectInventoryState(state, frame.item);
        }
    }

    public void LevelUp(ItemDataFrame frame)
    {
        inventory.LevelUp(frame.item);
        frame.SetupButton(frame.topButton, frame.leftTabText, LevelUp, nameof(LevelUp).SplitCamelCase(), true);
        frame.ReflectInventoryState(state, frame.item);
        scrapText.text = inventory.scrap.ToString();
        if(!frame.topButton.interactable)
        {
            eventSystem.SetSelectedGameObject(continueButton.gameObject);
        }

        //Audio Section
        AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);
        UpdateAllItemFrames(state);
    }

    public void Disassemble(ItemDataFrame frame)
    {
        if (frame.item == inventory.activeGun)
        {
            if(inventory.inactiveGun == null)
            {
                frame.bottomButton.interactable = false;
                frame.rightTabText.text = "Can't Disassemble.";
                if (frame.topButton.interactable)
                {
                    eventSystem.SetSelectedGameObject(frame.topButton.gameObject);
                }
                else
                {
                    eventSystem.SetSelectedGameObject(continueButton.gameObject);
                }
                return;
            }
            else
            {
                inventory.SwitchWeapons();
                foreach(var weaponFrame in weaponItemFrames)
                {
                    if(weaponFrame.item == inventory.activeGun && !weaponFrame.rightTabText.text.Contains("(E)"))
                    {
                        weaponFrame.rightTabText.text += "(E)";
                    }
                }
            }
        }

        inventory.DisassembleItem(frame.item);

        if (state == InventoryUIState.Inventory)
        {
            frame.ReflectInventoryState(state, null);
            UpdateAllItemFrames(state);
        }
        else if (state == InventoryUIState.Orb)
        {
            TransitionState(InventoryUIState.Inventory);
            inventoryButtonText.text = "Inventory";
            inventoryButton.interactable = false;
            lastState = InventoryUIState.Close;
        }

        if (!frame.bottomButton.interactable)
        {
            if (frame.topButton.interactable)
            {
                eventSystem.SetSelectedGameObject(frame.topButton.gameObject);
            }
            else
            {
                eventSystem.SetSelectedGameObject(continueButton.gameObject);
            }
        }

        //Audio Section
        AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);

        scrapText.text = inventory.scrap.ToString();
    }
}
