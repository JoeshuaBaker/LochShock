using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrchinSpawner : MonoBehaviour
{
    public Enemy enemyToSpawn;
    public Transform enemyParent;
    public float spawnRate = 0.2f;
    public float spawnTimer;

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0f)
        {
            Instantiate(enemyToSpawn, this.transform.position, Quaternion.identity, enemyParent);
            spawnTimer = spawnRate;
        }
    }
}
