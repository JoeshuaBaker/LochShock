using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodadManager : MonoBehaviour
{
    public Player player;
    public int offPathBasicDoodadArraySize;
    public GameObject[] offPathBasicDoodadArray;
    public List<GameObject> offPathBasicDoodads;
    public float doodadHoriLimit = 22;
    public float doodadVertLimit = 12;

    public int onPathBasicDoodadArraySize;
    public GameObject[] onPathBasicDoodadArray;
    public List<GameObject> onPathBasicDoodads;


    void Start()
    {
        player = Player.activePlayer;

        var initialPos = player.transform.position;

        offPathBasicDoodadArray = new GameObject[offPathBasicDoodadArraySize];
        onPathBasicDoodadArray = new GameObject[onPathBasicDoodadArraySize];

        for ( int i = 0 ; i < offPathBasicDoodadArray.Length ; i++)
        {
            offPathBasicDoodadArray[i] = Instantiate(offPathBasicDoodads[Random.Range(0, offPathBasicDoodads.Count)]);
            offPathBasicDoodadArray[i].transform.parent = this.transform;
            offPathBasicDoodadArray[i].transform.position = new Vector3 ((initialPos.x + Random.Range( -doodadHoriLimit , doodadHoriLimit)), (initialPos.y + Random.Range( -doodadVertLimit , doodadVertLimit)), 0f);
        }

        for (int i = 0; i < onPathBasicDoodadArray.Length; i++)
        {
            onPathBasicDoodadArray[i] = Instantiate(onPathBasicDoodads[Random.Range(0, onPathBasicDoodads.Count)]);
            onPathBasicDoodadArray[i].transform.parent = this.transform;
            onPathBasicDoodadArray[i].transform.position = new Vector3((initialPos.x + Random.Range(-doodadHoriLimit, doodadHoriLimit)), (initialPos.y + Random.Range(-doodadVertLimit, doodadVertLimit)), 0f);
        }
    }

    void Update()
    {

        var playerPos = player.transform.position;

        for (int i=0; i< offPathBasicDoodadArray.Length; i++)
        {
            if (offPathBasicDoodadArray[i].transform.position.x < (playerPos.x - doodadHoriLimit))
            {
                offPathBasicDoodadArray[i].transform.position = new Vector3((playerPos.x + doodadHoriLimit), (playerPos.y + Random.Range(-doodadVertLimit, doodadVertLimit)), 0f);
            }
            
            else if (offPathBasicDoodadArray[i].transform.position.y < (playerPos.y - doodadVertLimit))
            {
                offPathBasicDoodadArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-doodadHoriLimit , doodadHoriLimit)), (playerPos.y + doodadVertLimit), 0f);
            }

            else if (offPathBasicDoodadArray[i].transform.position.y > (playerPos.y + doodadVertLimit))
            {
                offPathBasicDoodadArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-doodadHoriLimit, doodadHoriLimit)), (playerPos.y - doodadVertLimit), 0f);
            }
        }

        for (int i = 0; i < onPathBasicDoodadArray.Length; i++)
        {
            if (onPathBasicDoodadArray[i].transform.position.x < (playerPos.x - doodadHoriLimit))
            {
                onPathBasicDoodadArray[i].transform.position = new Vector3((playerPos.x + doodadHoriLimit), (playerPos.y + Random.Range(-doodadVertLimit, doodadVertLimit)), 0f);
            }

            else if (onPathBasicDoodadArray[i].transform.position.y < (playerPos.y - doodadVertLimit))
            {
                onPathBasicDoodadArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-doodadHoriLimit, doodadHoriLimit)), (playerPos.y + doodadVertLimit), 0f);
            }

            else if (onPathBasicDoodadArray[i].transform.position.y > (playerPos.y + doodadVertLimit))
            {
                onPathBasicDoodadArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-doodadHoriLimit, doodadHoriLimit)), (playerPos.y - doodadVertLimit), 0f);
            }
        }
    }
}
