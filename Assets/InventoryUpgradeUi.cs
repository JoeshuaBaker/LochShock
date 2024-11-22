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

    public bool interactInventory;
    public bool focusInv;
    public TMP_Text switchButtonText;

    public bool levelPreview;

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
        }
        else
        {
            levelPreview = false;
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
            switchButtonText.text = "INVENTORY";
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
            switchButtonText.text = "UPGRADE";
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
        invUi.DisplayInventory();
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
        invUi.DisplayInventory();
        hasActiveUpgrade = false;
        focusInv = true;
        switchButtonText.text = "UPGRADE";

        switchButton.DisableButton();
        continueButton.EnableButton();

        FocusBottomButtons();

    }

    public void EnterUpgrade(Item[] items)
    {
        // entering upgrade from gameplay
        upgradeUi.DisplayItems(items);
        hasActiveUpgrade = true;
        focusInv = false;
        switchButtonText.text = "INVENTORY";

        continueButton.DisableButton();
        switchButton.EnableButton();

        FocusBottomButtons();
    }

    public void InteractInventory()
    {
        if (hasActiveUpgrade)
        {
            return;
        }
        if (!interactInventory)
        {
            EnterInventory();
            interactInventory = true;
        }
        else
        {
            invUi.DismissInventory();
            UiClose();
        }
    }

    public void UiClose()
    {
        // continue the game
        DismissBottomButtons();
    }

    public void UpdateScrapAmount()
    {
        if(inventory != null)
        {
            heldScrap.text = $"x{inventory.scrap.ToString()}";
        }
    }

    public void FocusBottomButtons()
    {
        // mov ethe bottom buttons in
    }

    public void DismissBottomButtons()
    {
        //move them away
    }
}
