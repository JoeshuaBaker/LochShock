using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodadManager : MonoBehaviour
{
    public Player player;
    public int rootsArraySize;
    public GameObject[] rootsHighArray;
    public GameObject rootsHigh;
    public GameObject[] rootsMidArray;
    public GameObject rootsMid;
    public GameObject[] rootsLowArray;
    public GameObject rootsLow;
    public float rootHoriLimit = 22;
    public float rootVertLimit = 12;
    public float rootTargetNumber;
    public float rootCurrentNumber;
    public float rootHoldTime;
    public float rootTransitionTime;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.activePlayer;

        var initialPos = player.transform.position;

        rootTargetNumber = rootsArraySize;

        ////for controlling roots number over time
        //rootTargetNumber = Random.Range(0, rootsArraySize);
        //rootCurrentNumber = rootTargetNumber;

        //rootHoldTime = Random.Range(5f , 20f);
        //rootTransitionTime = Random.Range(3f, 10f);

        rootsHighArray = new GameObject[rootsArraySize];
        rootsMidArray = new GameObject[rootsArraySize];
        rootsLowArray = new GameObject[rootsArraySize];

        for ( int i = 0 ; i < rootsHighArray.Length ; i++)
        {
            rootsHighArray[i] = Instantiate(rootsHigh);
            rootsHighArray[i].transform.parent = this.transform;
            rootsHighArray[i].transform.position = new Vector3 ((initialPos.x + Random.Range( -rootHoriLimit , rootHoriLimit)), (initialPos.y + Random.Range( -rootVertLimit , rootVertLimit)), 0f);

            if (i > rootTargetNumber)
            {
                rootsHighArray[i].SetActive(false);
            }
        }

        for (int i = 0; i < rootsMidArray.Length; i++)
        {
            rootsMidArray[i] = Instantiate(rootsMid);
            rootsMidArray[i].transform.parent = this.transform;
            rootsMidArray[i].transform.position = new Vector3((initialPos.x + Random.Range( -rootHoriLimit , rootHoriLimit)), (initialPos.y + Random.Range( -rootVertLimit , rootVertLimit)), 0f);

            if (i > rootTargetNumber)
            {
                rootsMidArray[i].SetActive(false);
            }
        }

        for (int i = 0; i < rootsLowArray.Length; i++)
        {
            rootsLowArray[i] = Instantiate(rootsLow);
            rootsLowArray[i].transform.parent = this.transform;
            rootsLowArray[i].transform.position = new Vector3((initialPos.x + Random.Range( -rootHoriLimit , rootHoriLimit)), (initialPos.y + Random.Range( -rootVertLimit , rootVertLimit)), 0f);

            if (i > rootTargetNumber)
            {
                rootsLowArray[i].SetActive(false);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        var playerPos = player.transform.position;

        ////for changing the number of roots over time
        //if (rootCurrentNumber == rootTargetNumber)
        //{
        //    rootHoldTime = rootHoldTime - Time.deltaTime;

        //    if (rootHoldTime <= 0f)
        //    {
        //        rootTargetNumber = Random.Range(0, rootsArraySize);
        //        rootHoldTime = Random.Range(5f, 20f);
        //        rootTransitionTime = Random.Range(3f, 10f);
        //    }

        //}
       
        //if (rootCurrentNumber > rootTargetNumber)
        //{
        //    rootCurrentNumber = rootCurrentNumber + (rootCurrentNumber - rootTargetNumber / rootTransitionTime);
        //}

     

        for (int i=0; i< rootsHighArray.Length; i++)
        {
            if (rootsHighArray[i].transform.position.x < (playerPos.x - rootHoriLimit))
            {
                rootsHighArray[i].transform.position = new Vector3((playerPos.x + rootHoriLimit), (playerPos.y + Random.Range(-rootVertLimit, rootVertLimit)), 0f);
            }
            
            else if (rootsHighArray[i].transform.position.y < (playerPos.y - rootVertLimit))
            {
                rootsHighArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-rootHoriLimit , rootHoriLimit)), (playerPos.y + rootVertLimit), 0f);
            }

            else if (rootsHighArray[i].transform.position.y > (playerPos.y + rootVertLimit))
            {
                rootsHighArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-rootHoriLimit, rootHoriLimit)), (playerPos.y - rootVertLimit), 0f);
            }

        }

        for (int i = 0; i < rootsMidArray.Length; i++)
        {
            if (rootsMidArray[i].transform.position.x < (playerPos.x - rootHoriLimit))
            {
                rootsMidArray[i].transform.position = new Vector3((playerPos.x + rootHoriLimit), (playerPos.y + Random.Range(-rootVertLimit, rootVertLimit)), 0f);
            }

            else if (rootsMidArray[i].transform.position.y < (playerPos.y - rootVertLimit))
            {
                rootsMidArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-rootHoriLimit, rootHoriLimit)), (playerPos.y + rootVertLimit), 0f);
            }

            else if (rootsMidArray[i].transform.position.y > (playerPos.y + rootVertLimit))
            {
                rootsMidArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-rootHoriLimit, rootHoriLimit)), (playerPos.y - rootVertLimit), 0f);
            }

        }

        for (int i = 0; i < rootsLowArray.Length; i++)
        {
            if (rootsLowArray[i].transform.position.x < (playerPos.x - rootHoriLimit))
            {
                rootsLowArray[i].transform.position = new Vector3((playerPos.x + rootHoriLimit), (playerPos.y + Random.Range(-rootVertLimit, rootVertLimit)), 0f);
            }

            else if (rootsLowArray[i].transform.position.y < (playerPos.y - rootVertLimit))
            {
                rootsLowArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-rootHoriLimit, rootHoriLimit)), (playerPos.y + rootVertLimit), 0f);
            }

            else if (rootsLowArray[i].transform.position.y > (playerPos.y + rootVertLimit))
            {
                rootsLowArray[i].transform.position = new Vector3((playerPos.x + Random.Range(-rootHoriLimit, rootHoriLimit)), (playerPos.y - rootVertLimit), 0f);
            }

        }
    }
}
