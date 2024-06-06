using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDataFrame : MonoBehaviour
{
    public Item item;
    public bool isStash = false;
    public Item.ItemType slotType = Item.ItemType.Item;
    public InventoryUI.InventoryUIState inventoryUIState;
    public Transform fullFrameParent;
    public Transform emptyFrameParent;
    public Animator flyinAnimator;
    public Image invDataFrameImage;
    public Image icon;
    public TMP_Text itemName;
    public TMP_Text itemLevel;
    public TMP_Text itemRarity;
    public Image itemFrame;
    public Sprite[] frameRarities;
    public TMP_Text itemStatusSlot;
    public TMP_Text itemData;
    public Button topButton;
    public TMP_Text topButtonText;
    public Button bottomButton;
    public TMP_Text bottomButtonText;
    public TMP_Text emptyFrameSlotText;
    public TMP_Text emptyFrameSlotGlowText;

    // Start is called before the first frame update
    void Start()
    {
        if(item != null)
        {
            slotType = item.itemType;
            ReflectInventoryState(inventoryUIState, item);
        }
    }

    private void SetItem(Item item)
    {
        this.item = item;
        topButton.interactable = item != null;
        bottomButton.interactable = item != null;

        if (item == null)
        {
            return;
        }

        icon.sprite = item.icon;
        if ((int)item.rarity < frameRarities.Length)
        {
            itemFrame.sprite = frameRarities[(int)item.rarity];
        }
        itemName.text = item.name;
        itemLevel.text = "Lv " + item.level.ToString();
        itemRarity.text = item.rarity.ToString();
        itemStatusSlot.text = item.itemType.ToString();
        if(topButtonText.text.Contains("%value%"))
        {
            topButton.interactable = Player.activePlayer.inventory.scrap >= item.levelUpCost;
        }
        topButtonText.text = topButtonText.text.Replace("%value%", item.levelUpCost.ToString());
        bottomButtonText.text = bottomButtonText.text.Replace("%value%", item.disassembleValue.ToString());

        StatBlockContext context = item.GetStatBlockContext();
        IEnumerable<string> itemDataStrings = context.GetStatContextStrings();
        IEnumerable<string> eventStrings = item.GetEventTooltips();

        string itemDataString = "";
        foreach (string data in itemDataStrings)
        {
            itemDataString += data + Environment.NewLine;
        }

        itemDataString += Environment.NewLine;

        foreach (string eventString in eventStrings)
        {
            itemDataString += eventString + Environment.NewLine;
        }

        itemData.text = itemDataString.TrimEnd();
    }

    public void ReflectInventoryState(InventoryUI.InventoryUIState state, Item item, int offset = 0)
    {
        inventoryUIState = state;

        switch (state)
        {
            case InventoryUI.InventoryUIState.Inventory:
                gameObject.SetActive(true);
                if(this.item != item)
                {
                    flyinAnimator.Play("UiItemIntro", 0, UnityEngine.Random.Range(-.2f, 0f));
                }

                //disable disassemble button
                if(item == Player.activePlayer.inventory.activeGun && !bottomButtonText.text.Contains("(E)"))
                {
                    bottomButtonText.text += "(E)";
                }

                fullFrameParent.gameObject.SetActive(item != null);

                if (isStash)
                {
                    emptyFrameSlotText.text = $"Stash";
                    emptyFrameSlotGlowText.text = $"Stash";
                }
                else
                {
                    emptyFrameSlotText.text = slotType.ToString();
                    emptyFrameSlotGlowText.text = slotType.ToString();
                }

                invDataFrameImage.color = isStash ? Color.grey : Color.white;
                break;

            case InventoryUI.InventoryUIState.Orb:
                invDataFrameImage.color = Color.white;
                gameObject.SetActive(item != null);
                fullFrameParent.gameObject.SetActive(item != null);
                flyinAnimator.Play("UiItemIntro", 0, UnityEngine.Random.Range(-.2f, 0f));
                break;

            default:
                topButtonText.text = "";
                bottomButtonText.text = "";
                break;
        }

        SetItem(item);
    }

    public void SetupButton(Button button, TMP_Text buttonText, Action<ItemDataFrame> function, string functionName, bool hasValue)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { function.Invoke(this); });
        buttonText.text = $"{functionName}{(hasValue ? " (%value%)" : "")}";
    }
}
