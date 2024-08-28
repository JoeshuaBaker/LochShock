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


    private bool onPath = true;
    private bool reChecker = true;
    private int splatCount = 0;
    private int plumeCount = 0;
    private bool plumeCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.SetState("PathStatus", "OnPath");
    }

    // Update is called once per frame
    void Update()
    {
        PathChecker();
        RainMaker();
        SplatManJones();
        PlumeRide();
    }

    //Checks path and adjust sound of on/off path volume accordingly
    public void PathChecker()
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

        if(onPath && !reChecker)
        {
            AkSoundEngine.SetState("PathStatus", "OnPath");
            reChecker = true;

            //Debug
            if (debugPath) { Debug.Log("On Path"); }
            ///////

            return;
        }
        else if(!onPath && reChecker)
        {
            AkSoundEngine.SetState("PathStatus", "OffPath");
            reChecker = false;

            //Debug
            if (debugPath) { Debug.Log("Off Path"); }
            ///////

            return;
        }
    }

    //Controls rain ambiance by number of rain particles on screen
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

}
