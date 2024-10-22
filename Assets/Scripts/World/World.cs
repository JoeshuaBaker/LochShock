using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapPool))]
public class World : MonoBehaviour
{
    public Player player;
    public GameObject spawnContainer;
    public static World activeWorld;
    public bool paused = false;
    public ExplosionSpawner explosionSpawner;
    public GameContext worldStaticContext;
    public CraterCreator craterCreator;
    public BulletHitEffect hitEffect;
    public LightningBolt lightningBolt;
    public GameplayUI gameplayUi;
    public EnemyEmitterSpawner enemyEmitterSpawner;
    public GameObject crosshairVis;
    public DeathWall deathWall;
    public Transform projectilePoolParent;

    //Map Variables
    public MapPool mapPool;
    Queue<int> mapQueue;
    List<Map> loadedMaps;
    Queue<Map> activeMaps;
    public Map playerInBounds = null;
    Map mostRecentMap = null;
    int numMapsInPool = 0;
    int distToCullMap = 45;
    public Map startingMap;

    //Enemy Variables
    public EnemyPool enemyPool;
    public float horizontalSpawnBarrier = 16f;
    public float verticalSpawnBarrier = 12f;
    public float enemySpawn;
    public float enemySpawnRate = 1f;
    public float adjustedSpawnRate = 1f;
    public float enemySpawnRateFloor = .01f;
    public AnimationCurve enemySpawnRateDecay = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve orbDecay = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float enemySpawnRateDecayTime = 10f;
    public float orbDecayMaxtime = 1.5f;

    //internal state variables

    //boss variables
    public BossSeed boss;
    public BossSeed spawnedBoss;
    public int bossSpawnDistance;
    public bool bossSpawned;
    public bool bossDead;
    public bool bossDeathFinished;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = -1;
        SetupMaps();
        SetupEnemies();
        player = Player.activePlayer;

        if (player != null)
        {
            foreach (Map map in activeMaps)
            {
                if (map.IsWithinBounds(player.transform.position))
                {
                    playerInBounds = map;
                    break;
                }
            }

            if (startingMap != null)
            {
                startingMap = Instantiate(startingMap, this.transform);
                PositionMap(startingMap, playerInBounds, false);
                Tile startTile = startingMap.startTiles[UnityEngine.Random.Range(0, startingMap.startTiles.Count)];
                player.transform.position = startTile.transform.position;
                playerInBounds = startingMap;
            }
            else
            {
                player.transform.position = playerInBounds.startTiles[0].transform.position;
            }

            if (spawnContainer != null)
            {
                spawnContainer.transform.position = player.transform.position;
            }
        }

