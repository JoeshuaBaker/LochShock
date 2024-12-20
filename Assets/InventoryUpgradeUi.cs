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

    public bool uiIsActive;
    public bool focusInv;
    public TMP_Text switchButtonText;

    public bool levelPreview;

    public TMP_Text heldScrap;
    public TMP_Text heldUpgradeKits;

    public bool statsActive;
    public bool menuActive;

    public MenuButtonEventHandler switchButton;
    public MenuButtonEventHandler continueButton;

    public bool checkOutro;
    public Animator animator;

    public Action OnClose;

    public bool checkUpgrades;
    public float timeBetweenUpgradeCheck;
    public float timeBetweenUpgradeCheckCurrent;

    public float shimmerScrollTime;
    public Material shimmerMat;

    public bool setUp;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (checkUpgrades)
        {
            if (checkOutro)
            {
                checkUpgrades = false;
                inventory.CombineDuplicateItems();
                return;
            }

            timeBetweenUpgradeCheckCurrent += Time.unscaledDeltaTime;
            if(timeBetweenUpgradeCheckCurrent >= timeBetweenUpgradeCheck)
            {
                timeBetweenUpgradeCheckCurrent = 0f;
                checkUpgrades = false;
                inventory.CombineDuplicateItems();
            }
        }

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

                    uiIsActive = false;

                    invUi.gameObject.SetActive(false);

                    this.gameObject.SetActive(false);
                }
            }
        }

        shimmerScrollTime += Time.unscaledDeltaTime;
        shimmerMat.SetFloat("_UnscaledTime", shimmerScrollTime);
    }

    public void SetUp()
    {
        if (inventory == null)
        {
            inventory = Player.activePlayer.inventory;
        }
        setUp = true;
    }

    private void OnDestroy()
    {
        shimmerMat.SetFloat("_UnscaledTime", 0f);
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
            statsUi.FocusStats();
            statsActive = true;
        }
        else
        {
            statsUi.DismissStats();
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
        focusInv = true;
        switchButtonText.text = "SHOP";

        uiIsActive = true;

        switchButton.DisableButton();

        FocusBottomButtons();

        UpdateScrapAmount();
    }

    public void EnterUpgrade(Item[] items, int gainedScrap)
    {
        // entering upgrade from gameplay
        World.activeWorld.Pause(true);

        upgradeUi.SetUpgradeItems(items, gainedScrap);
        focusInv = false;
        switchButtonText.text = "INVENTORY";

        uiIsActive = true;

        switchButton.EnableButton();

        FocusBottomButtons();

        UpdateScrapAmount();
    }

    public void InteractInventory()
    {
        if (checkOutro)
        {
            return;
        }

        if (uiIsActive)
        {
            UiClose();
        }
        else
        {
            EnterInventory();

        }
    }

    public void UiClose()
    {
        // continue the game
        if (menuUi.menuActive)
        {
            return;
        }

        if (statsActive)
        {
            statsUi.DismissStats();
            statsActive = false;
        }

        if (focusInv)
        {
            invUi.DismissInventory();
        }
        else
        {
            upgradeUi.DismissUpgradeUi();
        }


        invUi.AllModesOff();
        cursor.AllTogglesOff();
        DismissBottomButtons();
    }

    public void PlayCardEffects(int upgraded = -1, int destroyed = -1)
    {
        if (focusInv)
        {
            invUi.PlayCardEffects(upgraded, destroyed);
        }
        else
        {
            upgradeUi.PlayCardEffects(upgraded, destroyed);
        }

        CheckUpgrades();
    }

    public void CheckUpgrades()
    {
        checkUpgrades = true;
    }

    public void UpdateScrapAmount()
    {
        if (!setUp)
        {
            SetUp();
        }

        if(inventory != null)
        {
            heldScrap.text = $"x{inventory.scrap.ToString()}";
            heldUpgradeKits.text = $"x{inventory.upgradeKits.ToString()}";

            Player.activePlayer.gameplayUI.SetMoney(inventory.scrap);
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
