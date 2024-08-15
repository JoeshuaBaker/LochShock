using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CraterCreator : MonoBehaviour
{
    public GameObject crater;
    public GameObject[] craterArray;
    public int craterArraySize = 100;
    public Transform craterParent;
    public Sprite smallestCrater;
    public Sprite smallCrater;
    public Sprite mediumCrater;
    public Sprite largeCrater;
    public int currentCrater;
    [Header("test fields")]
    public Player activePlayer;
    public float craterTime;
    public float craterInterval;
    public int randomCrater;
    public bool testCraters;


    // Start is called before the first frame update
    void Start()
    {

        craterTime = 0f;

        currentCrater = 0;

        craterArray = new GameObject[craterArraySize];

        for (int i = 0; i < craterArray.Length; i++)
        {
            craterArray[i] = Instantiate(crater);
            craterArray[i].transform.parent = craterParent;
            craterArray[i].SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //this is purely to test the crater creator
        craterTime = craterTime + Time.deltaTime;
        randomCrater = Random.Range(0, 4);


        if (craterTime > craterInterval && testCraters)
        {
            CreateCrater(activePlayer.transform.position, randomCrater);
            craterTime = 0f;
        }

    }

    public GameObject CreateCrater (Vector3 craterPos, int craterSize)
    {
        GameObject spawnedCrater = craterArray[currentCrater];

        if (currentCrater < craterArraySize - 1 )
        {
            currentCrater++;
        }
        else
        {
            currentCrater = 0;
        }

        spawnedCrater.SetActive(false);
        spawnedCrater.SetActive(true);

        spawnedCrater.transform.position = new Vector3(craterPos.x, craterPos.y, 0f);
        SpriteRenderer craterSprite = spawnedCrater.GetComponentInChildren<SpriteRenderer>();
        ParticleSystem craterPS = spawnedCrater.GetComponentInChildren<ParticleSystem>();
        CinemachineImpulseSource craterImp = spawnedCrater.GetComponent<CinemachineImpulseSource>();
        var em = craterPS.emission;
        var sp = craterPS.shape;
        

        if (craterSize == 4)
        {
            craterSprite.sprite = largeCrater;
            sp.radius = .5f;
            em.SetBurst(0, new ParticleSystem.Burst(0, 4, 7));
           // craterImp.GenerateImpulse(3f);
        }
        if (craterSize == 3)
        {
            craterSprite.sprite = mediumCrater;
            sp.radius = .45f;
            em.SetBurst(0, new ParticleSystem.Burst(0, 3, 6));
            //craterImp.GenerateImpulse(2f);
        }
        if (craterSize == 2)
        {
            craterSprite.sprite = smallCrater;
            sp.radius = .35f;
            em.SetBurst(0, new ParticleSystem.Burst(0, 2, 4));
           // craterImp.GenerateImpulse(1f);
        }
        if (craterSize == 1)
        {
            craterSprite.sprite = smallestCrater;
            sp.radius = .2f;
            em.SetBurst(0, new ParticleSystem.Burst(0 , 1 , 3));

        }

        

        return spawnedCrater;
    }
}
