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

    //Item Frames
    public ItemDataFrame[] allFrames = new ItemDataFrame[10];
    public ItemDataFrame[] topItemFrames = new ItemDataFrame[5];
    public ItemDataFrame[] bottomItemFrames = new ItemDataFrame[5];
    public ItemDataFrame[] weaponItemFrames = new ItemDataFrame[2];
    public ItemDataFrame[] activeItemFrames = new ItemDataFrame[1];
    public ItemDataFrame[] stashItemFrames = new ItemDataFrame[2];

    //Buttons
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

        isSetup = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TransitionState(InventoryUIState newState, List<Item> items = null)
    {
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

        state = newState;
    }

    private void Inventory(InventoryUIState newState, List<Item> items)
    {
        Inventory inventory = Player.activePlayer.inventory;
        Gun[] weapons = inventory.guns;
        Item[] activeItem = inventory.activeItem;
        Item[] itemStash = inventory.itemStash;
        Item[] heldItems = inventory.items;
        int offset = 0;

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

        //Set buttons to level up and disassemble
        foreach (ItemDataFrame frame in allFrames)
        {
            frame.SetupButton(frame.topButton, frame.topButtonText, LevelUp, nameof(LevelUp).SplitCamelCase(), true);
            frame.SetupButton(frame.bottomButton, frame.bottomButtonText, Disassemble, nameof(Disassemble).SplitCamelCase(), true);
        }
    }

    private void Orb(InventoryUIState newState, List<Item> items)
    {
        for (int i = 0; i < allFrames.Length; i++)
        {
            bool inRange = i < items.Count;
            ItemDataFrame frame = allFrames[i];
            frame.ReflectInventoryState(newState, inRange ? items[i] : null);

            //Set buttons to Take and Disassemble
            frame.SetupButton(frame.topButton, frame.topButtonText, Take, nameof(Take).SplitCamelCase(), false);
            frame.SetupButton(frame.bottomButton, frame.bottomButtonText, Disassemble, nameof(Disassemble).SplitCamelCase(), true);
        }
    }

    public void OnStatsButtonPressed()
    {
        TransitionState(InventoryUIState.Stats);
    }

    public void OnInventoryButtonPressed()
    {
        TransitionState(InventoryUIState.Inventory);
    }

    public void OnContinueButtonPressed()
    {
        TransitionState(InventoryUIState.Close);
    }

    public void Take(ItemDataFrame itemDataFrame)
    {
        Debug.Log("Take called on item frame " + itemDataFrame.name);
    }

    public void Stash(ItemDataFrame itemDataFrame)
    {
        Debug.Log("Stash called on item frame " + itemDataFrame.name);
    }

    public void LevelUp(ItemDataFrame itemDataFrame)
    {
        Debug.Log("Level Up called on item frame " + itemDataFrame.name);
    }

    public void Disassemble(ItemDataFrame itemDataFrame)
    {
        Debug.Log("Disassemble called on item frame " + itemDataFrame.name);
    }
}
