using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CraterCreator : MonoBehaviour
{
    public GameObject crater;
    public Transform craterParent;
    public Sprite smallestCrater;
    public Sprite smallCrater;
    public Sprite mediumCrater;
    public Sprite largeCrater;
    [Header("test fields")]
    public Player activePlayer;
    public float craterTime;
    public float craterInterval;
    public int randomCrater;


    // Start is called before the first frame update
    void Start()
    {
        craterTime = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        ////this is purely to test the crater creator
        //craterTime = craterTime + Time.deltaTime;
        //randomCrater = Random.Range(0, 4);
        

        //if (craterTime > craterInterval)
        //{
        //    CreateCrater(activePlayer.transform.position , randomCrater);
        //    craterTime = 0f;
        //}
        
    }

    public GameObject CreateCrater (Vector3 craterPos, int craterSize)
    {
        GameObject spawnedCrater = Instantiate(crater);
        spawnedCrater.transform.position = new Vector3(craterPos.x , craterPos.y , 0f);
        spawnedCrater.transform.parent = craterParent;
        SpriteRenderer craterSprite = spawnedCrater.GetComponentInChildren<SpriteRenderer>();
        craterSprite.sprite = smallestCrater;
        ParticleSystem craterPS = spawnedCrater.GetComponentInChildren<ParticleSystem>();
        CinemachineImpulseSource craterImp = spawnedCrater.GetComponent<CinemachineImpulseSource>();
        var em = craterPS.emission;
        var sp = craterPS.shape;
        

        if (craterSize == 3)
        {
            craterSprite.sprite = largeCrater;
            sp.radius = .5f;
            em.SetBurst(0, new ParticleSystem.Burst(0, 4, 7));
           // craterImp.GenerateImpulse(3f);
        }
        else if (craterSize == 2)
        {
            craterSprite.sprite = mediumCrater;
            sp.radius = .45f;
            em.SetBurst(0, new ParticleSystem.Burst(0, 3, 6));
            //craterImp.GenerateImpulse(2f);
        }
        else if (craterSize == 1)
        {
            craterSprite.sprite = smallCrater;
            sp.radius = .35f;
            em.SetBurst(0, new ParticleSystem.Burst(0, 2, 4));
           // craterImp.GenerateImpulse(1f);
        }
        else
        {
            craterSprite.sprite = smallestCrater;
            sp.radius = .2f;
            em.SetBurst(0, new ParticleSystem.Burst(0 , 1 , 3));

        }

        return spawnedCrater;
    }
}
