using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FireLightTest : MonoBehaviour
{
    public Light2D fireLight;
    public float maxIntensity;

    // Start is called before the first frame update
    void Start()
    {
        fireLight.intensity = 0;

    }

    // Update is called once per frame
    void Update()
    {
        fireLight.intensity = maxIntensity - (Time.timeSinceLevelLoad % 1f * maxIntensity * 3f);
        
    }
}
