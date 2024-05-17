using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int xOffset = 0;
    public int xMax = 0;
    public int yOffset = 0;
    public int yMax = 0;
    public Tile[,] tiles;
    public List<Tile> childTiles;
    public List<Tile> startTiles;
    public List<Tile> endTiles;
    public List<Tile> emitterSpawnTiles;

    private void Start()
    {
        tiles = new Tile[xMax, yMax];
        foreach(var tile in childTiles)
        {
            int x = (int)(tile.transform.localPosition.x + xOffset);
            int y = (int)(tile.transform.localPosition.y + yOffset);
            tiles[x, y] = tile;
        }
    }

    void Reset()
    {
        startTiles = new List<Tile>();
        endTiles = new List<Tile>();
        emitterSpawnTiles = new List<Tile>();
        childTiles = new List<Tile>(GetComponentsInChildren<Tile>());

        //calculate xyoffsets to normalize indexes to 0,0 so we can store in array by index
        foreach(var tile in childTiles)
        {
            if (tile.transform.localPosition.x < xOffset)
            {
                xOffset = (int)tile.transform.localPosition.x;
            }
            if(tile.transform.localPosition.x > xMax)
            {
                xMax = (int)tile.transform.localPosition.x;
            }

            if (tile.transform.localPosition.y < yOffset)
            {
                yOffset = (int)tile.transform.localPosition.y;
            }
            if(tile.transform.localPosition.y > yMax)
            {
                yMax = (int)tile.transform.localPosition.y;
            }
        }


        xOffset = Mathf.Abs(xOffset);
        yOffset = Mathf.Abs(yOffset);
        xMax += xOffset + 1;
        yMax += yOffset + 1;

        foreach(var tile in childTiles)
        {
            if(tile.startTile)
            {
                startTiles.Add(tile);
            }
            if(tile.endTile)
            {
                endTiles.Add(tile);
            }
            if(tile.emitterSpawnLocation)
            {
                emitterSpawnTiles.Add(tile);
            }
        }
    }

    public bool IsWithinBounds(Vector3 position)
    {
        int lowerLeftCornerX = (int)this.transform.position.x - xOffset;
        int lowerLeftCornerY = (int)this.transform.position.y - yOffset;
        Vector3 lowerLeftCornerPos = new Vector3(lowerLeftCornerX, lowerLeftCornerY, 0);
        Vector3 dist = position - lowerLeftCornerPos;

        return dist.x >= 0 && dist.x < xMax && dist.y >= 0 && dist.y < yMax;
    }

    public Tile TileAt(Vector3 position)
    {
        int lowerLeftCornerX = (int)this.transform.position.x - xOffset;
        int lowerLeftCornerY = (int)this.transform.position.y - yOffset;
        Vector3 lowerLeftCornerPos = new Vector3(lowerLeftCornerX, lowerLeftCornerY, 0);
        Vector3 dist = position - lowerLeftCornerPos;
        int x = (int)(dist.x);
        int y = (int)(dist.y);

        if (x >= 0 && x < xMax && y >= 0 && y < yMax)
        {
            return tiles[x, y];
        }

        return null;
    }
}
