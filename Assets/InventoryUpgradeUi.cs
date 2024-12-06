using System;
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
    public Crosshair cursor;

    public bool hasActiveUpgrade;

    public bool interactInventory;
    public bool focusInv;
    public TMP_Text switchButtonText;

    public bool levelPreview;

    public TMP_Text heldScrap;

    public bool statsActive;
    public bool menuActive;

    public MenuButtonEventHandler switchButton;
    public MenuButtonEventHandler continueButton;

    public bool checkOutro;
    public Animator animator;

    public Action OnClose;

    public float shimmerScrollTime;
    public Material shimmerMat;

    // Start is called before the first frame update
    void Start()
    {
        if(inventory == null)
        {
            inventory = Player.activePlayer.inventory;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (checkOutro)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

            if (animState.IsName("BottomButtonsOutro") && animState.normalizedTime >= 1f)
            {
                if(!upgradeUi.checkOutro && !menuUi.checkOutro && !statsUi.checkOutro && !invUi.checkOutro)
                {
                    checkOutro = false;
                    World.activeWorld.Pause(false);
                    OnClose?.Invoke();
                    OnClose = null;

                    invUi.gameObject.SetActive(false);

                    this.gameObject.SetActive(false);
                }
            }
        }

        shimmerScrollTime += Time.unscaledDeltaTime;
        shimmerMat.SetFloat("_UnscaledTime", shimmerScrollTime);
    }

    public void OnLevelUpButtonPressed()
    {
        invUi.ToggleLevelUpMode();
        cursor.ToggleLevelUp();
    }

    public void OnRecycleButtonPressed()
    {
        invUi.ToggleRecycleMode();
        cursor.ToggleRecycle();
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
                OnStatsButtonPressed();
            }

            SwitchToUpgrade();

        }
        else
        {
            if (statsActive)
            {
                OnStatsButtonPressed();
            }

            SwitchToInventory();

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
        InteractInventory();
    }

    public void SwitchToInventory()
    {
        //switching from upgrade to inv
        invUi.DisplayInventory();
        upgradeUi.DismissUpgradeUi();

        focusInv = true;
        switchButtonText.text = "SHOP";
    }

    public void SwitchToUpgrade()
    {
        //switching from inv to upgrade
        upgradeUi.FocusUpgradeUi();
        invUi.DismissInventory();

        focusInv = false;
        switchButtonText.text = "INVENTORY";
    }

    public void EnterInventory()
    {
        // entering inv from gameplay
        World.activeWorld.Pause(true);

        invUi.DisplayInventory();
        hasActiveUpgrade = false;
        focusInv = true;
        switchButtonText.text = "SHOP";

        switchButton.DisableButton();
        continueButton.EnableButton();

        FocusBottomButtons();
    }

    public void EnterUpgrade(Item[] items)
    {
        // entering upgrade from gameplay
        World.activeWorld.Pause(true);

        upgradeUi.SetUpgradeItems(items);
        hasActiveUpgrade = true;
        focusInv = false;
        switchButtonText.text = "INVENTORY";

        continueButton.DisableButton();
        switchButton.EnableButton();

        FocusBottomButtons();
    }

    public void InteractInventory()
    {
        if (checkOutro)
        {
            return;
        }

        if (hasActiveUpgrade)
        {
            OnSwitchButtonPressed();
        }
        else
        {
            if (!interactInventory)
            {
                EnterInventory();
                interactInventory = true;
            }
            else
            {
                interactInventory = false;
                UiClose();
            }
        }
    }

    public void UiClose(bool tookUpgrade = false)
    {
        // continue the game
        if (menuUi.menuActive)
        {
            return;
        }

        if (tookUpgrade)
        {
            hasActiveUpgrade = false;
        }

        if (statsActive)
        {
            statsUi.DismissStats(focusInv);
            statsActive = false;
        }

        if (focusInv)
        {
            invUi.DismissInventory();
        }

        invUi.AllModesOff();
        cursor.AllTogglesOff();
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
        animator.Play("BottomButtonsIntro");
        this.gameObject.SetActive(true);
    }

    public void DismissBottomButtons()
    {
        animator.Play("BottomButtonsOutro");
        checkOutro = true;
    }
}
