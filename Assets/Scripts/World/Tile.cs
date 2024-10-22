using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isSolid;
    public bool startTile;
    public bool endTile;
    public bool emitterSpawnLocation;
    public Collider2D collider2d;

    private void Reset()
    {
        collider2d = GetComponent<Collider2D>();
    }

    public void Start()
    {
        if(collider2d == null)
        {
            collider2d = GetComponent<Collider2D>();
        }
    }
}
