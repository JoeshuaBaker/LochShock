using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class InitialLightConteroller : MonoBehaviour
{
    public Light2D initialLight;
    [SerializeField]
    private float startingIntensity;

    // Start is called before the first frame update
    void Start()
    {
        initialLight.intensity = startingIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        float intensityTimePast;
        intensityTimePast = startingIntensity - (0.005f * Time.timeSinceLevelLoad);

        initialLight.intensity = intensityTimePast * Player.activePlayer.totalVision;

        if (intensityTimePast < 0)
        {
            Object.Destroy(gameObject);
        }
    }
}
