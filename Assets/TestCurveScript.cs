using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCurveScript : MonoBehaviour
{
    public BezierCurve curve;
    public int resolution = 16;
    int totalResolution = 0;
    public GameObject prefab;
    private GameObject[] links;
    // Start is called before the first frame update
    void Start()
    {
        curve = GetComponent<BezierCurve>();
        totalResolution = resolution * (curve.pointCount - 1);
        links = new GameObject[totalResolution+1];

        for(int i = 0; i <= totalResolution; i++)
        {
            links[i] = Instantiate(prefab, this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int index = 0;
        for(int i = 0; i <= totalResolution; i++)
        {
            Vector3 point = curve.GetPointAtDistance(i/(float)totalResolution);
            links[index].transform.position = point;
            links[index].SetActive(true);
            index++;
            Debug.Log(i / (float)totalResolution);
        }
    }
}
