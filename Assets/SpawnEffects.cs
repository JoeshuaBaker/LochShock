using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SpawnEffects : MonoBehaviour
{
    public CinemachineImpulseSource spawnImp;
    public Animator pillarAnimator;
    public GameObject spawnPillar;
    private bool hasCrater;
    public LightningBolt lightningBolt;
    public bool spawnedBolt;
    public bool decay = true;

    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.PostEvent("PlaySpawn", this.gameObject); 
    }

    // Update is called once per frame
    void Update()
    {
        if (decay)
        {
            AnimatorStateInfo spawnAnimState = pillarAnimator.GetCurrentAnimatorStateInfo(0);

            if (!spawnedBolt)
            {
                lightningBolt.CallLightning(this.transform.position);
                spawnedBolt = true;
            }

            if (spawnAnimState.normalizedTime >= 0.005f && hasCrater == false)
            {
                World.activeWorld.craterCreator.CreateCrater(this.transform.position, 3);
                spawnImp.GenerateImpulse(3f);
                hasCrater = true;
            }
            if (spawnAnimState.normalizedTime >= 1f)
            {
                Destroy(this.gameObject);
            }
        }  
    }

    public void PlayerSpawned()
    {
        spawnPillar.SetActive(true);
        lightningBolt.CallLightning(this.transform.position);
        decay = true;
    }
}
