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
    private float playerDistance = 10.0f;

    public bool bossDead;


    // Start is called before the first frame update
    void Start()
    {
        player = Player.activePlayer;
        wallDeath = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetDistanceFromPlayer();
        MoveWall();
        WallDamage();
    }

    private void MoveWall()
    {
        if (!bossDead)
        {
            //float posX = deathWall.transform.position.x;
            posX = deathWall.transform.position.x;

            posX = Mathf.Max(posX + (speed * Time.deltaTime), player.transform.position.x - maxDistance);

            deathWall.transform.position = new Vector3(posX, player.transform.position.y, deathWall.transform.position.z);
        }
        else
        {
            posX = player.transform.position.x - 50f;
            deathWall.transform.position = new Vector3(posX, deathWall.transform.position.y, 0f);
        }

    }

    void WallDamage()
    {
        if (player.transform.position.x + xDamageOffset <= posX)
        {
            if (!wallDeath && !player.isDead) { player.dieWallPS.Play(); player.KillSelf(); player.UpdateHp(-100); player.Execute();  }
            wallDeath = true;
        }
    }

    //Sets deathwall audio based on distance from player
    public void SetDistanceFromPlayer()
    {
        playerDistance = Vector3.Distance(deathWall.gameObject.transform.position, Player.activePlayer.gameObject.transform.position);
        AkSoundEngine.SetRTPCValue("DistanceFromDeathWall", playerDistance);

        Debug.Log("Deathwall Distance From Player = " + playerDistance);

        return;
    }
}
