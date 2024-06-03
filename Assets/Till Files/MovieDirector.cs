using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MovieDirector : MonoBehaviour
{
    public Camera Cam;
    public GameObject VideoPlayerObject;
    public long holdFrame = 77;   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Gets Video from the player Object
        var Video = VideoPlayerObject.gameObject.GetComponent<VideoPlayer>();

        //Checks for frame top stop on
        if(Video.frame == holdFrame)
        {
            Video.Pause();

            //Checks for mouse click after stop
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Video.frame = 81;
                Video.Play();
                //holdFrame = 0;
            }
        }

        Debug.Log(Video.frame);
    }
}
