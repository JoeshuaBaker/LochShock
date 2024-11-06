using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMenu : MonoBehaviour
{

    public Image[] scrollScreens;
    public Sprite[] scrollScreenSprites;
    public float scrollScreenAnimDuration;
    public float scrollScreenAnimTimeCurrent;
    public float scrollOffset;
    public float timePerFrame;
    public GameObject[] centerCollumnSegments;
    public GameObject[] supports;
    public GameObject[] smallEyes;

    private float wiggle;
    public float wiggleAmount;
    public float wiggleOffset;
    public float wiggleSpeed;
    public float wiggleSupportDampening;

    public GameObject centerEyeMain;
    public GameObject centerEyeFlap;
    public GameObject centerEyeBonusEye;
    public GameObject centerEyeLargeIris;
    public GameObject centerEyeLargePupil;
    public GameObject centerEyeSmallIris;
    public GameObject centerEyeSmallPupil;

    public GameObject characterPanel;
    public GameObject levelPanel;
    public GameObject characterPanelRot;
    public GameObject levelPanelRot;

    public GameObject terrorRing;

    public float panelWiggle = 0.8f;
    public float panelRot = 0.2f;
        

    public float eyeBob;
    public float flapDampening;
    public float flapOffset;

    public float eyeRot;
    public float rotOffset;

    public bool moveEye = true;
    public float minTimeToMove = 0.5f;
    public float maxTimeToMove = 5f;
    public float eyeMoveRange = 10f;
    public Vector3 eyeTargetPos;
    public float eyeMoveTime;
    public float eyeMoveTimeCurrent;
    public Vector3 eyeStartPos;
    public float eyeMoveTransition;
    public float eyeTransitionTime = 1f;

    public bool moveEyeSmall = true;
    public float eyeSmallMoveRange = 6f;
    private Vector3 eyeSmallTargetPos;
    private float eyeSmallMoveTime;
    private float eyeSmallMoveTimeCurrent;
    private Vector3 eyeSmallStartPos;
    private float eyeSmallMoveTransition;



    void Start()
    {
        
    }

    void Update()
    {
        scrollScreenAnimTimeCurrent = Time.time;

        AnimateScroll();

        AnimateCenter();

        AnimateCenterEye();

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

    public void AnimateCenterEye()
    {
        centerEyeMain.transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time) * eyeBob, 0f);
        centerEyeFlap.transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time + flapOffset) * eyeBob * flapDampening, 0f);
        centerEyeBonusEye.transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time - (flapOffset * -2f)) * eyeBob * flapDampening, 0f);

        var rot = centerEyeMain.transform.localRotation;

        rot.eulerAngles = new Vector3 (0f, 0f, Mathf.Sin(Time.time * 0.5f + rotOffset) * eyeRot);

        centerEyeMain.transform.localRotation = rot;

        if (moveEye)
        {
            centerEyeLargeIris.transform.localPosition = new Vector2(Mathf.Lerp(eyeStartPos.x, eyeTargetPos.x, eyeMoveTransition), Mathf.Lerp(eyeStartPos.y, eyeTargetPos.y, eyeMoveTransition));
            centerEyeLargePupil.transform.localPosition = new Vector2(Mathf.Lerp(eyeStartPos.x, eyeTargetPos.x, eyeMoveTransition), Mathf.Lerp(eyeStartPos.y, eyeTargetPos.y, eyeMoveTransition));

            eyeMoveTransition += (Time.deltaTime / eyeTransitionTime);

            if(eyeMoveTransition > 1f)
            {
                moveEye = false;

                eyeMoveTime = Random.Range(minTimeToMove, maxTimeToMove);
            }

        }
        else
        {

            eyeMoveTimeCurrent += Time.deltaTime;
            if(eyeMoveTimeCurrent > eyeMoveTime)
            {
                moveEye = true;

                eyeMoveTimeCurrent = 0f;

                eyeStartPos = centerEyeLargeIris.transform.localPosition;

                eyeTargetPos = new Vector2(Random.Range(-eyeMoveRange, eyeMoveRange), Random.Range(-eyeMoveRange, eyeMoveRange));

                eyeMoveTransition = 0f;
            }
        }

        if (moveEyeSmall)
        {
            centerEyeSmallIris.transform.localPosition = new Vector2(Mathf.Lerp(eyeSmallStartPos.x, eyeSmallTargetPos.x, eyeSmallMoveTransition), Mathf.Lerp(eyeSmallStartPos.y, eyeSmallTargetPos.y, eyeSmallMoveTransition));
            centerEyeSmallPupil.transform.localPosition = new Vector2(Mathf.Lerp(eyeSmallStartPos.x, eyeSmallTargetPos.x, eyeSmallMoveTransition), Mathf.Lerp(eyeSmallStartPos.y, eyeSmallTargetPos.y, eyeSmallMoveTransition));

            eyeSmallMoveTransition += (Time.deltaTime / eyeTransitionTime);

            if (eyeSmallMoveTransition > 1f)
            {
                moveEyeSmall = false;

                eyeSmallMoveTime = Random.Range(minTimeToMove, maxTimeToMove);
            }

        }
        else
        {

            eyeSmallMoveTimeCurrent += Time.deltaTime;
            if (eyeSmallMoveTimeCurrent > eyeSmallMoveTime)
            {
                moveEyeSmall = true;

                eyeSmallMoveTimeCurrent = 0f;

                eyeSmallStartPos = centerEyeSmallIris.transform.localPosition;

                eyeSmallTargetPos = new Vector2(Random.Range(-eyeSmallMoveRange, eyeSmallMoveRange), Random.Range(-eyeSmallMoveRange, eyeSmallMoveRange));

                eyeSmallMoveTransition = 0f;
            }
        }

    }
}
