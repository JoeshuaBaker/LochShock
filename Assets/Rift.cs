using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rift : MonoBehaviour
{
    public SpriteRenderer riftMaskSprite;
    public SpriteMask riftMask;

    void Start()
    {

    }

    void Update()
    {

        riftMask.sprite = riftMaskSprite.sprite;
     
    }
}
