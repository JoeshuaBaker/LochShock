using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceAudioController : MonoBehaviour
{
    public World world;
    public ParticleSystem rain;
    public AK.Wwise.RTPC rainAmountRTPC;

    private bool onPath = true;
    private bool reChecker= true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PathChecker();
        RainMaker();
    }

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
            //Debug.Log("On Path");
            return;
        }
        else if(!onPath && reChecker)
        {
            AkSoundEngine.SetState("PathStatus", "OffPath");
            reChecker = false;
            //Debug.Log("Off Path");
            return;
        }
    }

    public void RainMaker()
    {
        //Debug.Log(rain.particleCount);
        rainAmountRTPC.SetGlobalValue((float)rain.particleCount);
        return;
    }

}
