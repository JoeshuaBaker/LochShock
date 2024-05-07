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
    public ParticleSystem collectParticle;
    public bool spawnedParticle;
 


    void Start()
    {

        player = Player.activePlayer;
    }

    void Update()
    {
        
        Vector3 directionToPlayer = player.transform.position - this.transform.position ;

        if(directionToPlayer.magnitude < collectionRange)
        {
            isCollected = true;
        }
        if(isCollected == true)
        {

            if(directionToPlayer.magnitude < (directionToPlayer.normalized * collectSpeedStart).magnitude)
            {
                this.transform.position = player.transform.position;
                if(spawnedParticle == false)
                {
                    var collect = Instantiate(collectParticle.gameObject);
                    collect.transform.position = player.transform.position;
                    player.orbsHeld = player.orbsHeld + 1f;
                    spawnedParticle = true;
                }
              
                Destroy (this.gameObject, 0.1f);
            }
            else
            {
                this.transform.position = this.transform.position + directionToPlayer.normalized * collectSpeedStart;
                collectSpeedStart = collectSpeedStart + collectSpeedGrowth;
            }

            
        }

        if (player.transform.position.x - 25f > this.transform.position.x)
        {
            Destroy(this.gameObject);
        }

    }
 
}
