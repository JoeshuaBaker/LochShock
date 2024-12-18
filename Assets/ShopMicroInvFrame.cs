using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopMicroInvFrame : MonoBehaviour
{
    public GameObject frameVis;
    public Image glow;
    public Image frame;
    public TMP_Text level;
    public Image icon;
    public float lockAlpha = 0.03f;

    public Item item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetItem(Item item, Color color, Sprite frame)
    {
        this.item = item;
        float alpha = glow.color.a;
        glow.color = color;

        Color glowNoAlpha = glow.color;
        glowNoAlpha.a = alpha;
        glow.color = glowNoAlpha;

        if (item == null)
        {
            frameVis.SetActive(false);
        }
        else
        {
            frameVis.SetActive(true);

            icon.sprite = item.icon;
            level.text = item.level.ToString();

            if (item.lockCombine)
            {
                glow.color = Color.red;
                Color alphaColor = glow.color;
                alphaColor.a = lockAlpha;
                glow.color = alphaColor;
            }
        }
        this.frame.sprite = frame;
    }

    public void SetColor()
    {
        Color color = glow.color;
        color.a = .6f;
        glow.color = color;
    }
}