        if(gameplayUi == null)
        {
            Debug.Log("world needs gameplayUI reference");
        }
        if(crosshairVis == null)
        {
            Debug.Log("world needs CrosshairVisibility reference, Camera->worldspaceui->crosshaircontainer->crosshairVis");
        }

    }

    void Update()
    {
        UpdateMaps();
        UpdateEnemies();
        UpdateBoss();
    }

    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        paused = pause;
    }

    public void SetupContext()
    {
        activeWorld = this;
        worldStaticContext = new GameContext
        {
            player = Player.activePlayer,
            activeEnemies = new HashSet<Enemy>()
        };
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

    public bool IsBoundsOnPath(Bounds bounds, bool solidTilesOnly = true)
    {
        bool withinPath = true;
        int leftEdge = Mathf.FloorToInt(bounds.center.x - bounds.extents.x);
        int rightEdge = Mathf.FloorToInt(bounds.center.x + bounds.extents.x);
        int upEdge = Mathf.FloorToInt(bounds.center.y - bounds.extents.y);
        int downEdge = Mathf.FloorToInt(bounds.center.y + bounds.extents.y);

        foreach (Map map in activeMaps)
        {
            if(map.IsWithinBounds(bounds.center) || map.IsWithinBounds(bounds.center - bounds.extents) || map.IsWithinBounds(bounds.center + bounds.extents))
            {
                for(int x = leftEdge; x <= rightEdge; x++)
                {
                    for(int y = upEdge; y <= downEdge; y++)
                    {
                        Tile tileUnderBounds = map.TileAt(new Vector3(x, y));

                        if(tileUnderBounds == null || (solidTilesOnly && !tileUnderBounds.isSolid))
                        {
                            withinPath = false;
                        }
                    }
                }
            }
        }

        return withinPath;
    }

    public List<Tile> TilesWithinBounds(Bounds bounds, bool solidTilesOnly = false)
    {
        List<Tile> tilesWithinBounds = new List<Tile>();
        int leftEdge = Mathf.FloorToInt(bounds.center.x - bounds.extents.x);
        int rightEdge = Mathf.FloorToInt(bounds.center.x + bounds.extents.x);
        int upEdge = Mathf.FloorToInt(bounds.center.y - bounds.extents.y);
        int downEdge = Mathf.FloorToInt(bounds.center.y + bounds.extents.y);

        foreach (Map map in activeMaps)
        {
            if (map.IsWithinBounds(bounds.center) || map.IsWithinBounds(bounds.center - bounds.extents) || map.IsWithinBounds(bounds.center + bounds.extents))
            {
                for (int x = leftEdge; x <= rightEdge; x++)
                {
                    for (int y = upEdge; y <= downEdge; y++)
                    {
                        Tile tileUnderBounds = map.TileAt(new Vector3(x, y));

                        if (tileUnderBounds != null)
                        {
                            if(solidTilesOnly)
                            {
                                if(tileUnderBounds.isSolid)
                                {
                                    tilesWithinBounds.Add(tileUnderBounds);
                                }
                            }
                            else
                            {
                                tilesWithinBounds.Add(tileUnderBounds);
                            } 
                        }
                    }
                }
            }
        }

        return tilesWithinBounds;
    }

    public Tile ClosestTile(Vector3 pos)
    {
        float distanceSqr = float.MaxValue;
        Tile closest = null;
        foreach(Map map in activeMaps)
        {
            if(map.IsWithinBounds(pos) && (closest = map.TileAt(pos)) != null)
            {
                return closest;
            }
        }


        foreach(Map map in activeMaps)
        {
            foreach(Tile tile in map.childTiles)
            {
                float sqrMag = (tile.transform.position - pos).sqrMagnitude;
                if (sqrMag < distanceSqr)
                {
                    distanceSqr = sqrMag;
                    closest = tile;
                }
            }
        }

        return closest;
    }

    public float ClosestTileDistance(Vector3 pos)
    {
        Tile closestTile = ClosestTile(pos);
        return closestTile == null ? float.MaxValue : (closestTile.transform.position - pos).magnitude;
    }

    public Tile TileUnderPlayer(Vector3 position)
    {
        return playerInBounds == null ? null : playerInBounds.TileAt(position);
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

        if(playerInBounds != startingMap)
        {
            playerInBounds = null;
        }
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
        float levelLoadTimeRatio = Mathf.Min(Time.timeSinceLevelLoad / (enemySpawnRateDecayTime * 60f), 1f);
        float orbTimeRatio = Mathf.Min(Player.activePlayer.timeSinceOrbUsed / (orbDecayMaxtime * 60f), 1f);
        adjustedSpawnRate = Mathf.Lerp(enemySpawnRate, enemySpawnRateFloor, enemySpawnRateDecay.Evaluate(
            Mathf.Min(levelLoadTimeRatio + (1f - levelLoadTimeRatio) * orbDecay.Evaluate(orbTimeRatio), 1f)));
       
        while (enemySpawn < adjustedSpawnRate)
        {
            enemySpawn += adjustedSpawnRate;
            SpawnRandomBlockEnemy();
        }

        worldStaticContext.activeEnemies = enemyPool.activeEnemies;

        Enemy closestEnemy = null;
        float distanceSqr = float.MaxValue;
        Vector3 playerPosition = player.transform.position;
        foreach (Enemy enemy in worldStaticContext.activeEnemies)
        {
            float newDist = (enemy.transform.position - playerPosition).sqrMagnitude;
            if (newDist < distanceSqr)
            {
                closestEnemy = enemy;
                distanceSqr = newDist;
            }
        }
        worldStaticContext.closestEnemy = closestEnemy;

        if (enemyEmitterSpawner != null && enemyEmitterSpawner.isActiveAndEnabled)
        {
            enemyEmitterSpawner.UpdateEmitterSpawner(enemySpawnRateDecay.Evaluate(levelLoadTimeRatio), orbDecay.Evaluate(orbTimeRatio));
        }
    }

    public void ClearAllEnemyBullets()
    {
        if(enemyEmitterSpawner != null && enemyEmitterSpawner.isActiveAndEnabled)
        {
            enemyEmitterSpawner.ClearAllBullets();
        }
    }

    void SpawnRandomEnemy()
    {
        List<Type> enemyTypes = enemyPool.GetAvailableEnemyTypes();
        if(enemyTypes == null || enemyTypes.Count == 0)
        {
            return;
        }

        Enemy enemy = enemyPool.GetEnemy(enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)]);
        SetupEnemy(enemy);
    }

    void SpawnRandomBlockEnemy()
    {
        Enemy enemy = enemyPool.GetCurrentBlockEnemy();
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

    void PositionMap(Map left, Map right, bool positionRight = true)
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
        if(positionRight)
        {
            right.transform.position = right.transform.position + difference;
        }
        else
        {
            left.transform.position = left.transform.position - difference;
        }
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

    public void UpdateBoss()
    {
        if( player.transform.position.x >= bossSpawnDistance && !bossSpawned)
        {
            spawnedBoss = Instantiate(boss);
            spawnedBoss.SetDistance(bossSpawnDistance);

            spawnedBoss.bossCurrentHP = spawnedBoss.bossMaxHP;

            bossSpawned = true;
            gameplayUi.bossActive = true;
        }
        if(spawnedBoss != null)
        {
            if(spawnedBoss.bossHPPercent <= 0f)
            {

                bossDead = true;
                gameplayUi.bossDead = true;
                player.bossDead = true;
                deathWall.bossDead = true;

                crosshairVis.SetActive(false);

                bossDeathFinished = spawnedBoss.deathFinished;
                gameplayUi.bossDeathFinished = spawnedBoss.deathFinished;

                if (bossDeathFinished)
                {
                    crosshairVis.SetActive(true);
                }
            }

            gameplayUi.bossHpPercent = spawnedBoss.bossHPPercent;
        }
    }
}
