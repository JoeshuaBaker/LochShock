using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    public GameObject deathWall;
    public Player player;
    public float speed = 0.1f;
    public float maxDistance = 25f;
    public int xDamageOffset = 0;

    private float posX = 0;
    private bool wallDeath = false;


    // Start is called before the first frame update
    void Start()
    {
        player = Player.activePlayer;
        wallDeath = false;
    }

    // Update is called once per frame
    void Update()
    {
        MoveWall();
        WallDamage();
    }

    private void MoveWall()
    {
        //float posX = deathWall.transform.position.x;
        posX = deathWall.transform.position.x;

        posX = Mathf.Max(posX + (speed * Time.deltaTime), player.transform.position.x - maxDistance);

        deathWall.transform.position = new Vector3(posX, player.transform.position.y, deathWall.transform.position.z);
    }

    void WallDamage()
    {
        if (player.transform.position.x + xDamageOffset <= posX)
        {
            if (!wallDeath && !player.isDead) { player.dieWallPS.Play(); player.KillSelf(); player.UpdateHp(-100); player.Execute();  }
            wallDeath = true;
        }
    }
}
