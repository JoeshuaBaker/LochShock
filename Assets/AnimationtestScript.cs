using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnimationtestScript : MonoBehaviour
{
    public Image currentSprite;

    public Sprite[] assignedSprites;

    public float animationDuration;

    private float currentAnimTime;
    private float timePerFrame;

    // Update is called once per frame
    void Update()
    {
        currentAnimTime += Time.deltaTime;
        Animate();
    }
    public void Animate()
    {
        if (assignedSprites == null || assignedSprites.Length == 0)
        {
            return;
        }

        currentAnimTime %= animationDuration;

        timePerFrame = animationDuration / assignedSprites.Length;

        currentSprite.sprite = assignedSprites[(int)(currentAnimTime / timePerFrame)];
    }
}
