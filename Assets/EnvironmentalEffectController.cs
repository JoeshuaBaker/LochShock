using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class EnvironmentalEffectController : MonoBehaviour, ILevelLoadComponent
{
    public Light2D initialLight;
    public bool initialLightDestroyed;
    [SerializeField]
    private float startingIntensity;
    public float minInitialIntensity;
    public float minIntensityCutOff;
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
    public bool spawnBoltPlayed;
    [Header("Rain")]
    public ParticleSystem rain;
    [Header("Time")]
    public float gameTime;
    public float maxTimeScaling;
    public float timeSinceOrbUsed;
    public float orbResetTime;
    [Header("Test Lightning")]
    public bool testLightning;
    public bool testSmall;
    public bool testMedium;
    public bool testLarge;
    public bool testLightningStrike;
    public float timePerTest = 3f;
    public float timePerTestCurrent;


    // Start is called before the first frame update
    void Start()
    {
        initialLight.intensity = startingIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceOrbUsed = Player.activePlayer.timeSinceOrbUsed;
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
            intensityTimePast = Mathf.Max (startingIntensity - (0.005f * gameTime), minInitialIntensity);

            //if (intensityTimePast < 0)
            //{
            //    Object.Destroy(initialLight.gameObject);
            //    initialLightDestroyed = true;
            //}
            //else
            if (Player.activePlayer.totalVision >= minIntensityCutOff)
            {
                initialLight.intensity = intensityTimePast * Player.activePlayer.totalVision;
            }
            else
            {
                initialLight.intensity = 0f;
            }
        }
    }
    private void UpdateLightningLight()
    {
        if (!testLightning)
        {
            float lightningChance = Random.Range(0f, 1f);


            //lightningAnimator.SetBool("LightningStrike", false);
            //lightningAnimator.SetBool("LightningLarge", false);
            //lightningAnimator.SetBool("LightningMedium", false);
            //lightningAnimator.SetBool("LightningSmall", false);

            isBaseLightningChanceScaling = baseLightningChance * Mathf.Min((gameTime * (baseLightningMax / 2f) / maxTimeScaling) + (timeSinceOrbUsed * (baseLightningMax / 2f) / orbResetTime) + 1f, baseLightningMax);

            if (lightningChance < baseLightningChance * Mathf.Min((gameTime * (baseLightningMax / 2f) / maxTimeScaling) + (timeSinceOrbUsed * (baseLightningMax / 2f) / orbResetTime), baseLightningMax) || !spawnBoltPlayed)
            {
                float lightningPick = Random.Range(0f, 1f);

                // the math bit post lightingStrikeChance should be its own function with arguments for the max and the ratio between the two?
                isLightningStrikeScaling = lightningStrikeChance * Mathf.Min((gameTime * ((lightningStrikeMax / 2f) / maxTimeScaling)) + (timeSinceOrbUsed * ((lightningStrikeMax / 2f) / orbResetTime)) + 1f, lightningStrikeMax);
                isLightningLargeScaling = lightningStrikeChance * lightningLargeChance * Mathf.Min((gameTime * ((lightningLargeMax / 2f) / maxTimeScaling)) + (timeSinceOrbUsed * ((lightningLargeMax / 2f) / orbResetTime)) + 1f, lightningLargeMax);
                isLightningMediumScaling = lightningStrikeChance * lightningMediumChance * Mathf.Min((gameTime * ((lightningMediumMax / 2f) / maxTimeScaling)) + (timeSinceOrbUsed * ((lightningMediumMax / 2f) / orbResetTime)) + 1f, lightningMediumMax);

                if (lightningPick < isLightningStrikeScaling || !spawnBoltPlayed)
                {
                    //Debug.Log("LightningSTRIKE" + lightningPick);

                    lightningAnimator.Play("LightningStrike", -1, 0f);

                    //lightningAnimator.SetBool("LightningStrike", true);
                    AkSoundEngine.PostEvent("PlayThunder", this.gameObject);

                    if (spawnBoltPlayed)
                    {
                        Vector3 pos = Player.activePlayer.transform.position;
                        Vector3 randomPos = new Vector3(pos.x + Random.Range(-10f, 10f), pos.y + Random.Range(-10f, 10f), 0f);

                        World.activeWorld.lightningBolt.CallLightning(randomPos);
                        World.activeWorld.explosionSpawner.CreateDangerZone(5000f, 0f, randomPos, true, true, false, Vector3.one, false, Quaternion.identity, 0);

                    }

                    spawnBoltPlayed = true;

                }
                else if (lightningPick < isLightningLargeScaling)
                {
                    //Debug.Log("lightningLarge" + lightningPick);

                    lightningAnimator.Play("LightningLarge", -1, 0f);

                    //lightningAnimator.SetBool("LightningLarge", true);
                    AkSoundEngine.PostEvent("PlayThunder", this.gameObject);
                }
                else if (lightningPick < isLightningMediumScaling)
                {
                    //Debug.Log("lightningMedium" + lightningPick);

                    lightningAnimator.Play("LightningMedium", -1, 0f);

                    //lightningAnimator.SetBool("LightningMedium", true);
                    AkSoundEngine.PostEvent("PlayThunder", this.gameObject);
                }
                else
                {
                    //Debug.Log("lightning small" + lightningPick);

                    lightningAnimator.Play("LightningSmall", -1, 0f);

                    //lightningAnimator.SetBool("LightningSmall", true);
                    AkSoundEngine.PostEvent("PlayThunder", this.gameObject);
                }
            }
        }
        else
        {

            timePerTestCurrent += Time.deltaTime;

            if(timePerTestCurrent >= timePerTest)
            {
                if (testSmall)
                {
                    lightningAnimator.Play("LightningSmall", -1, 0f);
                }
                else if (testMedium)
                {
                    lightningAnimator.Play("LightningMedium", -1, 0f);
                }
                else if (testLarge)
                {
                    lightningAnimator.Play("LightningLarge", -1, 0f);
                }
                else if (testLightningStrike)
                {
                    lightningAnimator.Play("LightningStrike", -1, 0f);

                    Vector3 pos = Player.activePlayer.transform.position;
                    Vector3 randomPos = new Vector3(pos.x + Random.Range(-10f, 10f), pos.y + Random.Range(-10f, 10f), 0f);

                    World.activeWorld.lightningBolt.CallLightning(randomPos);
                    World.activeWorld.explosionSpawner.CreateDangerZone(5000f, 0f, randomPos, true, true, false, Vector3.one, false, Quaternion.identity, 0);
                }

                timePerTestCurrent = 0f;
            }
        }

    
    }
    private void UpdateRain()
    {
        var emission = rain.emission;
        emission.rateOverTime = Mathf.Min((gameTime * ( 750f / maxTimeScaling )) + (timeSinceOrbUsed * (250f / orbResetTime)) , 1000f ) - 100f;
        //this probably needs to be looked at again so itdoesnt start raining too soon
    }

    //Loadable Interface Functions
    public string LoadLabel()
    {
        return "Eerie Particles";
    }

    public int LoadPriority()
    {
        return 1000;
    }

    public void Load(World world)
    {
        EnvironmentalEffectController environmentalEffectInstance = Instantiate(this, Camera.main.transform);
        world.level.environmentalEffects = environmentalEffectInstance;
    }
}
