using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SpawnEffects : MonoBehaviour
{
    public CinemachineImpulseSource spawnImp;
    public Animator pillarAnimator;
    public CraterCreator craterCreator;
    private bool hasCrater;
    public LightningBolt lightningBolt;

    // Start is called before the first frame update
    void Start()
    {
        GameObject world = GameObject.FindWithTag ("World");
        craterCreator = world.GetComponent<CraterCreator>();
        AkSoundEngine.PostEvent("PlaySpawn", this.gameObject);

       

    }

    // Update is called once per frame
    void Update()
    {

        AnimatorStateInfo spawnAnimState = pillarAnimator.GetCurrentAnimatorStateInfo(0);

        if (spawnAnimState.normalizedTime >= 0.005f && hasCrater == false)
        {
            if(craterCreator != null)
            {
                craterCreator.CreateCrater(this.transform.position, 3);

            }
            spawnImp.GenerateImpulse(3f);
            hasCrater = true;

            lightningBolt.CallLightning(this.transform.position);

        }
        if (spawnAnimState.normalizedTime >= 1f)
        {
            Destroy(this.gameObject);
        }
        
    }
}
