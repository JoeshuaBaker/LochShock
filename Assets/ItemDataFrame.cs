using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDataFrame : MonoBehaviour
{
    public Item item;
    private Item cachedItem;
    public Image icon;
    public TMP_Text itemName;
    public TMP_Text itemLevel;
    public TMP_Text itemRarity;
    public Image itemFrame;
    public Sprite[] frameRarities;
    public TMP_Text itemStatusSlot;
    public TMP_Text itemData;
    public Button levelUpButton;
    public TMP_Text levelUpButtonText;
    public Button disassembleButton;
    public TMP_Text disassembleButtonText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(item != null && cachedItem == null)
        {
            UpdateFields();
            cachedItem = item;
        }

        if(item != null && cachedItem != null && item != cachedItem)
        {
            UpdateFields();
            cachedItem = item;
        }
    }

    void UpdateFields()
    {
        if (item != null)
        {
            icon.sprite = item.icon;
            if((int)item.rarity < frameRarities.Length)
            {
                itemFrame.sprite = frameRarities[(int)item.rarity];
            }
            itemName.text = item.name;
            itemLevel.text = "Lv " + item.level.ToString();
            itemRarity.text = item.rarity.ToString();
            itemStatusSlot.text = item.itemType.ToString();
            levelUpButtonText.text = $"Level Up ({item.levelUpCost})";
            disassembleButtonText.text = $"Disassemble ({item.disassembleValue})";

            StatBlockContext context = item.GetStatBlockContext();
            IEnumerable<string> itemDataStrings = context.GetStatContextStrings();
            IEnumerable<string> eventStrings = item.GetEventTooltips();

            string itemDataString = "";
            foreach(string data in itemDataStrings)
            {
                itemDataString += data + Environment.NewLine;
            }

            itemDataString += Environment.NewLine;

            foreach(string eventString in eventStrings)
            {
                itemDataString += eventString + Environment.NewLine;
            }

            itemData.text = itemDataString.TrimEnd();
        }
    }
}
