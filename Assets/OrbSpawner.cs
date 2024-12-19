using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public Player player;
    public World world;
    public Vector3 nextOrbX;
    public float minOrbVariation;
    public float maxOrbVariation;
    public OrbScript orb;
    public Transform orbParent;


    void Start()
    {
        player = Player.activePlayer;
        if(world == null)
        {
            world = World.activeWorld;
        }
        nextOrbX = player.transform.position;
    }

    void Update()
    {
        if (player.transform.position.x > nextOrbX.x)
        {
            TrySpawnNextOrb();
        }

    }

    void TrySpawnNextOrb()
    {
        float nextOrbXPos = nextOrbX.x + Random.Range(minOrbVariation, maxOrbVariation);
        Tile tile = world.RandomPathTileAtXPosition(nextOrbXPos);
        if (tile != null)
        {
            nextOrbX = new Vector3(nextOrbXPos + 0.5f, tile.transform.position.y + 0.5f, tile.transform.position.z);
            SpawnOrb();
        }
    }

    void SpawnOrb()
    {
        OrbScript spawnedOrb = Instantiate(orb,nextOrbX,Quaternion.identity, orbParent) ;
    }

    public void SpawnCollectedOrbAtLocation(Vector3 location)
    {
        OrbScript spawnedOrb = Instantiate(orb, location, Quaternion.identity, orbParent);
        spawnedOrb.isCollected = true;
    }
}
