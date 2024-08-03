using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem dangerZonePS;
    public CircleCollider2D dangerZoneCollider;
    public ContactFilter2D hitFilter;
    public List<Collider2D> hitBuffer;
    public bool safeOnPlayer;
    public float delay;
    public float damageLocal;
    public Vector3 scale;
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


    public void Setup( float damage , float delay , Vector3 position , bool dealsDamage , bool safeOnPlayer , bool noPS , Vector3 scale)
    {
        animator.SetBool("skip", false);
        animator.Play(0);

        damageDealt = false;

        willDealDamage = dealsDamage;

        this.gameObject.SetActive(true);

        hitBuffer = new List<Collider2D>();
        hitFilter = new ContactFilter2D
        {
            useTriggers = false,
            layerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Player"),
            useLayerMask = true
        };

        this.transform.position = position;
        this.scale = scale;
        this.transform.localScale = scale;

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

        this.damageLocal = damage;

        this.safeOnPlayer = safeOnPlayer;

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

        var dzPSPlayStop = dangerZonePS;
        var dzPS = dangerZonePS.main;
        var dzPSEmission = dangerZonePS.emission;
        var dzPSColor = dangerZonePS.colorOverLifetime;

        if (!noPS)
        {
            dangerZonePS.Play();
            float area = Mathf.PI * scale.x * scale.x * .33f;
            dzPSEmission.SetBurst( 0 , new ParticleSystem.Burst(0f, (short) (pSLowerLim * area) , (short) (pSUpperLim * area) , 3 , 0.02f));

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
            }


            if (delay > 0.5f)
            {
                dzPS.startDelay = delay;
            }
            else if (delay == 0f)
            {
                dzPS.startDelay = delay;
            }
            else
            {
                dzPS.startDelay = 0.5f;
            }
        }
        else
        {
            dangerZonePS.Stop();
        }

        

    

    }

    void Update()
    {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

        if (willDealDamage)
        {

            if (animState.IsName("DangerZoneFinish") && damageDealt == false)
            {

                damageDealt = true;

                Physics2D.OverlapCollider(dangerZoneCollider, hitFilter, hitBuffer);

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

            }
        }

        if (animState.IsName("DangerZoneFinish") && animState.normalizedTime >= 1)
        {
            this.gameObject.SetActive(false);
        }

    }
}
