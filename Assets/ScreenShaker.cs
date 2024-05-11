using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShaker : MonoBehaviour

{ 
    public CinemachineVirtualCamera cam;
    //public float m_AmplitudeGain;
    public float shakeAmount;
    public float dampenAmount;
    public float settingsDampen;
   
    // Start is called before the first frame update
    void Start()
    {
        shakeAmount = 0f;
        settingsDampen = 0f; // player settings
    }

    // Update is called once per frame
    void Update()
    {
        shakeResolver();
    }

    public void shakeIntake(float shakeAdded)
    {
        shakeAmount = shakeAmount + shakeAdded;
    }

    public void shakeResolver()
    {
        shakeAmount = shakeAmount * 0.1f;
        //cam.GetCinemachineComponent();
    }
}
