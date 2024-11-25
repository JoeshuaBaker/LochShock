using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTape : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Sprite[] assignedSprites;
    public float animationDuration;
    public float currentAnimTime;
    public float timePerFrame;

    public bool useHoldFrame;
    public float holdTime;
    public float currentHoldTime;
    public int holdFrame;

    public Material mat;
    public Renderer rend;


    // Start is called before the first frame update
    void Start()
    {
        if (assignedSprites == null || assignedSprites.Length == 0)
        {
            return;
        }
        else
        {
            timePerFrame = animationDuration / assignedSprites.Length;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (assignedSprites == null || assignedSprites.Length == 0)
        {
            return;
        }

        if(useHoldFrame && sprite.sprite == assignedSprites[holdFrame])
        {
            currentHoldTime += Time.deltaTime;
            
            if(currentHoldTime >= holdTime)
            {
                sprite.sprite = assignedSprites[holdFrame + 1];

                currentHoldTime -= holdTime;

                currentAnimTime += currentHoldTime + timePerFrame;

                currentHoldTime = 0f;
            }
        }
        else
        {
            currentAnimTime += Time.deltaTime;

            currentAnimTime %= animationDuration;

            sprite.sprite = assignedSprites[(int)(currentAnimTime / timePerFrame)];
        }

        //Vector2 pos = new Vector2(0f, Player.activePlayer.transform.position.y);

        //Vector2 test = new Vector2(Time.time, Time.time);

        //var offset = rend.material.mainTextureOffset;

        //offset.y = Time.time;

        //rend.material.mainTextureOffset = new Vector2( 0f , (Player.activePlayer.transform.position.y *.2f));

        //rend.material.SetTextureOffset("_MainTex", new Vector2(0f, Time.time));


        mat.SetFloat("PlayerPos", (Player.activePlayer.transform.position.y *.25f));
    }


}
