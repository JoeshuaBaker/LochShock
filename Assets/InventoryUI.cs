using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if(inventory != null)
        {
            this.inventory = inventory;
        }
        
        if(!isSetup)
        {
            Setup();
        }
        if (!gameObject.activeSelf && newState != InventoryUIState.Close)
        {
            gameObject.SetActive(true);
        }

        switch (newState)
        {
            case InventoryUIState.Inventory:
                Inventory(newState, items);
                break;

            case InventoryUIState.Orb:
                Orb(newState, items);
                break;

            case InventoryUIState.Stats:
                //TODO, NOT IMPLEMENTED
                return;

            case InventoryUIState.Close:
                gameObject.SetActive(false);
                break;
        }

        lastState = state;
        lastItems = items;
        state = newState;
    }

    private void Inventory(InventoryUIState newState, Item[] items)
    {
        bottomFrameParent.gameObject.SetActive(true);
        inventoryButton.interactable = true;
        inventoryButtonText.text = "Return";

        Inventory inventory = Player.activePlayer.inventory;
        Gun[] weapons = inventory.guns;
        Item[] activeItem = inventory.activeItem;
        Item[] itemStash = inventory.itemStash;
        Item[] heldItems = inventory.items;
        int offset = 0;

        //Set buttons to level up and disassemble
        foreach (ItemDataFrame frame in allFrames)
        {
            frame.SetupButton(frame.topButton, frame.topButtonText, LevelUp, nameof(LevelUp).SplitCamelCase(), true);
            frame.SetupButton(frame.bottomButton, frame.bottomButtonText, Disassemble, nameof(Disassemble).SplitCamelCase(), true);
        }

        for (int i = 0; i < weaponItemFrames.Length; i++)
        {
            weaponItemFrames[i].ReflectInventoryState(newState, weapons[i], offset++);
        }
        for (int i = 0; i < activeItemFrames.Length; i++)
        {
            activeItemFrames[i].ReflectInventoryState(newState, activeItem[i], offset++);
        }
        for (int i = 0; i < stashItemFrames.Length; i++)
        {
            stashItemFrames[i].ReflectInventoryState(newState, itemStash[i], offset++);
        }
        for (int i = 0; i < bottomItemFrames.Length; i++)
        {
            bottomItemFrames[i].ReflectInventoryState(newState, heldItems[i], offset++);
        }
    }

    private void Orb(InventoryUIState newState, Item[] items)
    {
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

            //Set buttons to Take and Disassemble
            frame.SetupButton(frame.topButton, frame.topButtonText, Take, nameof(Take).SplitCamelCase(), false);
            frame.SetupButton(frame.bottomButton, frame.bottomButtonText, Disassemble, nameof(Disassemble).SplitCamelCase(), true);

            frame.ReflectInventoryState(newState, inRange ? items[i] : null);
        }
    }

    public void OnStatsButtonPressed()
    {
        TransitionState(InventoryUIState.Stats);
    }

    public void OnInventoryButtonPressed()
    {
        if(state == InventoryUIState.Inventory)
        {
            TransitionState(lastState, inventory, lastItems);
        }
        else
        {
            TransitionState(InventoryUIState.Inventory, inventory, lastItems);
        }
    }

    public void OnContinueButtonPressed()
    {
        TransitionState(InventoryUIState.Close);
    }

    public void Take(ItemDataFrame frame)
    {
        inventory.AddItem(frame.item);
        TransitionState(InventoryUIState.Close);
    }

    public void Stash(ItemDataFrame frame)
    {
        Debug.Log("Stash called on item frame " + frame.name);
    }

    public void LevelUp(ItemDataFrame frame)
    {
        Debug.Log("Level up button pressed on item " + frame.item.name);
        inventory.LevelUp(frame.item);
        frame.SetupButton(frame.topButton, frame.topButtonText, LevelUp, nameof(LevelUp).SplitCamelCase(), true);
        frame.ReflectInventoryState(state, frame.item);
        scrapText.text = inventory.scrap.ToString();
    }

    public void Disassemble(ItemDataFrame frame)
    {
        inventory.DisassembleItem(frame.item);

        if(state == InventoryUIState.Inventory)
        {
            frame.ReflectInventoryState(state, null);
        }
        else if(state == InventoryUIState.Orb)
        {
            TransitionState(InventoryUIState.Inventory);
            inventoryButtonText.text = "Inventory";
            inventoryButton.interactable = false;
        }

        scrapText.text = inventory.scrap.ToString();
    }
}
