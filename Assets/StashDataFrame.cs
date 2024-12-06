using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StashDataFrame : MonoBehaviour, IDataFrame
{
    public InventorySubUi invUi;

    public Animator animator;

    public GameObject fullFrameParent;
    public GameObject emptyFrameParent;

    public Item item;
    public Image itemFrame;
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemLevel;
    public Sprite[] frameRarities;

    public GameObject tabParent;
    public TMP_Text leftTabText;
    public TMP_Text rightTabText;
    public Button cardBackButton;

    public Image upgradeEffect;
    public bool decayUpgradeColor;
    public float timeToDecayUpgrade;
    public float upgradeDecayCurrent;

    public GameObject contextMessageParent;
    public TMP_Text contextMessageText;
    public bool playContextMessage;
    public float contextMessageDuration;
    public float contextMessageDurationCurrent;
    public Color32[] contextMessageColors;
    public float colorPulseDuration;
    public float colorPulseDurationCurrent;
    public bool pulseUp;

    public bool decayRecycleColor;
    public Image recycleEffect;
    public float recycleDecayCurrent;
    public float timeToDecayRecycle = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (decayRecycleColor)
        {
            var recycleColor = recycleEffect.color;

            recycleColor.a = Mathf.Lerp(recycleColor.a, 0f, recycleDecayCurrent);

            recycleDecayCurrent += (Time.unscaledDeltaTime * 2f);

            if (recycleDecayCurrent >= timeToDecayRecycle)
            {
                decayRecycleColor = false;
                recycleDecayCurrent = 0f;
                recycleEffect.gameObject.SetActive(false);
            }

            recycleEffect.color = recycleColor;
        }

        if (decayUpgradeColor)
        {
            var upgradeColor = upgradeEffect.color;

            upgradeColor.a = Mathf.Lerp(upgradeColor.a, 0f, upgradeDecayCurrent);

            upgradeDecayCurrent += Time.unscaledDeltaTime;

            if (upgradeDecayCurrent >= timeToDecayUpgrade)
            {
                decayUpgradeColor = false;
                upgradeDecayCurrent = 0f;
                upgradeEffect.gameObject.SetActive(false);
            }

            upgradeEffect.color = upgradeColor;
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

            if (colorPulseDurationCurrent > colorPulseDuration)
            {
                pulseUp = false;
            }
            if (colorPulseDurationCurrent <= 0f)
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

        if(item == null)
        {
            emptyFrameParent.SetActive(true);
            fullFrameParent.SetActive(false);
            return;
        }
        else
        {
            emptyFrameParent.SetActive(false);
            fullFrameParent.SetActive(true);
        }

        itemIcon.sprite = item.icon;
        if ((int)item.rarity < frameRarities.Length)
        {
            itemFrame.sprite = frameRarities[(int)item.rarity];
        }
        itemName.text = item.DisplayName;
        itemName.text = itemName.text.ToUpper();

        itemLevel.text = item.level.ToString();
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
        animator.Play("StashFrameShake",0,0f);
    }

    public void PlayCardIntro(float maxDelay = 0f, bool playShineEffect = false)
    {
        animator.Play("StashFrameIntro", -1, UnityEngine.Random.Range(-0.5f, 0f));
    }

    public void PlayCardOutro(float maxDelay = 0f)
    {
        animator.Play("StashFrameOutro", 0, maxDelay);
    }

    public void PlayUpgradeEffect()
    {
        upgradeEffect.gameObject.SetActive(true);

        var upgradeColor = upgradeEffect.color;

        upgradeColor.a = 1f;
        upgradeEffect.color = upgradeColor;
        decayUpgradeColor = true;

        upgradeDecayCurrent = 0f;

        UpdateTabText();
    }

    public void PlayRecycleEffect()
    {
        recycleEffect.gameObject.SetActive(true);

        var recycleColor = recycleEffect.color;

        recycleColor.a = 1f;
        recycleEffect.color = recycleColor;
        decayRecycleColor = true;

        recycleDecayCurrent = 0f;
    }

    public void OnBaseButtonPressed()
    {

       if (invUi != null)
       {
            invUi.CardClicked(this);
       }
    }

    public void OnSelect()
    {
        if (invUi != null)
        {
            UpdateTabText();

            if (invUi.recycleMode || invUi.levelUpMode)
            {
                tabParent.SetActive(true);
            }
        }
    }

    public void OnPointerEnter()
    {
        if (cardBackButton.interactable == true)
        {
            cardBackButton.Select();
        }
    }

    public void OnPointerExit()
    {
        if (EventSystem.current.currentSelectedGameObject == cardBackButton.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnDeselect()
    {
        if (invUi != null)
        {
            tabParent.SetActive(false);
        }
    }

    public void UpdateTabText()
    {
        if (invUi.recycleMode)
        {
            //TODO items giving/taking upgrade kits
            //rightTabText.text = "+" + item.upgradeKitsOnRecycle.ToString();

            leftTabText.text = "+" + item.disassembleValue.ToString();
            rightTabText.text = "0";

            leftTabText.color = Color.white;
            rightTabText.color = Color.white;
        }
        else if (invUi.levelUpMode)
        {
            //TODO items giving/taking upgrade kits
            //leftTabText.text = "-" + item.disassembleValue.ToString();
            //rightTabText.text = "-" + item.upgradeKitsToLevel.ToString();

            leftTabText.text = "-" + item.levelUpCost.ToString();
            rightTabText.text = "0";

            leftTabText.color = Color.red;
            rightTabText.color = Color.red;
        }
    }

    public Item GetItem()
    {
        return item;
    }
}
