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
    public GameObject orb;
    public Transform orbParent;

    void Start()
    {
        player = Player.activePlayer;
        if(world == null)
        {
            world = GetComponent<World>();
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
        GameObject spawnedOrb = Instantiate(orb);
        spawnedOrb.transform.position = nextOrbX;
        spawnedOrb.transform.parent = orbParent;
    }
}
