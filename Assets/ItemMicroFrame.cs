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

    public Sprite[] frameRarities;

    public GameplayUI gameplayUI;
    public float grappleCD;
    public float grappleCDMax;

    public float animXDistance = -200f;
    public bool playAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(item == null && !isGrapple)
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

            if(grappleCD > 0f)
            {
                topText.gameObject.SetActive(true);
                topText.text = grappleCD.ToString("0.0");
                var distancePercent = grappleCD / grappleCDMax;
                itemGlow.color = itemGlowRed;
                itemGlow.rectTransform.localPosition = new Vector3(animXDistance * distancePercent, 0f, 0f);
                playAnim = true;
                
            }
            else
            {

                itemGlow.rectTransform.localPosition = Vector3.zero;
                if (playAnim)
                {
                    itemGlow.color = itemGlowBlue;
                    topText.gameObject.SetActive(false);
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
                topText.text = $"{(gun.magazine)} / {(gun.maxMagazine)}";
                topText.gameObject.SetActive(true);

                if(gun == gameplayUI.activeGun)
                {
                    itemGlow.gameObject.SetActive(true);
                }
                else
                {
                    itemGlow.gameObject.SetActive(false);
                }

            }
            if(item is ActiveItem)
            {
                ActiveItem activeItem = item as ActiveItem;
                topText.text = activeItem.cooldownTimer.ToString("0.0");

                if (activeItem.cooldownTimer > 0f)
                {
                    itemGlow.color = itemGlowRed;
                    topText.gameObject.SetActive(true);
                    var distancePercent = activeItem.cooldownTimer / activeItem.cooldown;
                    itemGlow.rectTransform.localPosition = new Vector3(animXDistance * distancePercent, 0f, 0f);
                    playAnim = true;
                }
                else
                {
                    itemGlow.gameObject.SetActive(true);
                    itemGlow.rectTransform.localPosition = Vector3.zero;
                    if (playAnim)
                    {
                        itemGlow.color = itemGlowBlue;
                        topText.gameObject.SetActive(false);
                        itemGlow.gameObject.SetActive(false);
                        itemGlow.gameObject.SetActive(true);
                        playAnim = false;
                    }
                    
                }
            }
            if (!(item is Gun) && !(item is ActiveItem))
            {
                itemIcon.sprite = item.icon;
                itemRarityFrame.sprite = frameRarities[(int)item.rarity];
                itemIcon.gameObject.SetActive(true);
            }
        }


    }


}
