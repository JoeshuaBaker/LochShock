using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUpgradeUi : MonoBehaviour
{
    public UpgradeSubUi upgradeUi;
    public InventorySubUi invUi;
    public MenuSubUi menuUi;
    public StatsSubUi statsUi;

    public Inventory inventory;

    public bool hasActiveUpgrade;

    public bool focusInv;
    public TMP_Text switchButtonText;

    public bool levelPreview;
    public Image levelPreviewDiamond;
    public bool levelPreviewAll;
    public Image levelPreviewAllDiamond;
    public Sprite[] previewDiamondSprites;

    public TMP_Text heldScrap;

    public bool statsActive;

    public MenuButtonEventHandler switchButton;
    public MenuButtonEventHandler continueButton;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPreviewButtonPressed()
    {
        if (!levelPreview)
        {
            levelPreview = true;
            levelPreviewDiamond.sprite = previewDiamondSprites[1];
        }
        else if (levelPreview && !levelPreviewAll)
        {
            levelPreviewAll = true;
            levelPreviewAllDiamond.sprite = previewDiamondSprites[1];
        }
        else
        {
            levelPreview = false;
            levelPreviewAll = false;
            levelPreviewDiamond.sprite = previewDiamondSprites[0];
            levelPreviewAllDiamond.sprite = previewDiamondSprites[0];
        }
    }

    public void OnMenuButtonPressed()
    {
        menuUi.FocusMenuSubUi();
    }

    public void OnSwitchButtonPressed()
    {
        if (focusInv)
        {
            if (statsActive)
            {
                statsUi.DismissStats(focusInv);
                statsActive = false;
            }
            focusInv = false;
            SwitchToUpgrade();
            switchButtonText.text = "UPGRADE";
        }
        else
        {
            if (statsActive)
            {
                statsUi.DismissStats(focusInv);
                statsActive = false;
            }
            focusInv = true;
            SwitchToInventory();
            switchButtonText.text = "INVENTORY";
        }
    }

    public void OnStatsButtonPressed()
    {
        if (!statsActive)
        {
            statsUi.FocusStats(focusInv);
            statsActive = true;
        }
        else
        {
            statsUi.DismissStats(focusInv);
            statsActive = false;
        }
    }

    public void OnContinueButtonPressed()
    {
        UiClose();
    }

    public void SwitchToInventory()
    {
        //switching from upgrade to inv
        invUi.FocusInventory();
        upgradeUi.DismissUpgradeUi();
    }

    public void SwitchToUpgrade()
    {
        //switching from inv to upgrade
        upgradeUi.FocusUpgradeUi();
        invUi.DismissInventory();
    }

    public void EnterInventory()
    {
        // entering inv from gameplay
        invUi.FocusInventory();
        hasActiveUpgrade = false;
        focusInv = true;
        switchButtonText.text = "UPGRADE";

        switchButton.DisableButton();
        continueButton.EnableButton();

    }

    public void EnterUpgrade()
    {
        // entering upgrade from gameplay
        upgradeUi.FocusUpgradeUi();
        hasActiveUpgrade = true;
        focusInv = false;
        switchButtonText.text = "INVENTORY";

        continueButton.DisableButton();
        switchButton.EnableButton();

    }

    public void UiClose()
    {
        // continue the game
    }

    public void UpdateScrapAmount()
    {
        if(inventory != null)
        {
            heldScrap.text = $"x{inventory.scrap.ToString()}";
        }

    }

}
