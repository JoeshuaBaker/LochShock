using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class EnvironmentalEffectController : MonoBehaviour
{
    public Light2D initialLight;
    public bool initialLightDestroyed;
    [SerializeField]
    private float startingIntensity;
    public Light2D lightningLight;
    public Animator lightningAnimator;
    [Header("Lightning Attributes")]
    [Tooltip("% chance for lightning per frame")]
    public float baseLightningChance;
    public float baseLightningMax;
    public float lightningStrikeChance;
    public float lightningStrikeMax;
    public float lightningLargeChance;
    public float lightningLargeMax;
    public float lightningMediumChance;
    public float lightningMediumMax;
    public float isLightningChanceScaling;
    public float isLightningStrikeScaling;
    public float isLightningLargeScaling;
    public float isLightningMediumScaling;
    public float isBaseLightningChanceScaling;
    [Header("Rain")]
    public ParticleSystem rain;
    [Header("Time")]
    public float gameTime;
    public float maxTimeScaling;
    public float timeSinceOrbUsed;
    public float orbResetTime;
    public StatBlock stats;

    // Start is called before the first frame update
    void Start()
    {
        initialLight.intensity = startingIntensity;
        stats = Player.activePlayer.stats;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceOrbUsed = (timeSinceOrbUsed + Time.deltaTime) % orbResetTime;
        gameTime = Time.timeSinceLevelLoad;
        UpdateInitialLight();
        UpdateLightningLight();
        UpdateRain();

    }
    private void UpdateInitialLight()
    {
        if (!initialLightDestroyed)
        {
            float intensityTimePast;
            intensityTimePast = startingIntensity - (0.005f * gameTime);

            if (intensityTimePast < 0)
            {
                Object.Destroy(initialLight.gameObject);
                initialLightDestroyed = true;
            }
            else
            {
                initialLight.intensity = intensityTimePast * stats.playerStats.totalVision;
            }
        }
    }
    private void UpdateLightningLight()
    {
        float lightningChance = Random.Range(0f, 1f);


        lightningAnimator.SetBool("LightningStrike", false);
        lightningAnimator.SetBool("LightningLarge", false);
        lightningAnimator.SetBool("LightningMedium", false);
        lightningAnimator.SetBool("LightningSmall", false);

        isBaseLightningChanceScaling = baseLightningChance * Mathf.Min((gameTime * (baseLightningMax / 2f) / maxTimeScaling) + (timeSinceOrbUsed * (baseLightningMax / 2f) / orbResetTime) + 1f, baseLightningMax);

        if (lightningChance < baseLightningChance *  Mathf.Min((gameTime * (baseLightningMax /2f) / maxTimeScaling) + (timeSinceOrbUsed * (baseLightningMax / 2f) / orbResetTime) , baseLightningMax))
        {
            float lightningPick = Random.Range(0f, 1f);

            // the math bit post lightingStrikeChance should be its own function with arguments for the max and the ratio between the two?
            isLightningStrikeScaling = lightningStrikeChance * Mathf.Min((gameTime * ((lightningStrikeMax / 2f) / maxTimeScaling)) + (timeSinceOrbUsed * ((lightningStrikeMax / 2f) / orbResetTime)) + 1f, lightningStrikeMax);
            isLightningLargeScaling = lightningStrikeChance * lightningLargeChance * Mathf.Min((gameTime * ((lightningLargeMax / 2f) / maxTimeScaling)) + (timeSinceOrbUsed * ((lightningLargeMax / 2f) / orbResetTime)) + 1f, lightningLargeMax);
            isLightningMediumScaling = lightningStrikeChance * lightningMediumChance * Mathf.Min((gameTime * ((lightningMediumMax / 2f) / maxTimeScaling)) + (timeSinceOrbUsed * ((lightningMediumMax / 2f) / orbResetTime)) + 1f, lightningMediumMax);

            if (lightningPick < isLightningStrikeScaling)
            {
                //Debug.Log("LightningSTRIKE" + lightningPick);
                lightningAnimator.SetBool("LightningStrike", true);
            }
            else if (lightningPick < isLightningLargeScaling)
            {
                //Debug.Log("lightningLarge" + lightningPick);
                lightningAnimator.SetBool("LightningLarge", true);
            }
            else if (lightningPick < isLightningMediumScaling)
            {
                //Debug.Log("lightningMedium" + lightningPick);
                lightningAnimator.SetBool("LightningMedium", true);
            }
            else
            {
                //Debug.Log("lightning small" + lightningPick);
                lightningAnimator.SetBool("LightningSmall", true);
            }
        }
    }
    private void UpdateRain()
    {
        var emission = rain.emission;
        emission.rateOverTime = Mathf.Min((gameTime * ( 750f / maxTimeScaling )) + (timeSinceOrbUsed * (250f / orbResetTime)) , 1000f ) - 100f;
        //this probably needs to be looked at again so itdoesnt start raining too soon
    }
}
