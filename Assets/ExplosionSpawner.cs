using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public class ExplosionSpawner : MonoBehaviour
{
    public float maxSize;
    public float minSize;
    public float maxSlow;
    public float minFast;
    public GameObject explosionPrefab;
    public float noDebrisBelow;
    public CraterCreator craterCreator;


    [Header("test fields")]
    public Player activePlayer;
    public float explosionTime;
    public float explosionInterval;
    public float explosionSize;

    // Start is called before the first frame update
    void Start()
    {
        explosionTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        explosionTime = explosionTime + Time.deltaTime;
        if (explosionTime > explosionInterval)
        {
            CreateExplosion(activePlayer.transform.position , activePlayer.transform.rotation , explosionSize);
            explosionTime = 0f;
        }
    }

    public GameObject CreateExplosion(Vector3 explosionPos , Quaternion explosionRot ,float size)
    {
        float explosionSize = Mathf.Clamp(size, minSize , maxSize);
       
        GameObject spawnedExplosion = Instantiate(explosionPrefab);
        spawnedExplosion.transform.position = new Vector3(explosionPos.x , explosionPos.y , 0f);
        spawnedExplosion.transform.rotation = explosionRot; //rotating explosion
        spawnedExplosion.transform.localScale = new Vector3( explosionSize , explosionSize , explosionSize );

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

                //expPSMain.startLifetime = new ParticleSystem.MinMaxCurve(0.36f, 0.54f); // setting debris duration for rotatable explosion
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

        craterCreator.CreateCrater( spawnedExplosion.transform.position , 3f ); //rotated explosion spawn no crater

        return spawnedExplosion;
    }
    
}