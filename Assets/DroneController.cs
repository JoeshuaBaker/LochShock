using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public GameObject droneCarrier;

    public GameObject droneDesiredPosOne;
    public GameObject droneDesiredPosTwo;

    public GameObject droneParentPosOne;
    public GameObject droneParentPosTwo;

    public GameObject dronePosOne;
    public GameObject dronePosTwo;

    public Animator droneOneAnimator;
    public Animator droneTwoAnimator;

    public SpriteRenderer droneOneHead;
    public SpriteRenderer droneOneHeadLit;
    public SpriteRenderer droneOneLegs;
    public SpriteRenderer droneOneLegsLit;

    public SpriteRenderer droneTwoHead;
    public SpriteRenderer droneTwoHeadLit;
    public SpriteRenderer droneTwoLegs;
    public SpriteRenderer droneTwoLegsLit;

    public Sprite[] clydeHead;
    public Sprite[] clydeHeadLit;
    public Sprite[] clydeLegs;
    public Sprite[] clydeLegsLit;

    public Sprite[] beetleHead;
    public Sprite[] beetleHeadLit;
    public Sprite[] beetleLegs;
    public Sprite[] beetleLegsLit;

    public float currentAnimTime;
    public float animationDuration;
    public float timePerFrame;
    public float animationDurationModified;

    public Vector2 dist;
    public Vector2 addedPos;
    public float distMagMod;

    public float minRadius;
    public float maxRadius;
    public float speed;

    public float minRadius2;
    public float maxRadius2;
    public float speed2;

    public Vector3 bobValue;

    public float faceLookCenter;
    public float faceLookClose;
    public float faceLookFar;

    public float maxRotation = 30f;

    public float largestDist;
    public float largestDistDecay;
    public float maxSpin;

    void Start()
    {
        
    }

    void Update()
    {
        currentAnimTime += Time.deltaTime; // mult by speed

        droneCarrier.transform.position = Player.activePlayer.transform.position;

        MoveDrones();

        AnimateLegs();

        AnimateHead();
    }

    public void ActivateDroneOne()
    {
        droneOneHeadLit.enabled = true;
        droneOneLegsLit.enabled = true;
        droneOneAnimator.Play(0, 0, 0f);
    }

    public void DeactiveDroneOne()
    {
        droneOneHeadLit.enabled = false;
        droneOneLegsLit.enabled = false;
    }

    public void ActivateDroneTwo()
    {
        droneTwoHeadLit.enabled = true;
        droneTwoLegsLit.enabled = true;
    }

    public void DeactiveDroneTwo()
    {
        droneTwoHeadLit.enabled = false;
        droneTwoLegsLit.enabled = false;
        droneTwoAnimator.Play(0, 0, 0f);
    }

    public void AnimateHead()
    {
        float rot = Mathf.Max(-maxRotation, (Mathf.Min(maxRotation, dist.x * 15f)));

        var oneRot = dronePosOne.transform.eulerAngles;
        var twoRot = dronePosTwo.transform.eulerAngles;

        oneRot.z = -rot;

        dronePosOne.transform.eulerAngles = oneRot;
        dronePosTwo.transform.eulerAngles = oneRot;

        if (dist.x < faceLookCenter && dist.x > -faceLookCenter)
        {
            droneOneHead.sprite = clydeHead[2];
            droneOneHeadLit.sprite = clydeHeadLit[2];

            droneTwoHead.sprite = beetleHead[2];
            droneTwoHeadLit.sprite = beetleHeadLit[2];
        }
        else if (dist.x > faceLookCenter)
        {
            if (dist.x < faceLookClose)
            {
                droneOneHead.sprite = clydeHead[3];
                droneOneHeadLit.sprite = clydeHeadLit[3];

                droneTwoHead.sprite = beetleHead[3];
                droneTwoHeadLit.sprite = beetleHeadLit[3];
            }
            else
            {
                droneOneHead.sprite = clydeHead[4];
                droneOneHeadLit.sprite = clydeHeadLit[4];

                droneTwoHead.sprite = beetleHead[4];
                droneTwoHeadLit.sprite = beetleHeadLit[4];
            }
        }
        else
        {
            if (dist.x > -faceLookClose)
            {
                droneOneHead.sprite = clydeHead[1];
                droneOneHeadLit.sprite = clydeHeadLit[1];

                droneTwoHead.sprite = beetleHead[1];
                droneTwoHeadLit.sprite = beetleHeadLit[1];
            }
            else
            {
                droneOneHead.sprite = clydeHead[0];
                droneOneHeadLit.sprite = clydeHeadLit[0];

                droneTwoHead.sprite = beetleHead[0];
                droneTwoHeadLit.sprite = beetleHeadLit[0];
            }
        }

        bobValue = new Vector3(0f, Mathf.Sin(Time.time *4f) * 0.15f, 0f);
        var bobValue2 = new Vector3(0f, Mathf.Sin((Time.time + 0.3f)* 4f) * 0.15f, 0f);

        dronePosOne.transform.localPosition = bobValue;
        dronePosTwo.transform.localPosition = bobValue2;
    }

    public void AnimateLegs()
    {
        if (clydeLegs == null || clydeLegs.Length == 0)
        {
            return;
        }

        largestDist = Mathf.Max(largestDist, dist.magnitude * distMagMod);

        if (largestDist > (dist.magnitude * distMagMod))
        {
            largestDist -= largestDistDecay * Time.deltaTime;

            largestDist = Mathf.Min(maxSpin, largestDist);

            if(largestDist < 0)
            {
                largestDist = 0f;
            }
        }

        animationDurationModified = animationDuration /(1f + largestDist);

        currentAnimTime %= animationDurationModified;

        timePerFrame = animationDurationModified / clydeLegs.Length;

        droneOneLegs.sprite = clydeLegs[(int)(currentAnimTime / timePerFrame)];
        droneOneLegsLit.sprite = clydeLegsLit[(int)(currentAnimTime / timePerFrame)];
        droneTwoLegs.sprite = beetleLegs[(int)(currentAnimTime / timePerFrame)];
        droneTwoLegsLit.sprite = beetleLegsLit[(int)(currentAnimTime / timePerFrame)];
    }

    public void MoveDrones()
    {
        dist = droneDesiredPosOne.transform.position - droneParentPosOne.transform.position;

        if (dist.magnitude > minRadius)
        {
            addedPos = dist.normalized * (speed * Time.deltaTime);

            if (dist.magnitude < maxRadius)
            {
                droneParentPosOne.transform.position = new Vector3((droneParentPosOne.transform.position.x + addedPos.x), (droneParentPosOne.transform.position.y + addedPos.y), 0f);
            }
            else
            {
                float extraDist = dist.magnitude - maxRadius;

                addedPos = dist.normalized * (speed * Time.deltaTime) * (1f + extraDist);

                droneParentPosOne.transform.position = new Vector3((droneParentPosOne.transform.position.x + addedPos.x), (droneParentPosOne.transform.position.y + addedPos.y), 0f);
            }
        }

        var dist2 = droneDesiredPosTwo.transform.position - droneParentPosTwo.transform.position;

        if (dist2.magnitude > minRadius2)
        {
            addedPos = dist2.normalized * (speed2 * Time.deltaTime);

            if (dist2.magnitude < maxRadius)
            {
                droneParentPosTwo.transform.position = new Vector3((droneParentPosTwo.transform.position.x + addedPos.x), (droneParentPosTwo.transform.position.y + addedPos.y), 0f);
            }
            else
            {
                float extraDist = dist2.magnitude - maxRadius2;

                addedPos = dist2.normalized * (speed2 * Time.deltaTime) * (1f + extraDist);

                droneParentPosTwo.transform.position = new Vector3((droneParentPosTwo.transform.position.x + addedPos.x), (droneParentPosTwo.transform.position.y + addedPos.y), 0f);
            }
        }

        ////funny but not what i want
        //dist = droneDesiredPosOne.transform.position - droneParentPosOne.transform.position;

        //addedPos = dist.normalized * (distancePerSec * Time.deltaTime);

        //posChange += addedPos;

        //droneParentPosOne.transform.position = new Vector3((droneParentPosOne.transform.position.x + posChange.x), (droneParentPosOne.transform.position.y + posChange.y), 0f);

        //if (dist.magnitude > targetPointRadius)
        //{
        //    acceleration = Mathf.SmoothStep(0.01f, maxAcceleration, velocity / (maxVelocity * 5f));
        //    velocity = Mathf.Min(maxVelocity, velocity + acceleration);
        //    droneParentPosOne.transform.position += dist.normalized * velocity;
        //}
        //else
        //{
        //    acceleration = Mathf.SmoothStep(0.01f, maxAcceleration, velocity / (maxVelocity * 5f));
        //    velocity = Mathf.Min(maxVelocity, velocity - acceleration);
        //    droneParentPosOne.transform.position -= dist.normalized * velocity;
        //}
    }
}
