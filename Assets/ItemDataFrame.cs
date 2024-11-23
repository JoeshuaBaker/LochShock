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
    public Color32[] rarityShineColors;
    public Color32[] rarityTextColors;
    public Color32[] raritySmearColors;
    public Color32[] rarityTintColors;
    public TMP_Text itemStatusSlot;
    public TMP_Text itemData;
    public Button topButton;
    public TMP_Text topButtonText;
    public Button bottomButton;
    public TMP_Text bottomButtonText;
    public TMP_Text emptyFrameSlotText;

    public Image raritySmear;
    public Image rarityShine;
    public Image rarityTint;

    public bool playShineEffect;
    public float shineDuration;
    public float shineDurationCurrent;
    public Sprite[] assignedSprites;
    public Sprite[] assignedSpritesTint;
    private float timePerFrame;
    private float timePerFrameTint;

    public Image upgradeEffect;
    public bool decayUpgradeColor;
    public float timeToDecayUpgrade;
    public float upgradeDecayCurrent;

    public TMP_Text stashText;

    public Button cardBackButton;

    public InventorySubUi invUi;
    public UpgradeSubUi upgradeUi;

    public GameObject contextMessageParent;
    public TMP_Text contextMessageText;
    public bool playContextMessage;
    public float contextMessageDuration;
    public float contextMessageDurationCurrent;
    public Color32[] contextMessageColors;
    public float colorPulseDuration;
    public float colorPulseDurationCurrent;
    public bool pulseUp;

    public bool newFrame;

    // Start is called before the first frame update
    void Start()
    {
        if(item != null)
        {
            slotType = item.itemType;
            ReflectInventoryState(inventoryUIState, item);
        }
    }


    void Update()
    {
        if (decayUpgradeColor)
        {
            var upgradeColor = upgradeEffect.color;

            upgradeColor.a = Mathf.Lerp(upgradeColor.a, 0f, upgradeDecayCurrent);

            upgradeDecayCurrent += Time.unscaledDeltaTime;

            if(upgradeDecayCurrent >= timeToDecayUpgrade)
            {
                decayUpgradeColor = false;
                upgradeDecayCurrent = 0f;
                upgradeEffect.gameObject.SetActive(false);
            }

            upgradeEffect.color = upgradeColor;
        }

        if (playShineEffect)
        {
            if (assignedSprites == null || assignedSprites.Length == 0)
            {
                return;
            }

            shineDurationCurrent %= shineDuration;

            timePerFrame = shineDuration / assignedSprites.Length;
            timePerFrameTint = shineDuration / assignedSpritesTint.Length;

            rarityShine.sprite = assignedSprites[(int)(shineDurationCurrent / timePerFrame)];
            rarityTint.sprite = assignedSpritesTint[(int)(shineDurationCurrent / timePerFrameTint)];

            shineDurationCurrent += Time.unscaledDeltaTime;

            if(shineDurationCurrent > shineDuration)
            {
                playShineEffect = false;
                rarityShine.gameObject.SetActive(false);
                shineDurationCurrent = 0f;
            }
        }

        if (playContextMessage)
        {
            contextMessageDurationCurrent += Time.unscaledDeltaTime;

            if (pulseUp)
            {
                colorPulseDurationCurrent += Time.unscaledDeltaTime;
            }
            else
            {
                colorPulseDurationCurrent -= Time.unscaledDeltaTime;
            }

            Color32 currentColor = Color.Lerp(contextMessageColors[0], contextMessageColors[1], colorPulseDurationCurrent);

            contextMessageText.color = currentColor;

            if(colorPulseDurationCurrent> colorPulseDuration)
            {
                pulseUp = false;
            }
            if(colorPulseDurationCurrent <= 0f)
            {
                pulseUp = true;
            }

            if (contextMessageDurationCurrent >= contextMessageDuration)
            {
                contextMessageParent.SetActive(false);
                playContextMessage = false;
            }
        }
    }

    public void SetItem(Item item)
    {
        this.item = item;

        if (!newFrame)
        {
            topButton.interactable = item != null;
            bottomButton.interactable = item != null;
        }

        if (item == null)
        {
            fullFrameParent.gameObject.SetActive(false);

            if (isStash)
            {
                emptyFrameSlotText.text = $"STASH SLOT";
            }
            else
            {
                emptyFrameSlotText.text = slotType.ToString() + "\nSLOT";
                emptyFrameSlotText.text = emptyFrameSlotText.text.ToUpper();
            }

            return;
        }
        else
        {
            fullFrameParent.gameObject.SetActive(true);
        }

        icon.sprite = item.icon;
        if ((int)item.rarity < frameRarities.Length)
        {
            itemFrame.sprite = frameRarities[(int)item.rarity];
        }
        itemName.text = item.DisplayName;
        itemName.text = itemName.text.ToUpper();

        itemLevel.text = item.level.ToString();

        itemRarity.text = item.rarity.ToString();
        itemRarity.text = "<alpha=#88>" + itemRarity.text.ToUpper();

        Debug.Log("setitem");

        if (newFrame)
        {
            itemRarity.color = rarityTextColors[(int)item.rarity];
            raritySmear.color = raritySmearColors[(int)item.rarity];
            rarityShine.color = rarityShineColors[(int)item.rarity];
            rarityTint.color = rarityTintColors[(int)item.rarity];

            Debug.Log(item.rarity);

            if (isStash)
            {
                itemRarity.gameObject.SetActive(false);
                stashText.gameObject.SetActive(true);

                invDataFrameImage.color = Color.grey;
            }
            else
            {
                itemRarity.gameObject.SetActive(true);
                stashText.gameObject.SetActive(false);

                invDataFrameImage.color = Color.white;
            }
        }

        itemStatusSlot.text = item.itemType.ToString();
        itemStatusSlot.text = itemStatusSlot.text.ToUpper();

        if(!topButtonText.text.Contains("Take") && !topButtonText.text.Contains("Unstash") && !newFrame)
        {
            topButton.interactable = item.levelUpCost > 0 && Player.activePlayer.inventory.scrap >= item.levelUpCost;
        }

        topButtonText.text = item.levelUpCost.ToString();
        bottomButtonText.text = item.disassembleValue.ToString();

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

        if (playContextMessage)
        {
            playContextMessage = false;
            contextMessageDurationCurrent = 0f;
            contextMessageParent.SetActive(false);
        }
    }

    public void ReflectInventoryState(InventoryUI.InventoryUIState state, Item item)
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
                    emptyFrameSlotText.text = $"STASH SLOT";
                }
                else
                {
                    emptyFrameSlotText.text = slotType.ToString() + "\nSLOT";
                    emptyFrameSlotText.text = emptyFrameSlotText.text.ToUpper();
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


    public void PlayContextMessage(String message, float duration = 1f)
    {
        contextMessageText.text = message;
        playContextMessage = true;

        contextMessageDuration = duration;
        contextMessageDurationCurrent = 0f;

        contextMessageParent.SetActive(true);
    }

    public void PlayCardShake()
    {
        flyinAnimator.Play("UiItemLevelUp",0,0f);
    }

    public void PlayCardIntro(float maxDelay = 0f, bool playShineEffect = false)
    {
        flyinAnimator.Play("UiItemIntro", 0, UnityEngine.Random.Range(maxDelay, 0f));

        if (playShineEffect)
        {
            PlayShineEffect();
        }
    }

    public void PlayCardOutro(float maxDelay = 0f)
    {
        flyinAnimator.Play("UiItemOutro", 0, UnityEngine.Random.Range(maxDelay, 0f));
    }

    public void PlayCardDrop()
    {
        flyinAnimator.Play("UiItemDrop");
    }

    public void PlayShineEffect()
    {
        if(item != null)
        {
            rarityShine.color = rarityShineColors[(int)item.rarity];
            rarityTint.color = rarityTintColors[(int)item.rarity];
        }
        else
        {
            var rand = UnityEngine.Random.Range(0, 5);
            rarityShine.color = rarityShineColors[rand];
            rarityTint.color = rarityTintColors[rand];
        }
        playShineEffect = true;
        shineDurationCurrent = 0f;

        rarityShine.gameObject.SetActive(true);
    }

    public void PlayUpgradeEffect()
    {
        upgradeEffect.gameObject.SetActive(true);

        var upgradeColor = upgradeEffect.color;

        upgradeColor.a = 1f;
        upgradeEffect.color = upgradeColor;
        decayUpgradeColor = true;

        upgradeDecayCurrent = 0f;
    }

    public void PlayRecycleEffect()
    {

    }

    public void OnLevelUpButtonPressed()
    {
        if (invUi != null)
        {
            invUi.AttemptUpgradeItem(this);
        }
    }

    public void OnRecycleButtonPressed()
    {
        if(invUi != null)
        {
            invUi.AttemptRecycleItem(this);
        }
    }

    public void OnBaseButtonPressed()
    {
        if(upgradeUi != null)
        {
            upgradeUi.Take(this);
        }
    }
}
