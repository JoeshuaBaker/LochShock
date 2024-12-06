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
    public GameObject menuCursor;
    public Image menuContextImage;
    public Sprite recycleSprite;
    public Sprite levelUpSprite;

    public bool showRecycle;
    public bool showLevelUp;

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
        //var screenPoint = Input.mousePosition;
        //screenPoint.z = 19.5f;

        if (World.activeWorld.paused)
        {
            menuCursor.SetActive(true);
            crosshairVis.SetActive(false);
        }
        else
        {
            menuCursor.SetActive(false);
            crosshairVis.SetActive(true);
        }
        


    }

    public void UpdateCrosshairPosition(Vector2 position)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(new Vector3(position.x, position.y, 0f));
        crosshair.transform.position = new Vector3(screenPoint.x, screenPoint.y, 0f);
        crosshair.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    public void UpdateCrosshair(Vector2 position, bool reloading, float reloadFill, string currentAmmo)
    {
        if(ammoIndicator != null && reloadIndicator != null)
        {
            ammoIndicator.text = currentAmmo;
            ammoIndicatorShadow.text = currentAmmo;
            reloadIndicator.gameObject.SetActive(reloading);

            UpdateCrosshairPosition(position);

            if (reloading)
            {
                reloadIndicator.fillAmount = reloadFill;
            }
        }
    }

    public void ToggleRecycle()
    {
        if (!showRecycle || showLevelUp)
        {
            menuContextImage.sprite = recycleSprite;
            menuContextImage.gameObject.SetActive(true);
            showLevelUp = false;
            showRecycle = true;
        }
        else
        {
            menuContextImage.gameObject.SetActive(false);
            showRecycle = false;
        }
    }

    public void ToggleLevelUp()
    {
        if (!showLevelUp || showRecycle)
        {
            menuContextImage.sprite = levelUpSprite;
            menuContextImage.gameObject.SetActive(true);
            showRecycle = false;
            showLevelUp = true;
        }
        else
        {
            menuContextImage.gameObject.SetActive(false);
            showLevelUp = false;
        }
    }

    public void AllTogglesOff()
    {
        showRecycle = false;
        showLevelUp = false;
        menuContextImage.gameObject.SetActive(false);
    }
}
