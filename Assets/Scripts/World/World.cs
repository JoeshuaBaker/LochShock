using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapPool))]
public class World : MonoBehaviour
{
    public Player player;
    public static World activeWorld;
    public bool paused = false;

    //Map Variables
    public MapPool mapPool;
    Queue<int> mapQueue;
    List<Map> loadedMaps;
    Queue<Map> activeMaps;
    public Map playerInBounds = null;
    Map mostRecentMap = null;
    int numMapsInPool = 0;
    int distToCullMap = 45;

    //Enemy Variables
    public EnemyPool enemyPool;
    public float horizontalSpawnBarrier = 16f;
    public float verticalSpawnBarrier = 12f;
    public float enemySpawn;
    public float enemySpawnRate = 10f;
    public float enemySpawnRateFloor = 1f;
    public float enemySpawnRateDecay = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        activeWorld = this;
        Application.targetFrameRate = -1;
        SetupMaps();
        SetupEnemies();
    }

    void Update()
    {
        UpdateMaps();
        UpdateEnemies();
    }

    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        paused = pause;
    }

    void SetupMaps()
    {
        mapPool = GetComponent<MapPool>();
        mapQueue = new Queue<int>();
        loadedMaps = new List<Map>();
        activeMaps = new Queue<Map>();
        numMapsInPool = mapPool.maps.Count;

        for (int i = 0; i < numMapsInPool; i++)
        {
            Map map = mapPool.maps[i];

            //if there are abnormally wide maps we may need to set our cull distance higher to account.
            //as of right now, all maps are width=30 so this shouldnt do anything. just insurance for future.
            float mapWidth = Mathf.Abs(map.endTiles[0].transform.position.x - map.startTiles[0].transform.position.x);
            if ((int)(mapWidth * 1.5f) > distToCullMap)
            {
                distToCullMap = (int)(mapWidth * 1.5f);
            }

            map = Instantiate(map, this.transform);
            map.gameObject.SetActive(false);
            loadedMaps.Add(map);
        }

        for (int i = 0; i < 3; i++)
        {
            SpawnMap();
        }
    }

    public Tile RandomPathTileAtXPosition(float xPos)
    {
        Tile tileAtPos = null;
        foreach(Map map in activeMaps)
        {
            if(map.IsWithinBounds(new Vector3(xPos, map.transform.position.y, map.transform.position.z)))
            {
                tileAtPos = map.GetRandomTileAtXPos(xPos);
                if(tileAtPos != null)
                {
                    return tileAtPos;
                }
            }
        }

        return null;
    }

    public Tile TileUnderPlayer(Vector3 position)
    {
        return playerInBounds?.TileAt(position);
    }

    void SetupEnemies()
    {
        enemySpawn = 0f;
    }

    void UpdateMaps()
    {
        //cull maps too far behind player
        if (player.transform.position.x - activeMaps.Peek().transform.position.x > distToCullMap)
        {
            Map cullMap = activeMaps.Dequeue();
            cullMap.gameObject.SetActive(false);
        }

        //spawn next map if player gets too close to most recent map
        if (player.transform.position.x - mostRecentMap.transform.position.x > -distToCullMap)
        {
            SpawnMap();
        }

        playerInBounds = null;
        foreach(Map map in activeMaps)
        {
            if (map.IsWithinBounds(player.transform.position))
            {
                playerInBounds = map;
                break;
            }
        }
    }

    void UpdateEnemies()
    {
        enemySpawn -= Time.deltaTime;
        enemySpawnRate = Mathf.Max(enemySpawnRate - enemySpawnRateDecay * Time.deltaTime, enemySpawnRateFloor);
       
        for (float i = enemySpawn; i <= enemySpawnRate; i += enemySpawnRate)
        {
            enemySpawn += enemySpawnRate;
            SpawnRandomEnemy();
        }

       
        //    // To Spawn a specific enemy:
        //    //SetupEnemy(enemyPool.GetEnemy(typeof(Eye)));
        //    // Replace 'Urchin' with enemy type desired
        

    }

    void SpawnRandomEnemy()
    {
        List<Type> enemyTypes = enemyPool.GetAvailableEnemyTypes();

        Enemy enemy = enemyPool.GetEnemy(enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)]);
        SetupEnemy(enemy);
    }

    void SetupEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        bool leftright = UnityEngine.Random.Range(0f, 1f) < 0.5f;
        bool updown = UnityEngine.Random.Range(0f, 1f) < 0.5f;
        Vector2 diagonal = new Vector2(leftright ? 1 : -0.25f, updown ? 1 : -1);
        Vector2 lerpVecBase = Vector2.right;

        if (!leftright)
        {
            if (updown)
            {
                lerpVecBase = new Vector2(1, 1);
            }
            else
            {
                lerpVecBase = new Vector2(1, -1);
            }
        }
        Vector2 lerpVec = Vector2.Lerp(lerpVecBase, diagonal, UnityEngine.Random.Range(0f, 1f));
        Vector3 spawnPosition = new Vector3(
            horizontalSpawnBarrier * lerpVec.x, verticalSpawnBarrier * lerpVec.y, 0) + (player.transform.position - this.transform.position);

        enemy.transform.localPosition = spawnPosition;
        enemy.Reset();
    }

    void PositionMap(Map left, Map right)
    {
        Comparer<Tile> tileSorter = Comparer<Tile>.Create(
            (x, y) => (int)(x.transform.position.y - y.transform.position.y)
        );

        List<Tile> leftEndTiles = left.endTiles;
        List<Tile> rightStartTiles = right.startTiles;
        leftEndTiles.Sort(tileSorter);
        rightStartTiles.Sort(tileSorter);

        Tile leftMiddleTile = leftEndTiles[leftEndTiles.Count / 2];
        Tile rightMiddleTile = rightStartTiles[rightStartTiles.Count / 2];

        Vector3 difference = leftMiddleTile.transform.position - rightMiddleTile.transform.position + Vector3.right;
        right.transform.position = right.transform.position + difference;
    }

    void SpawnMap()
    {
        int mapIndex = GetOpenMapIndex();
        Map nextMap = loadedMaps[mapIndex];
        ResetMap(nextMap);
    }

    void ResetMap(Map map)
    {
        map.gameObject.SetActive(true);
        activeMaps.Enqueue(map);
        if(mostRecentMap != null)
        {
            PositionMap(mostRecentMap, map);
        }
        mostRecentMap = map;
    }

    private void RefillMapQueue()
    {
        for (int i = 0; i < 100; i++)
        {
            mapQueue.Enqueue(UnityEngine.Random.Range(0, numMapsInPool));
        }
    }

    private int GetOpenMapIndex()
    {
        if (mapQueue.Count < 10)
        {
            RefillMapQueue();
        }

        int mapIndex = mapQueue.Dequeue();
        while (loadedMaps[mapIndex].gameObject.activeSelf == true)
        {
            if (mapQueue.Count < 10)
            {
                RefillMapQueue();
            }
            mapIndex = mapQueue.Dequeue();
        }

        return mapIndex;
    }
}
