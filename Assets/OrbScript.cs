using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbScript : MonoBehaviour
{
    public Player player;
    public bool isCollected;
    public float collectionRange;
    public float collectSpeedStart;
    public float collectSpeedGrowth;
    public float cullDistance = 25f;
    public float collectSpeedMax = 150f;
    public ParticleSystem collectParticle;
    public bool spawnedParticle;

    //Audio Variables
    private bool stage1 = false;
    private bool stage2 = false;

    void Start()
    {
        player = Player.activePlayer;
    }

    void Update()
    {
        Vector3 directionToPlayer = player.transform.position - this.transform.position;

        if(World.activeWorld.paused == true)
        {
            return;
        }

        if(directionToPlayer.magnitude < collectionRange)
        {
            isCollected = true;

            //Audio Section
            OrbAudio(1);  
        }

        if(isCollected)
        {
            //Audio Section
            OrbAudio(2);

            if (directionToPlayer.magnitude < 0.5f)//(directionToPlayer.normalized * collectSpeedStart).magnitude)
            {
                this.transform.position = player.transform.position;
                if(spawnedParticle == false)
                {
                    var collect = Instantiate(collectParticle.gameObject);
                    collect.transform.position = player.transform.position;
                    player.CollectOrb();
                    spawnedParticle = true;
                }

                //Audio Section
                OrbAudio(3);

                Destroy (this.gameObject);
            }
            else
            {
                this.transform.position = this.transform.position + directionToPlayer.normalized * (collectSpeedStart* Time.deltaTime);
                collectSpeedStart = Mathf.Min(collectSpeedMax, collectSpeedStart + (collectSpeedGrowth * Time.deltaTime));
            }
        }

        if (player.transform.position.x - cullDistance > this.transform.position.x && !isCollected)
        {
            Destroy(this.gameObject);
        }

    }

    void OrbAudio(int orbStage)
    {
        if((stage1 == true && orbStage == 1) || (stage2 == true && orbStage == 2))
        {
            return;
        }

        AkSoundEngine.PostEvent("PlayOrbCollect", this.gameObject);

        switch (orbStage)
        {
            case 1:
                stage1 = true;
                return;
            case 2:
                stage2 = true;
                return;
        }
        return;
    } 
}
