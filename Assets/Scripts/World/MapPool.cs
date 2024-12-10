using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPool : MonoBehaviour, ILevelLoadComponent
{
    public List<Map> maps;
    public Map startingMap;

    Queue<int> mapQueue;
    List<Map> loadedMaps;
    Queue<Map> activeMaps;
    public Map playerInBounds = null;
    Map mostRecentMap = null;
    int numMapsInPool = 0;
    int distToCullMap = 45;

    public void Load(World world)
    {
        MapPool mapPoolInstance = Instantiate(this, world.transform);
        mapPoolInstance.SetupMaps();
        world.level.mapPool = mapPoolInstance;
    }

    //Loadable Interface Functions
    public string LoadLabel()
    {
        return "Map Chunks";
    }

    public int LoadPriority()
    {
        return 1000;
    }

    public void Setup()
    {
        
    }

    public void SetupMaps()
    {
        mapQueue = new Queue<int>();
        loadedMaps = new List<Map>();
        activeMaps = new Queue<Map>();
        numMapsInPool = maps.Count;

        for (int i = 0; i < numMapsInPool; i++)
        {
            Map map = maps[i];

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

        Player player = Player.activePlayer;

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
        }
    }

    public void UpdateMaps()
    {
        Player player = Player.activePlayer;
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

        if (playerInBounds != startingMap)
        {
            playerInBounds = null;
        }
        foreach (Map map in activeMaps)
        {
            if (map.IsWithinBounds(player.transform.position))
            {
                playerInBounds = map;
                break;
            }
        }
    }

    public Tile RandomPathTileAtXPosition(float xPos)
    {
        Tile tileAtPos = null;
        foreach (Map map in activeMaps)
        {
            if (map.IsWithinBounds(new Vector3(xPos, map.transform.position.y, map.transform.position.z)))
            {
                tileAtPos = map.GetRandomTileAtXPos(xPos);
                if (tileAtPos != null)
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
            if (map.IsWithinBounds(bounds.center) || map.IsWithinBounds(bounds.center - bounds.extents) || map.IsWithinBounds(bounds.center + bounds.extents))
            {
                for (int x = leftEdge; x <= rightEdge; x++)
                {
                    for (int y = upEdge; y <= downEdge; y++)
                    {
                        Tile tileUnderBounds = map.TileAt(new Vector3(x, y));

                        if (tileUnderBounds == null || (solidTilesOnly && !tileUnderBounds.isSolid))
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
                            if (solidTilesOnly)
                            {
                                if (tileUnderBounds.isSolid)
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
        foreach (Map map in activeMaps)
        {
            if (map.IsWithinBounds(pos) && (closest = map.TileAt(pos)) != null)
            {
                return closest;
            }
        }


        foreach (Map map in activeMaps)
        {
            foreach (Tile tile in map.childTiles)
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
        if (positionRight)
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
        if (mostRecentMap != null)
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
