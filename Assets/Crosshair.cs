using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crosshair : MonoBehaviour
{
    public GameObject crosshair;
    public Image reloadIndicator;
    public TextMeshProUGUI ammoIndicator;
    public TextMeshProUGUI ammoIndicatorShadow;
    public GameObject crosshairVis;

    public static Crosshair activeCrosshair;

    private void Awake()
    {
        activeCrosshair = this;
    }

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

        crosshair.transform.position = new Vector3( screenPoint.x , screenPoint.y , 0f );
        crosshair.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    public void UpdateCrosshair(bool reloading, float reloadFill, string currentAmmo)
    {
        ammoIndicator.text = currentAmmo;
        ammoIndicatorShadow.text = currentAmmo;
        reloadIndicator.gameObject.SetActive(reloading);

        if (reloading)
        {
            reloadIndicator.fillAmount = reloadFill;
        }
    }
}
