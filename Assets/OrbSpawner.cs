using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public Player player;
    public Vector3 nextOrbX;
    public float minOrbVariation;
    public float maxOrbVariation;
    public GameObject orb;

    void Start()
    {
        player = Player.activePlayer;
        nextOrbX = new Vector3 (player.transform.position.x + Random.Range(minOrbVariation, maxOrbVariation), 0f ,0f);
        spawnOrb();
    }

    void Update()
    {
        if (player.transform.position.x > nextOrbX.x)
        {
            nextOrbX = new Vector3 (nextOrbX.x + Random.Range(minOrbVariation, maxOrbVariation), 0f, 0f);
            spawnOrb();
        }

    }
    void spawnOrb()
    {
        Instantiate(orb).transform.position = nextOrbX;
    }
}
