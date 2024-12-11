using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDrones : MonoBehaviour
{
    public GameObject droneSpinner;
    public GameObject[] dronePositioners;
    public ParticleSystem[] droneTrails;
    public ParticleSystem[] droneLightning;

    public float rotationsPerSecond = 1f;
    public float positionScale = 10f;
    public float speedCurrent;
    public float speedMax = 20f;
    public float speedGainedOnHold = 2f;
    public float distMax = 25f;
    public float distCurrent;
    private float droneSetDist;
    public float droneMinDist = 1f;
    public float droneExpandStart;
    private float rotVel;
    private Vector3 droneDist;

    public float transitionTime;
    public float transitionTimeCurrent;
    private float transitionTimeScaled;
    public float holdTime;
    public float holdTimeCurrent;
    private float holdTimeScaled;
    public float transitionOutTime;
    public float transitionOutTimeCurrent;
    private float transitionOutTimeScaled;

    public float lightningMaxSize;
    private float lightningCurrentSize;

    public bool activateShopDrones;
    public bool deactivateShopDrones;

    public bool testInOut;
    public float testWaitTime = 5f;
    private float testWaitTimeCurrent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Player.activePlayer.dying)
        {
            DeactivateShopDrones();
        }

        if (testInOut && !activateShopDrones && !deactivateShopDrones)
        {
            testWaitTimeCurrent += Time.deltaTime;

            if(testWaitTimeCurrent >= testWaitTime)
            {
                activateShopDrones = true;
                testWaitTimeCurrent = 0f;
            }
        }

        if (activateShopDrones)
        {
            ShopDronesIn();
        }
        else if (deactivateShopDrones)
        {
            ShopDronesOut();
        }
    }

    public void ActivateShopDrones()
    {
        activateShopDrones = true;
    }

    public void DeactivateShopDrones()
    {
        activateShopDrones = false;
        deactivateShopDrones = true;

        droneExpandStart = distCurrent;

    }

    public void ShopDronesIn()
    {
        if (transitionTimeCurrent < transitionTime)
        {
            droneSpinner.gameObject.SetActive(true);

            transitionTimeCurrent += Time.deltaTime;
            transitionTimeScaled = transitionTimeCurrent / transitionTime;

            speedCurrent = Mathf.Lerp(-1f, speedMax, transitionTimeScaled);

            distCurrent = Mathf.Lerp(distMax, droneMinDist, transitionTimeScaled);
            droneSetDist = distCurrent;
            droneDist = new Vector3(droneSetDist, 0f, 0f);

            for (int i = 0; i < dronePositioners.Length; i++)
            {
                dronePositioners[i].transform.localPosition = droneDist;
            }
        }
        else
        {
            holdTimeCurrent += Time.deltaTime;
            holdTimeScaled = holdTimeCurrent / holdTime;

            speedCurrent += speedGainedOnHold * Time.deltaTime;

            var lightning = droneLightning[0].main;
            lightning.startSize = Mathf.Lerp(0f, lightningMaxSize, holdTimeScaled);
            droneLightning[0].gameObject.SetActive(true);

            var lightning2 = droneLightning[2].main;
            lightning2.startSize = Mathf.Lerp(0f, lightningMaxSize, holdTimeScaled);
            droneLightning[2].gameObject.SetActive(true);

            if (holdTimeCurrent >= holdTime)
            {
                lightning.startSize = 1f;
                lightning2.startSize = 1f;

                holdTimeCurrent = 0f;
                transitionTimeCurrent = 0f;

                Player.activePlayer.inventory.Orb(true);

                DeactivateShopDrones();

            }
        }

        rotVel = speedCurrent / Mathf.Max(droneDist.x, 1f);

        droneSpinner.transform.Rotate(new Vector3(0, 0, 360 * rotVel * Time.deltaTime));
    }

    public void ShopDronesOut()
    {
        for (int i = 0; i < droneLightning.Length; i++)
        {
            droneLightning[i].gameObject.SetActive(true);
        }

        transitionOutTimeCurrent += Time.deltaTime;
        transitionOutTimeScaled = transitionOutTimeCurrent / transitionOutTime;

        distCurrent = Mathf.Lerp(droneExpandStart, distMax, transitionOutTimeScaled);
        droneSetDist = distCurrent;
        droneDist = new Vector3(droneSetDist, 0f, 0f);

        for (int i = 0; i < dronePositioners.Length; i++)
        {
            dronePositioners[i].transform.localPosition = droneDist;
        }

        rotVel = speedCurrent / Mathf.Max(droneDist.x, 1f);
        droneSpinner.transform.Rotate(new Vector3(0, 0, 360 * rotVel * 2f * Time.deltaTime));

        if (transitionOutTimeCurrent >= transitionOutTime)
        {
            for(int i =0; i< droneLightning.Length; i++)
            {
                droneLightning[i].gameObject.SetActive(false);
            }

            deactivateShopDrones = false;
            transitionOutTimeCurrent = 0f;
            droneSpinner.gameObject.SetActive(false);
        }
    }
}
