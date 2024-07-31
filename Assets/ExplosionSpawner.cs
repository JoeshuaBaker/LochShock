using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public class ExplosionSpawner : MonoBehaviour
{
    [Header("Explosion Fields")]
    public float maxSize;
    public float minSize;
    public float maxSlow;
    public float minFast;
    public GameObject explosionPrefab;
    public GameObject[] explosionArray;
    public int explosionArraySize = 50;
    public float noDebrisBelow;
    public CraterCreator craterCreator;
    public Transform effectParent;
    private int currentExplosion;
    private bool firstPass = true;
    private ParticleSystem.MinMaxCurve ps2BaseValues;

    [Header("Damage Zone Fields")]

    public DangerZone dangerZonePrefab;
    public DangerZone[] dangerZoneArray;
    public int dangerZoneArraySize = 50;
    private DangerZone spawnedDangerZone;

    [Header("test fields")]
    public Player activePlayer;
    public float explosionTime;
    public float explosionInterval;
    public float explosionSize;
    public float randomRange;
    public bool testExplosions;

    // Start is called before the first frame update
    void Start()
    {
        explosionTime = 0f;

        currentExplosion = 0;

        explosionArray = new GameObject[explosionArraySize];
        dangerZoneArray = new DangerZone[dangerZoneArraySize];

        for (int i = 0; i < explosionArray.Length; i++)
        {
            explosionArray[i] = Instantiate(explosionPrefab);
            explosionArray[i].transform.parent = effectParent;
            explosionArray[i].SetActive(false);
        }

        for (int i = 0; i < dangerZoneArray.Length; i++)
        {
            dangerZoneArray[i] = Instantiate(dangerZonePrefab);
            dangerZoneArray[i].transform.parent = effectParent;
            dangerZoneArray[i].gameObject.SetActive(false);

        }

    }

    // Update is called once per frame
    void Update()
    {
        randomRange = Random.Range(0f, 1f);
        explosionSize = Random.Range(0.1f, 3f);

        explosionTime = explosionTime + Time.deltaTime;
        if (explosionTime > explosionInterval && testExplosions)
        {

            CreateDangerZone(1000f, 1f, activePlayer.transform.position);
            //if (randomRange >= 0.5f)
            //{
            //    CreateExplosion(activePlayer.transform.position, explosionSize, Random.rotation);
            //}
            //else
            //{
            //    CreateExplosionWithCrater(activePlayer.transform.position, explosionSize);
            //}
            explosionTime = 0f;
        }
    }

    public GameObject CreateExplosionWithCrater(Vector3 explosionPos , float size)
    {
        GameObject spawnedExplosion = CreateExplosion (explosionPos , size , new Quaternion(0f,0f,0f,1f) );
        craterCreator.CreateCrater(explosionPos , Mathf.RoundToInt(size));

        return spawnedExplosion;

    }

    public GameObject CreateExplosion(Vector3 explosionPos, float size, Quaternion explosionRot)
    {
        float explosionSize = Mathf.Clamp(size, minSize , maxSize);

        GameObject spawnedExplosion = explosionArray[currentExplosion];

        if (currentExplosion < explosionArraySize - 1)
        {
            currentExplosion++;
        }
        else
        {
            currentExplosion = 0;
        }

        spawnedExplosion.SetActive(false);
        spawnedExplosion.SetActive(true);

        spawnedExplosion.transform.position = new Vector3(explosionPos.x , explosionPos.y , 0f);
        spawnedExplosion.transform.rotation = explosionRot; //rotating explosion
        spawnedExplosion.transform.localScale = new Vector3( explosionSize , explosionSize , explosionSize );

        CinemachineImpulseSource explosionImp = spawnedExplosion.GetComponent<CinemachineImpulseSource>();
        explosionImp.GenerateImpulse(explosionSize);

        Light2D explosionLight = spawnedExplosion.GetComponentInChildren<Light2D>();
        explosionLight.pointLightOuterRadius = (explosionLight.pointLightOuterRadius * (explosionSize * 0.75f));

        ParticleSystem[] explosionPS = spawnedExplosion.GetComponentsInChildren<ParticleSystem>();
        
        for (int i = 0; i < explosionPS.Length ; i++)
        {
            var expPSMain = explosionPS[i].main;
            var em = explosionPS[i].emission;

            if (i < 2)
            {
                float timeMod = Mathf.Clamp(1f / explosionSize , maxSlow , minFast);
                expPSMain.simulationSpeed = timeMod;
                
                short burstMax = em.GetBurst(0).maxCount;
                short burstMin = em.GetBurst(0).minCount;
                float burstTime = em.GetBurst(0).time;

                em.SetBurst(0, new ParticleSystem.Burst((burstTime * timeMod) , burstMin , burstMax ));
            }
            if (i == 2)
            {
                if (firstPass)
                {
                    ps2BaseValues = expPSMain.startSpeed;
                    firstPass = false;
                }
                else
                {
                    expPSMain.startSpeed = ps2BaseValues;
                }

                if (explosionRot != new Quaternion(0f, 0f, 0f, 1f))
                {
                    expPSMain.startLifetime = new ParticleSystem.MinMaxCurve(0.36f, 0.54f); // setting debris duration for rotatable explosion
                }

                expPSMain.startSpeed = new ParticleSystem.MinMaxCurve((expPSMain.startSpeed.constantMin * Mathf.Min( explosionSize , 2f )) , (expPSMain.startSpeed.constantMax * Mathf.Min( explosionSize , 2f )));

                if (explosionSize > noDebrisBelow)
                {
                    
                    em.SetBurst(0, new ParticleSystem.Burst(0.066f, (short)(2 * explosionSize), (short)(4 * explosionSize)));
                }
                else
                {
                    em.SetBurst(0, new ParticleSystem.Burst(0.066f, 0, 0));
                }
                
            }
            
        }

        return spawnedExplosion;

      
    

    }

    public GameObject CreateDangerZone(float damage, float delay , Vector3 position)
    {
        int i;

        for (i = 0; i < dangerZoneArray.Length; i++)
        {
            if(dangerZoneArray[i].gameObject.activeInHierarchy == false)
            {

                spawnedDangerZone = dangerZoneArray[i];
                spawnedDangerZone.transform.parent = effectParent;

            }
        }

        spawnedDangerZone.Setup(damage, delay, position);

        return spawnedDangerZone.gameObject;
    }

}
