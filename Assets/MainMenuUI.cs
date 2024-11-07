using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Animator animator;
    public Animator animatorOverlay;
    public GameObject steamText;

    public CenterEyeController centerEye;

    public Image visionBlocker;
    public bool blockerAlphaUp;
    public bool fadeBlocker;

    public GameObject[] icons;
    public GameObject[] iconsGlow;
    public GameObject[] iconsText;
    public GameObject[] uiArms;
    public Image steamGlow;

    public float glowAlphaMax = 0.75f;
    public float glowAlphaMin = 0.25f;

    public float bounceIcon;
    public float bounceText;

    public float bounceOffset = 0.1f;
    public float bounceOffsetText = 0.15f;
    public float bounceDistance = 100f;

    public float bounceSpeed = 1f;
    public float bounceSpeedArms = .5f;
    public float bounceDistanceArms = 3f;
    public float bounceOffsetArms;

    public float glowFloat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeBlocker)
        {
            UpdateBlocker();
        }

        AnimateMainMenu();

    }

    public void AnimateMainMenu()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            Vector3 bounceAmount = new Vector3(0f, -bounceDistance * Mathf.Sin((Time.time * bounceSpeed) + (bounceOffset * (i + 1))), 0f);

            icons[i].transform.localPosition = bounceAmount;
            iconsGlow[i].transform.localPosition = bounceAmount;

            if (steamGlow != null)
            {
                Color glowColor = steamGlow.color;

                glowFloat = glowAlphaMax * 0.5f * (1f + Mathf.Sin((Time.time * bounceSpeed) + (bounceOffset * (i + 1))));

                glowColor.a = (1f - glowFloat) + glowAlphaMin;

                steamGlow.color = glowColor;
            }

        }

        for (int i = 0; i < iconsText.Length; i++)
        {
            Vector3 bounceAmount = new Vector3(0f, -bounceDistance * Mathf.Sin((Time.time * bounceSpeed) + (bounceOffset * (i + 1) + bounceOffsetText)), 0f);

            iconsText[i].transform.localPosition = bounceAmount;
        }

        for (int i = 0; i < uiArms.Length; i++)
        {
            float sinBounce = -bounceDistanceArms * Mathf.Sin((Time.time * bounceSpeedArms) + (bounceOffsetArms * (i + 1) + bounceOffsetText));

            Vector3 bounceAmount = new Vector3(sinBounce * 0.5f, sinBounce, 0f);

            //uiArms[i].transform.localPosition = bounceAmount;

            var rot = uiArms[i].transform.localRotation;

            rot.eulerAngles = new Vector3(0f, 0f, sinBounce * 0.15f);

            uiArms[i].transform.localRotation = rot;
        }

        Vector3 bounce = new Vector3(0f, (-bounceDistance * 2f) * Mathf.Sin((Time.time * (bounceSpeed) + (bounceOffset * (3) + bounceOffsetText))), 0f);

        steamText.transform.localPosition = bounce;
    }

    public void OnPlayButtonPressed()
    {

    }

    public void OnSettingsButtonPressed()
    {

    }

    public void OnCreditsButtonPressed()
    {

    }

    public void OnExitButtonPressed()
    {

    }

    public void UpdateBlocker()
    {

        if (blockerAlphaUp)
        {
            var visColor = visionBlocker.color;

            if (visColor.a >= 1f)
            {
                visColor.a = 1f;

                fadeBlocker = false;
            }

            visColor.a += Time.deltaTime * 5f;

            visionBlocker.color = visColor;
        }
        else
        {
            var visColor = visionBlocker.color;

            if (visColor.a <= 0f)
            {
                visColor.a = 0f;

                fadeBlocker = false;

                visionBlocker.gameObject.SetActive(false);

            }

            visColor.a -= Time.deltaTime * 5f;

            visionBlocker.color = visColor;
        }
    }

    public void BlockerFadeIn()
    {
        blockerAlphaUp = true;
        fadeBlocker = true;

        visionBlocker.gameObject.SetActive(true);

        animator.Play("MainMenuOutro");
        animatorOverlay.Play("MainMenuOutro");

    }

    public void BlockerFadeOut()
    {
        blockerAlphaUp = false;
        fadeBlocker = true;

        animator.Play("MainMenuIntro");
        animatorOverlay.Play("MainMenuIntro");

    }

    public void CenterEyeIn()
    {
        centerEye.CenterEyeEnter();
    }

    public void CenterEyeOut()
    {
        centerEye.CenterEyeExit();
    }
}
