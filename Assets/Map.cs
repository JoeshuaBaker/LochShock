using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public List<Tile> startTiles;
    public List<Tile> endTiles;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Reset()
    {
        startTiles = new List<Tile>();
        endTiles = new List<Tile>();
        IEnumerable<Tile> tiles = GetComponentsInChildren<Tile>();
        foreach(var tile in tiles)
        {
            if(tile.startTile)
            {
                startTiles.Add(tile);
            }
            if(tile.endTile)
            {
                endTiles.Add(tile);
            }
        }
    }


}
