using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterEyeController : MonoBehaviour
{
    public Animator animator;

    public GameObject centerEyeMain;
    public GameObject centerEyeFlap;
    public GameObject centerEyeBonusEye;
    public GameObject centerEyeLargeIris;
    public GameObject centerEyeLargePupil;
    public GameObject centerEyeSmallIris;
    public GameObject centerEyeSmallPupil;

    public float eyeBob = 8f;
    public float flapDampening = 0.8f;
    public float flapOffset = -1f;

    public float eyeRot = 3f;
    public float rotOffset = 0f;

    public bool moveEye = true;
    public float minTimeToMove = 0.5f;
    public float maxTimeToMove = 5f;
    public float eyeMoveRange = 12f;
    public Vector3 eyeTargetPos;
    public float eyeMoveTime;
    public float eyeMoveTimeCurrent;
    public Vector3 eyeStartPos;
    public float eyeMoveTransition;
    public float eyeTransitionTime = 0.5f;

    public bool moveEyeSmall = true;
    public float eyeSmallMoveRange = 6f;
    private Vector3 eyeSmallTargetPos;
    private float eyeSmallMoveTime;
    private float eyeSmallMoveTimeCurrent;
    private Vector3 eyeSmallStartPos;
    private float eyeSmallMoveTransition;

    public bool eyeActive;
    public GameObject eyeParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (eyeActive)
        { 
            AnimateCenterEye();
        }
    }

    public void AnimateCenterEye()
    {
        centerEyeMain.transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time) * eyeBob, 0f);
        centerEyeFlap.transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time + flapOffset) * eyeBob * flapDampening, 0f);
        centerEyeBonusEye.transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time - (flapOffset * -2f)) * eyeBob * flapDampening, 0f);

        var rot = centerEyeMain.transform.localRotation;

        rot.eulerAngles = new Vector3(0f, 0f, Mathf.Sin(Time.time * 0.5f + rotOffset) * eyeRot);

        centerEyeMain.transform.localRotation = rot;

        if (moveEye)
        {
            centerEyeLargeIris.transform.localPosition = new Vector2(Mathf.Lerp(eyeStartPos.x, eyeTargetPos.x, eyeMoveTransition), Mathf.Lerp(eyeStartPos.y, eyeTargetPos.y, eyeMoveTransition));
            centerEyeLargePupil.transform.localPosition = new Vector2(Mathf.Lerp(eyeStartPos.x, eyeTargetPos.x, eyeMoveTransition), Mathf.Lerp(eyeStartPos.y, eyeTargetPos.y, eyeMoveTransition));

            eyeMoveTransition += (Time.deltaTime / eyeTransitionTime);

            if (eyeMoveTransition > 1f)
            {
                moveEye = false;

                eyeMoveTime = Random.Range(minTimeToMove, maxTimeToMove);
            }

        }
        else
        {

            eyeMoveTimeCurrent += Time.deltaTime;
            if (eyeMoveTimeCurrent > eyeMoveTime)
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

    public void CenterEyeEnter()
    {
        eyeActive = true;
        //eyeParent.SetActive(true);
        animator.Play("MiddleEyeIntro");

    }

    public void CenterEyeExit()
    {
        eyeActive = false;
        animator.Play("MiddleEyeOutro");
    }

}
