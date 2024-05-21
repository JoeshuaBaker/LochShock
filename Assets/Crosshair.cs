using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public GameObject crosshair;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        var screenPoint = Input.mousePosition;
        screenPoint.z = 19.5f;

        crosshair.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        crosshair.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }
}
