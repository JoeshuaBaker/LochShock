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
    public Material matInstance;
    public Material lightningMat;
    public SpriteRenderer borderSprite;
    public SpriteRenderer heartSprite;
    public ParticleSystem crossPS;
    public ParticleSystem crossPS2;


    public GameObject scaleParent;
    public float scaleMin = 1f;
    public float scaleMax = 1.4f;
    public float fadeScale;
    public float fadeTime;
    public float fadeTimeCurrent;
    public bool fadeEnd;
    public float scaleMaxRange = 3f;
    public GameObject lightningParent;
    public float lightningMaxRange = 10f;
    public float lightningMinRange = 1f;
    public float lightningMaxSize = 0.6f;
    public float lightningMinSize = 0.1f;
    public float bobAmount;

    public float distToPlayer;
    public float collectRange;
    public bool collected;

    public Vector3 scaleTotal;

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

        //Aduio Section
        AkSoundEngine.PostEvent("PlayHealthWallLoop", this.gameObject);
    }

    private void OnDestroy()
    {
        mat.SetFloat("PlayerPos", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = this.transform.position.x - Player.activePlayer.transform.position.x;

        Position();

        if (collected)
        {
            if (!fadeEnd)
            {
                Fade();
            }

        }
        else
        {
            if (distToPlayer < collectRange)
            {
                Collect();
            }

            if (distToPlayer <= lightningMaxRange)
            {
                Scale();
            }
        }

        Animate();

      
        mat.SetFloat("PlayerPos", (Player.activePlayer.transform.position.y *.25f));

        //Audio Section
        AkSoundEngine.SetRTPCValue("DistanceFromHealthWall", distToPlayer);
        ControlAudioPan("HealthWallSpeakerPan_LR");
    }

    public void Animate()
    {
        if (assignedSprites == null || assignedSprites.Length == 0)
        {
            return;
        }

        if (useHoldFrame && sprite.sprite == assignedSprites[holdFrame])
        {
            currentHoldTime += Time.deltaTime;

            if (currentHoldTime >= holdTime)
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

    }

    public void Scale()
    {
        var scale = lightningParent.transform.localScale;

        scale.x = Mathf.Lerp(lightningMaxSize, lightningMinSize, ( Mathf.Max(distToPlayer, lightningMinRange) / lightningMaxRange));

        lightningParent.transform.localScale = scale;

        if(distToPlayer < scaleMaxRange)
        {
            scaleTotal = scaleParent.transform.localScale;

            scaleTotal.x = Mathf.Lerp(scaleMax, scaleMin, (Mathf.Max(distToPlayer, collectRange) / scaleMaxRange));

            scaleParent.transform.localScale = scaleTotal;

            var em = crossPS.emission;
            var em2 = crossPS2.emission;

            em.rateOverTime = 20;
            em2.rateOverTime = 20;
        }

    }

    public void Position()
    {
        var pos = transform.position;
        var scalePos = scaleParent.transform.localPosition;

        pos.y = Player.activePlayer.transform.position.y;
        scalePos.y = Mathf.Abs(Mathf.Sin(Time.time * 2f) * bobAmount);

        transform.position = pos;
        scaleParent.transform.localPosition = scalePos;
    }

    public void Collect()
    {
        collected = true;
        crossPS.Emit(40);
        crossPS.Stop();
        crossPS2.Emit(40);
        crossPS2.Stop();
        Player.activePlayer.UpdateHp(1);
        World.activeWorld.lightningBolt.CallLightning(Player.activePlayer.transform.position);
        lightningParent.SetActive(false);

        //Audio Section
        AkSoundEngine.PostEvent("PlayHealthWallCollect", this.gameObject);
    }

    public void Fade()
    {
        fadeTimeCurrent += Time.deltaTime *3f;

        Color color = borderSprite.color;

        color.a = Mathf.Lerp(1f, 0f, fadeTimeCurrent);

        borderSprite.color = color;
        heartSprite.color = color;

        var scale = scaleParent.transform.localScale;

        scale.x = scale.x * (1f - (6f * Time.deltaTime));

        scaleParent.transform.localScale = scale;

        if(color.a <= 0f)
        {
            fadeEnd = true;
        }
    }

    //Audio Section
    //Controls audio pan for health wall
    public void ControlAudioPan(string RTPCname)
    {
        //Audio Section
        //Sound is coming from Left of player
        if (this.gameObject.transform.position.x < Player.activePlayer.transform.position.x)
        {
            AkSoundEngine.SetRTPCValue(RTPCname, 0 - Vector3.Distance(Player.activePlayer.transform.position, this.gameObject.transform.position));
        }
        //Sound is coming from right of player
        else if (this.gameObject.transform.position.x > Player.activePlayer.transform.position.x)
        {
            AkSoundEngine.SetRTPCValue(RTPCname, Vector3.Distance(Player.activePlayer.transform.position, this.gameObject.transform.position));
        }
        return;
    }

}
