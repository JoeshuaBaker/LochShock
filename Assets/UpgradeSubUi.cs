using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSubUi : MonoBehaviour
{

    public Animator animator;

    public GameObject[] panelComponents;

    public GameObject[] wingRoots;
    public GameObject[] feathersLeft;
    public GameObject[] feathersRight;

    public GameObject[] eyes;
    public GameObject[] flaps;
    public GameObject[] bars;

    public GameObject[] eyesRot;
    public GameObject[] finsClose;
    public GameObject[] finsFar;

    public GameObject[] orbs;

    public float panelBob = 8f;
    public float panelBobOffset = -.3f;

    public float wingRot = 3f;
    public float wingRotOffset = 1f;

    public float eyeRot;

    public GameObject[] iris;
    public GameObject[] pupil;

    public bool[] moveEye;
    public Vector3[] eyeStartPos;
    public Vector3[] eyeTargetPos;
    public float[] eyeMoveTimeCurrent;
    public float[] eyeMoveTransition;
    public float[] eyeMoveTime;
    public float eyeMoveRange = 12f;
    public float minTimeToMove = 0.5f;
    public float maxTimeToMove = 5f;
    public float eyeTransitionTime = 0.5f;

    public ItemDataFrame[] items;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AnimateDetails();
    }

    public void DismissUpgradeUi()
    {
        animator.Play("UpgradeSubUiOutro");

        for(int i = 0; i < items.Length; i++)
        {
            items[i].PlayCardOutro(-.2f);
        }
    }

    public void FocusUpgradeUi()
    {
        animator.Play("UpgradeSubUiIntro");

        for (int i = 0; i < items.Length; i++)
        {
            items[i].PlayCardIntro(-.2f, true);
        }
    }

    public void AnimateDetails()
    {

        for(int i = 0; i < panelComponents.Length; i++)
        {
            panelComponents[i].transform.localPosition = new Vector3(0f,(int)(Mathf.Sin(Time.time + (panelBobOffset * i)) * panelBob), 0f);
        }

        var quat = new Quaternion(0f, 0f, 0f, 0f);

        Vector3 rot = new Vector3(0f, 0f, Mathf.Sin(Time.time + wingRotOffset) * wingRot);

        quat.eulerAngles = rot;

        for(int i = 0; i < feathersLeft.Length; i++)
        {
            feathersLeft[i].transform.localRotation = quat;
            feathersRight[i].transform.localRotation = quat;
        }

        for(int i = 0; i < eyes.Length; i++)
        {
            eyes[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time + ( -0.8f)) * panelBob * 1.5f, 0f);
            if(i < 2)
            {
                flaps[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time + (-0.85f)) * panelBob, 0f);
                bars[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time + (-.9f)) * panelBob, 0f);
            }
            else
            {
                flaps[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time + (-0.7f)) * -panelBob, 0f);
                bars[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time + (-.6f)) * -panelBob, 0f);
            }
          }

        rot = new Vector3(0f, 0f, Mathf.Sin(Time.time - 0.1f) * eyeRot);

        quat.eulerAngles = rot;

        for (int i = 0; i< eyesRot.Length; i++)
        {
            eyesRot[i].transform.localRotation = quat;
        }

        rot = new Vector3(0f, 0f, Mathf.Sin(Time.time - 0.15f) * eyeRot);

        quat.eulerAngles = rot;

        for(int i = 0; i < finsClose.Length; i++)
        {
            finsClose[i].transform.localRotation = quat;
        }

        rot = new Vector3(0f, 0f, Mathf.Sin(Time.time - 0.3f) * eyeRot);

        quat.eulerAngles = rot;

        for (int i = 0; i < finsFar.Length; i++)
        {
            finsFar[i].transform.localRotation = quat;
        }


        for(int i = 0; i< moveEye.Length; i++)
        {
            if (moveEye[i])
            {
                iris[i].transform.localPosition = new Vector2(Mathf.Lerp(eyeStartPos[i].x, eyeTargetPos[i].x, eyeMoveTransition[i]), Mathf.Lerp(eyeStartPos[i].y, eyeTargetPos[i].y, eyeMoveTransition[i]));
                pupil[i].transform.localPosition = new Vector2(Mathf.Lerp(eyeStartPos[i].x, eyeTargetPos[i].x, eyeMoveTransition[i]), Mathf.Lerp(eyeStartPos[i].y, eyeTargetPos[i].y, eyeMoveTransition[i]));

                eyeMoveTransition[i] += (Time.deltaTime / eyeTransitionTime);

                if (eyeMoveTransition[i] > 1f)
                {
                    moveEye[i] = false;

                    eyeMoveTime[i] = Random.Range(minTimeToMove, maxTimeToMove);
                }

            }
            else
            {

                eyeMoveTimeCurrent[i] += Time.deltaTime;
                if (eyeMoveTimeCurrent[i] > eyeMoveTime[i])
                {
                    moveEye[i] = true;

                    eyeMoveTimeCurrent[i] = 0f;

                    eyeStartPos[i] = iris[i].transform.localPosition;

                    eyeTargetPos[i] = new Vector2(Random.Range(-eyeMoveRange, eyeMoveRange), Random.Range(-eyeMoveRange, eyeMoveRange));

                    eyeMoveTransition[i] = 0f;
                }
            }
        }
    }

    public void OnCardTake()
    {

    }

    public void OnSkipButtonPressed()
    {

    }

}
