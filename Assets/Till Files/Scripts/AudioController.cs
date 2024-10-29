using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Path World")]
    public World world;

    [Header("Ambiance Controllers")]
    public ParticleSystem rainSystem;
    public AK.Wwise.RTPC rainAmountRTPC;
    public AK.Wwise.RTPC distanceFromPathRTPC;
    private float maxPathDistanceRTPC = 30;


    [Header("Damage Splats")]
    public ParticleSystem dripSystem;
    public ParticleSystem splatSystem;
    public AK.Wwise.RTPC splatAmountRTPC;

    //[Header("Explosion Handlers")]
    private GameObject plumeObject = null;
    private ParticleSystem plumeSystem;

    [Header("Debug Switches")]
    public bool debugPath = false;
    public bool debugRain = false;
    public bool debugSplats = false;
    public bool debugBooms = false;
    public bool debugDarkness = false;
    public bool pathDistanceCheck = false;

    private float offPathTimer = 0;
    private bool onPath = true;
    private float currentDistanceFromPath = 0.0f;
    private int splatCount = 0;
    private int plumeCount = 0;
    private bool plumeCheck = false;
    private bool darkCheck1 = true;
    private bool darkCheck2 = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        WaterCompany();
        RainMaker();
        SplatManJones();
        DarknessControl();
    }

    //Checks whether the player is off path and if so, adjust the RTPC for the Water blend container
    public void WaterCompany()
    {
       currentDistanceFromPath = world.ClosestTileDistance(this.gameObject.transform.position);

        if (!CheckPath())
        {
            if (currentDistanceFromPath >= maxPathDistanceRTPC)
            {
                distanceFromPathRTPC.SetGlobalValue(maxPathDistanceRTPC);
            }
            else
            {
                distanceFromPathRTPC.SetGlobalValue(currentDistanceFromPath);
            }

            //Debug
            if (pathDistanceCheck) { Debug.Log("Setting RTPC to Distance" + " | Current Distance From Path: " + currentDistanceFromPath); }
            ////////
        }
        else if (CheckPath())
        {
            distanceFromPathRTPC.SetGlobalValue(0.0f);

            //Debug
            if (pathDistanceCheck) { Debug.Log("Setting RTPC to 0" + " | Current Distance From Path: " + currentDistanceFromPath); }
            ///////
        }

        return;
    }

    //Starts Darkness sound if you are off path more than 5 seconds and stops Darkness sound if yoou return to path
    public void DarknessControl()
    {
        if (!CheckPath())
        {
            offPathTimer += Time.deltaTime;
            if(offPathTimer > 5.0)
            {
                if (darkCheck1)
                {
                    AkSoundEngine.PostEvent("PlayDarkness", this.gameObject);
                    darkCheck1 = false;
                    darkCheck2 = true;
                }
            }
        }
        else if (CheckPath())
        {
            if (darkCheck2)
            {
                AkSoundEngine.PostEvent("StopDarkness", this.gameObject);
                darkCheck1 = true;
                darkCheck2 = false;                
            }
            offPathTimer = 0;
        }

        //Debug
        if (debugDarkness) { Debug.Log("Time off Path: " + offPathTimer); }
        ///////

        return;
    }
    

    //Controls rain ambiance by setting number of rain particles on screen to the RainAmmount RTPC
    public void RainMaker()
    {        
        rainAmountRTPC.SetGlobalValue((float)rainSystem.particleCount);

        //Debug
        if (debugRain) { Debug.Log(rainSystem.particleCount); }
        ///////

        return;
    }

    //Handles oil splats that happen on player taking damage & death
    public void SplatManJones()
    {

        if (splatSystem.particleCount > splatCount && !dripSystem.isStopped)
        {
            AkSoundEngine.PostEvent("PlaySplats", gameObject);
            splatCount += 1;            
        }

        if(splatSystem.particleCount > 0 && dripSystem.isStopped)
        {
            splatCount = 0;
        }

        splatAmountRTPC.SetGlobalValue((float)splatCount);

        //Debug
        if (debugSplats) { Debug.Log("splatCount = " + splatCount); }
        ///////

        return;
    }

    //Handles Explosions that spawn on player death,  plays after player player death anim is finished
    void PlumeRide()
    {
        plumeObject = GameObject.Find("EplosionPlumePS");
        if(plumeObject != null)
        {
            if(!plumeCheck) {  AkSoundEngine.PostEvent("PlayDeathBoom", this.gameObject); plumeCheck = true; }
            plumeSystem = plumeObject.GetComponent<ParticleSystem>();

            if(plumeSystem.particleCount > plumeCount)
            {
                AkSoundEngine.PostEvent("PlayExplosion", gameObject);
                plumeCount++;
            }

            //Debug
            if (debugBooms) { Debug.Log("plumeCount = " + plumeCount); }
            if (debugBooms) { Debug.Log(plumeCheck); }
            ///////

        }

        return;
    }

    //Returns bool of whether you are currently above a path tile
    public bool CheckPath()
    {
        Tile tileUnderPlayer = world.TileUnderPlayer(this.transform.position);

        if (tileUnderPlayer != null)
        {
            onPath = tileUnderPlayer.collider2d.OverlapPoint(this.transform.position.xy());
        }
        else
        {
            onPath = false;
        }

        if (debugPath) { Debug.Log("On Path = " + onPath); }
        return (onPath);
    }

}
