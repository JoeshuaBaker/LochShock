using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem dangerZonePS;
    public ParticleSystem redRingPS;
    public CircleCollider2D dangerZoneCollider;
    public BoxCollider2D dangerZoneColliderBox;
    public Collider2D activeCollider;
    public ContactFilter2D hitFilter;
    public List<Collider2D> hitBuffer;
    public SpriteRenderer mainSprite;
    public SpriteRenderer fillSprite;
    public SpriteRenderer outlineSprite;
    public Sprite bigCircle;
    public Sprite bigRing;
    public Sprite bigSquare;
    public Sprite bigBox;
    public bool safeOnPlayer;
    public float delay;
    public float damageLocal;
    public bool damageDealt;
    public bool willDealDamage;
    public bool noPS;
    public int pSLowerLim;
    public int pSUpperLim;
    public Gradient redOnePSColor;
    public Gradient redTwoPSColor;
    public Gradient blueOnePSColor;
    public Gradient blueTwoPSColor;
    private static GradientColorKey[] redOneColorArray;
    private static GradientAlphaKey[] redOneAlphaArray;
    private static GradientColorKey[] redTwoColorArray;
    private static GradientAlphaKey[] redTwoAlphaArray;
    private static GradientColorKey[] blueOneColorArray;
    private static GradientAlphaKey[] blueOneAlphaArray;
    private static GradientColorKey[] blueTwoColorArray;
    private static GradientAlphaKey[] blueTwoAlphaArray;

    public CraterCreator craterCreator;
    public int craterSize;


    public void Setup( float damage , float delay , Vector3 position , bool dealsDamage , bool safeOnPlayer , bool noPS , Vector3 scale, bool squareShape, Quaternion rotation, int craterSize)
    {

        if(craterCreator == null)
        {
            craterCreator = World.activeWorld.craterCreator;
        }

        this.craterSize = craterSize;

        animator.SetBool("skip", false);
        this.gameObject.SetActive(true);
        animator.Play(0);

        damageDealt = false;
        willDealDamage = dealsDamage;
        damageLocal = damage;
        this.safeOnPlayer = safeOnPlayer;

        hitBuffer = new List<Collider2D>();
        hitFilter = new ContactFilter2D
        {
            useTriggers = false,
            layerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Player"),
            useLayerMask = true
        };

        transform.rotation = rotation;
        transform.position = position;
        transform.localScale = scale;

        if (squareShape)
        {
            mainSprite.sprite = bigSquare;
            fillSprite.sprite = bigSquare;
            outlineSprite.sprite = bigBox;
            dangerZoneCollider.enabled = false;
            dangerZoneColliderBox.enabled = true;
            activeCollider = dangerZoneColliderBox;
        }
        else
        {
            mainSprite.sprite = bigCircle;
            fillSprite.sprite = bigCircle;
            outlineSprite.sprite = bigRing;
            dangerZoneCollider.enabled = true;
            dangerZoneColliderBox.enabled = false;
            activeCollider = dangerZoneCollider;

        }

        if (delay == 0f)
        {
            animator.SetBool("skip", true);
        }
        else
        {
            if (delay > .5f)
            {
                animator.SetFloat("explosionSpeed", 1f /((delay - 0.5f) * 2f));
            }
            else
            {
                animator.SetBool("skipFirst", true);
            }
        }

        if(redOneColorArray == null)
        {
            redOneColorArray = new GradientColorKey[3];
            redOneAlphaArray = new GradientAlphaKey[3];

            redTwoColorArray = new GradientColorKey[3];
            redTwoAlphaArray = new GradientAlphaKey[3];

            blueOneColorArray = new GradientColorKey[3];
            blueOneAlphaArray = new GradientAlphaKey[3];

            blueTwoColorArray = new GradientColorKey[3];
            blueTwoAlphaArray = new GradientAlphaKey[3];

            for (int i = 0; i < redOneColorArray.Length; i++)
            {
                redOneColorArray[i] = new GradientColorKey(redOnePSColor.colorKeys[i].color, redOnePSColor.colorKeys[i].time);
                redOneAlphaArray[i] = new GradientAlphaKey(redOnePSColor.alphaKeys[i].alpha, redOnePSColor.alphaKeys[i].time);

                redTwoColorArray[i] = new GradientColorKey(redTwoPSColor.colorKeys[i].color, redTwoPSColor.colorKeys[i].time);
                redTwoAlphaArray[i] = new GradientAlphaKey(redTwoPSColor.alphaKeys[i].alpha, redTwoPSColor.colorKeys[i].time);

                blueOneColorArray[i] = new GradientColorKey(blueOnePSColor.colorKeys[i].color, blueOnePSColor.colorKeys[i].time);
                blueOneAlphaArray[i] = new GradientAlphaKey(blueOnePSColor.alphaKeys[i].alpha, blueOnePSColor.alphaKeys[i].time);

                blueTwoColorArray[i] = new GradientColorKey(blueTwoPSColor.colorKeys[i].color, blueTwoPSColor.colorKeys[i].time);
                blueTwoAlphaArray[i] = new GradientAlphaKey(blueTwoPSColor.alphaKeys[i].alpha, blueTwoPSColor.colorKeys[i].time);
            }

        }
    
        this.noPS = noPS;

        var dzPS = dangerZonePS.main;
        var dzPSEmission = dangerZonePS.emission;
        var dzPSColor = dangerZonePS.colorOverLifetime;
        var dzShape = dangerZonePS.shape;

        var rrPS = redRingPS.main;

        if (!noPS)
        {
            redRingPS.Play();

            dangerZonePS.Play();
            float area = scale.x * scale.y;
            dzPSEmission.SetBurst( 0 , new ParticleSystem.Burst(0f, (short) (pSLowerLim * area) , (short) (pSUpperLim * area) , 3 , 0.02f));

            if (squareShape)
            {
                dzShape.shapeType = ParticleSystemShapeType.Rectangle;
                dzShape.scale = new Vector3(3f, 3f, 1f);

                redRingPS.Stop();

            }
            else
            {
                dzShape.shapeType = ParticleSystemShapeType.Circle;
                dzShape.scale = new Vector3(1f, 1f, 1f);
            }

            if (!safeOnPlayer)
            {
                Gradient grad = new Gradient();
                grad.SetKeys(redOneColorArray, redOneAlphaArray);
                Gradient gradTwo = new Gradient();
                gradTwo.SetKeys(redTwoColorArray, redTwoAlphaArray);

                dzPSColor.color = new ParticleSystem.MinMaxGradient(grad, gradTwo);
            }
            else
            {
                Gradient grad = new Gradient();
                grad.SetKeys(blueOneColorArray, blueOneAlphaArray);
                Gradient gradTwo = new Gradient();
                gradTwo.SetKeys(blueTwoColorArray, blueTwoAlphaArray);

                dzPSColor.color = new ParticleSystem.MinMaxGradient(grad, gradTwo);

                redRingPS.Stop();
            }


            if (delay > 0.5f)
            {
                dzPS.startDelay = delay;
                rrPS.startDelay = delay;
            }
            else if (delay == 0f)
            {
                dzPS.startDelay = delay;
                rrPS.startDelay = delay;
            }
            else
            {
                dzPS.startDelay = 0.5f;
                rrPS.startDelay = 0.5f;
            }
        }
        else
        {
            dangerZonePS.Stop();
            redRingPS.Stop();
        }

    }

    void Update()
    {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

        
        if (animState.IsName("DangerZoneFinish") && damageDealt == false && willDealDamage)
        {

                damageDealt = true;

                Physics2D.OverlapCollider(activeCollider, hitFilter, hitBuffer);

                foreach (Collider2D Collider in hitBuffer)
                {
                    Enemy enemy = Collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damageLocal);
                        continue;
                    }
                    Player player = Collider.GetComponent<Player>();
                    if (player != null && !safeOnPlayer)
                    {
                        player.TakeDamageFromEnemy(-1);
                    }
                }


            if(craterSize > 0)
            {
                craterCreator.CreateCrater(this.transform.position, craterSize);
            }
                
           
      
        }
        

        if (animState.IsName("DangerZoneFinish") && animState.normalizedTime >= 1)
        {
            this.gameObject.SetActive(false);
        }

    }
}
