using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMenu : MonoBehaviour
{

    public Animator panelAnimator;

    public Image[] scrollScreens;
    public Sprite[] scrollScreenSprites;
    public float scrollScreenAnimDuration;
    public float scrollScreenAnimTimeCurrent;
    public float scrollOffset;
    public float timePerFrame;
    public GameObject[] centerCollumnSegments;
    public GameObject[] supports;

    private float wiggle;
    public float wiggleAmount;
    public float wiggleOffset;
    public float wiggleSpeed;
    public float wiggleSupportDampening;

    public GameObject characterPanel;
    public GameObject levelPanel;
    public GameObject characterPanelRot;
    public GameObject levelPanelRot;

    public GameObject terrorRing;

    public float panelWiggle = 0.8f;
    public float panelRot = 0.2f;
        


    void Start()
    {
        
    }

    void Update()
    {
        scrollScreenAnimTimeCurrent = Time.time;

        AnimateScroll();

        AnimateCenter();

    }

    public void AnimateScroll()
    { 
        for(int i = 0; i < scrollScreens.Length; i++)
        {

            float scrollSpeedModified = scrollScreenAnimTimeCurrent + (i * scrollOffset);

            if (scrollScreenSprites == null || scrollScreenSprites.Length == 0)
            {
                return;
            }

            scrollSpeedModified %= scrollScreenAnimDuration;

            timePerFrame = scrollScreenAnimDuration / scrollScreenSprites.Length;

            scrollScreens[i].sprite = scrollScreenSprites[(int)(scrollSpeedModified / timePerFrame)];
        }
    }

    public void AnimateCenter()
    {
        for( int i = 0; i < centerCollumnSegments.Length; i++)
        {
            wiggle = Mathf.Sin(Time.time + (wiggleOffset * i) * wiggleSpeed) * wiggleAmount;

            centerCollumnSegments[i].transform.localPosition = new Vector3(wiggle, 0f, 0f);

            supports[i].transform.localPosition = new Vector3(wiggle * wiggleSupportDampening, 0f, 0f);

        }

        // moving and rotating the panels
        characterPanelRot.transform.localEulerAngles = new Vector3( 0f, 0f, wiggle * panelRot);
        levelPanelRot.transform.localEulerAngles = new Vector3(0f, 0f, wiggle * panelRot);

        characterPanel.transform.localPosition = new Vector3((int)(wiggle * panelWiggle), 0f, 0f);
        levelPanel.transform.localPosition = new Vector3((int)(wiggle * panelWiggle), 0f, 0f);

        float ringX = Mathf.Sin(Time.time)* 5f;
        float ringY = Mathf.Sin(Time.time * 2f) * 2.5f;

        terrorRing.transform.localPosition = new Vector3(ringX, ringY , 0f);
        terrorRing.transform.localEulerAngles = new Vector3(0f, 0f, wiggle * -panelRot);

    }

    public void OnPlayButtonPressed()
    {
        panelAnimator.Play("MissionSelectUiIntro");
    }

    public void OnConfirmCharacter()
    {
        // go to level select

        panelAnimator.Play("MissionSelectCharacterToLevel");

    }

    public void OnConfirmLevel()
    {
        // load game
    }

    public void OnReturnFromCharacterSelect()
    {
        // back to main menu
        panelAnimator.Play("MissionSelectUiOutro");
    }

    public void OnReturnFromLevelSelect()
    {
        //back to character select
        panelAnimator.Play("MissionSelectLevelToCharacter");
    }

}
