using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapPool))]
public class World : MonoBehaviour
{
    public MapPool mapPool;
    public Player player;
    public int numMapsLoaded = 5;
    Queue<Map> mapQueue;
    List<Map> loadedMaps;

    // Start is called before the first frame update
    void Start()
    {
        mapPool = GetComponent<MapPool>();
        mapQueue = new Queue<Map>(Shuffle(mapPool.maps));
        loadedMaps = new List<Map>();

        Map last = null;
        for(int i = 0; i < numMapsLoaded; i++)
        {
            Map map = mapQueue.Dequeue();
            map = Instantiate(map, this.transform);
            loadedMaps.Add(map);

            if(last != null)
            {
                PositionMap(last, map);
            }
            last = map;
        }
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

    public static List<T> Shuffle<T>(IList<T> list)
    {
        List<T> listCopy = new List<T>(list);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = listCopy[k];
            listCopy[k] = listCopy[n];
            listCopy[n] = value;
        }

        return listCopy;
    }
}
