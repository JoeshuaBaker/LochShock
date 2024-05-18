using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    public GameObject deathWall;
    public Player player;
    public float speed = 0.1f;
    public float maxDistance = 25f;


    // Start is called before the first frame update
    void Start()
    {
        player = Player.activePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        float posX = deathWall.transform.position.x;

        posX = Mathf.Max(posX + (speed * Time.deltaTime), player.transform.position.x - maxDistance);

        deathWall.transform.position = new Vector3(posX, player.transform.position.y, deathWall.transform.position.z);
    }
}
