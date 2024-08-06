using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunAudio : MonoBehaviour
{
    public Gun railgunGun;
    public AK.Wwise.RTPC highPassFilterRTPC;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(railgunGun.fireSpeed);

        if(railgunGun.fireSpeed >= 5)
        {
            highPassFilterRTPC.SetGlobalValue(5);
            return;
        }
        else
        {
            highPassFilterRTPC.SetGlobalValue(railgunGun.fireSpeed);
        }
    }
}
