using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemMicroFrame : MonoBehaviour
{
    public Item item;
    public bool isGrapple;

    public Image itemIcon;
    public Image itemRarityFrame;
    public Image itemGlow;
    public Color itemGlowBlue;
    public Color itemGlowDullBlue;
    public Color itemGlowRed;

    public Animator glowAnimator;

    public TMP_Text topText;
    public TMP_Text bottomText;

    public TMP_Text activeItemChargeText;
    public TMP_Text activeItemChargeTextShadow;

    public Sprite[] frameRarities;

    public GameplayUI gameplayUI;
    public float grappleCD;
    public float grappleCDMax;

    public float animXDistance = -200f;
    public bool playAnim;

    public Image grappleNub;
    public Image activeNub;

    public Sprite nubFull;
    public Sprite nubFilling;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (item == null && !isGrapple)
        {

            itemRarityFrame.sprite = frameRarities[0];

            topText.gameObject.SetActive(false);
            bottomText.gameObject.SetActive(false);
            itemGlow.gameObject.SetActive(false);
            itemIcon.gameObject.SetActive(false);

        }
        else if (isGrapple)
        {
            itemRarityFrame.sprite = frameRarities[1];

            bottomText.gameObject.SetActive(false);
            itemIcon.gameObject.SetActive(true);

            grappleCD = gameplayUI.grappleCD;
            grappleCDMax = gameplayUI.grappleCDMax;

            if (grappleCD > 0f)
            {
                topText.gameObject.SetActive(true);
                topText.text = grappleCD.ToString("0.0");
                var distancePercent = grappleCD / grappleCDMax;

                grappleNub.fillAmount = 1f - distancePercent;
                grappleNub.color = new Color(1f, 1f, 1f, 0.1f + (1f - distancePercent) * .2f);
                grappleNub.sprite = nubFilling;

                itemGlow.color = itemGlowRed;
                itemGlow.rectTransform.localPosition = new Vector3(animXDistance * (1f - distancePercent), 0f, 0f);
                playAnim = true;

            }
            else
            {

                itemGlow.rectTransform.localPosition = Vector3.zero;
                if (playAnim)
                {
                    grappleNub.fillAmount = 1f;
                    grappleNub.color = new Color(1f, 1f, 1f, 1f);
                    grappleNub.sprite = nubFull;

                    itemGlow.color = itemGlowBlue;
                    topText.text = "READY";
                    topText.gameObject.SetActive(true);
                    itemGlow.gameObject.SetActive(false);
                    itemGlow.gameObject.SetActive(true);
                    playAnim = false;
                }
            }

        }
        else
        {
            itemIcon.sprite = item.icon;
            itemRarityFrame.sprite = frameRarities[(int)item.rarity];
            itemIcon.gameObject.SetActive(true);

            if (item is Gun)
            {
                Gun gun = item as Gun;
                topText.text = $"{(gun.magazine)}/{(gun.maxMagazine)}";
                topText.gameObject.SetActive(true);

                if (gun == gameplayUI.activeGun)
                {
                    itemGlow.gameObject.SetActive(true);
                }
                else
                {
                    itemGlow.gameObject.SetActive(false);
                }

            }
            if (item is ActiveItem)
            {
                ActiveItem activeItem = item as ActiveItem;
               

                if (activeItem.CooldownTimer > 0f && activeItem.currentCharges == 0)
                {
                    itemGlow.color = itemGlowRed;
                    topText.gameObject.SetActive(true);
                    var distancePercent = (1f - activeItem.percentCooldownComplete);

                    topText.text = activeItem.CooldownCountdown.ToString("0.0") + (activeItem.MaxCharges > 1 && activeItem.currentCharges > 0 ? $" ({activeItem.currentCharges})" : "");

                    activeNub.fillAmount = 1f - distancePercent;
                    activeNub.color = new Color(1f, 1f, 1f, 0.1f + (1f - distancePercent) * .2f);
                    activeNub.sprite = nubFilling;

                    itemGlow.rectTransform.localPosition = new Vector3(animXDistance * (1f - distancePercent), 0f, 0f);
                    playAnim = true;
                }
                else
                {
                    topText.gameObject.SetActive(true);

                    activeNub.fillAmount = 1f;
                    activeNub.color = new Color(1f, 1f, 1f, 1f);
                    activeNub.sprite = nubFull;

                    if (activeItem.MaxCharges <= 1 | activeItem.currentCharges == activeItem.MaxCharges)
                    {
                        if(activeItem.MaxCharges == 1)
                        {
                            topText.text = $"READY";
                        }
                        else
                        {
                            topText.text = $"READY" + $"({ activeItem.currentCharges})";
                        }

                    }
                    else
                    {
                        topText.text = activeItem.CooldownCountdown.ToString("0.0") + (activeItem.MaxCharges > 1 && activeItem.currentCharges > 0 ? $" ({activeItem.currentCharges})" : "");
                    }

                    itemGlow.gameObject.SetActive(true);
                    itemGlow.rectTransform.localPosition = Vector3.zero;
                    if (playAnim)
                    {
                        itemGlow.color = itemGlowBlue;
                        itemGlow.gameObject.SetActive(false);
                        itemGlow.gameObject.SetActive(true);
                        playAnim = false;
                    }

                }
                if(activeItem.MaxCharges > 1)
                {
                    activeItemChargeText.text = $"{activeItem.currentCharges}";
                    activeItemChargeTextShadow.text = $"{activeItem.currentCharges}";
                    activeItemChargeText.gameObject.SetActive(true);
                    activeItemChargeTextShadow.gameObject.SetActive(true);
                }
                else
                {
                    activeItemChargeText.gameObject.SetActive(false);
                    activeItemChargeTextShadow.gameObject.SetActive(false);
                }
           

             

            }
            if (!(item is Gun) && !(item is ActiveItem))
            {
                itemIcon.sprite = item.icon;
                itemRarityFrame.sprite = frameRarities[(int)item.rarity];
                itemIcon.gameObject.SetActive(true);

                if (item.buffs != null && item.buffs.Count > 0)
                {
                    if (item.buffs[0].currentDuration > 0f)
                    {
                        itemGlow.color = itemGlowBlue;
                        itemGlow.gameObject.SetActive(true);
                        topText.gameObject.SetActive(true);
                        topText.text = item.buffs[0].currentDuration.ToString("0.0");
                    }
                    else
                    {
                        itemGlow.gameObject.SetActive(false);
                        topText.gameObject.SetActive(false);
                    }

                }
            }
        }


    }



    //lol

}
