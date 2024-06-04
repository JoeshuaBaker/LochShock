using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemResourcesAtlas", menuName = "NemesisShock/ItemResourcesAtlas")]
public class ItemResourcesAtlas : ScriptableObject
{
    [SerializeField] private string baseGunsPath = "Prefabs/Guns";
    [SerializeField] private string baseItemsPath = "Prefabs/Items";
    [SerializeField] private string baseActiveItemsPath = "Prefabs/ActiveItems";
    [SerializeField] private string commonFolderName = "01_Common";
    [SerializeField] private string uncommonFolderName = "02_Uncommon";
    [SerializeField] private string rareFolderName = "03_Rare";
    [SerializeField] private string epicFolderName = "04_Epic";
    [SerializeField] private string legendaryFolderName = "05_Legendary";

    private Dictionary<string, Item[]> itemMap;

    public Item[] GetItemList(Item.ItemType type, Item.Rarity rarity)
    {
        itemMap.TryGetValue(GetItemFolderPath(type, rarity), out Item[] collection);
        return collection;
    }

    private string GetItemFolderPath(Item.ItemType type, Item.Rarity rarity)
    {
        string root;
        string folder;
        switch(type)
        {
            default:
            case Item.ItemType.Item:
                root = baseItemsPath;
                break;

            case Item.ItemType.Weapon:
                root = baseGunsPath;
                break;

            case Item.ItemType.Active:
                root = baseActiveItemsPath;
                break;
        }

        switch(rarity)
        {
            default:
            case Item.Rarity.Common:
                folder = commonFolderName;
                break;

            case Item.Rarity.Uncommon:
                folder = uncommonFolderName;
                break;

            case Item.Rarity.Rare:
                folder = rareFolderName;
                break;

            case Item.Rarity.Epic:
                folder = epicFolderName;
                break;

            case Item.Rarity.Legendary:
                folder = legendaryFolderName;
                break;
        }

        return root + '/' + folder;
    }

    void Reset()
    {
        Setup();
    }

    public void Setup()
    {
        itemMap = new Dictionary<string, Item[]>();

        foreach (Item.ItemType itemType in Enum.GetValues(typeof(Item.ItemType)))
        {
            foreach (Item.Rarity itemRarity in Enum.GetValues(typeof(Item.Rarity)))
            {
                string path = GetItemFolderPath(itemType, itemRarity);

                Item[] items = Resources.LoadAll<Item>(path);

                itemMap.Add(path, items);
            }
        }
    }
}
