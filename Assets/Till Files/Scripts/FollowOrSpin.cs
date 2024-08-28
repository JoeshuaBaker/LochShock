using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOrSpin : MonoBehaviour
{

    public bool follow = true;
    public GameObject player;


    public bool spin = true;
    public float rotationsPerSecond = 1f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (follow)
        {
            this.transform.position = new Vector3(player.transform.position.x,
                                                    player.transform.position.y,
                                                    player.transform.position.z);
        }

        if (spin)
        {
            this.transform.Rotate(new Vector3(0, 0, 360 * rotationsPerSecond * Time.deltaTime));
        }
    }
       
}

