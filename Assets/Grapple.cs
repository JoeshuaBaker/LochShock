using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public Player activePlayer;
    public Vector2 mouseDirection;
    public Vector3 grappleTargetPoint;
    public float grappleSpeed = 10f;
    public float noGrappleBefore = 0.3f;
    public float grappleDistance;
    public float grappleMaxDistance = 10f;
    public GameObject pointSetter;

    public CircleCollider2D grapplingCollider;
    public ContactFilter2D hitFilter;
    public List<Collider2D> hitBuffer;

    public bool fireGrappling;
    public bool grapplingReturn;
    public bool grapplingPullPlayer;
    public bool grappleStartSet;
    public bool overTerrain;
    public float playerSpeedMult = 1f;
    public float grappleSpeedToPlayer;

    public BezierCurve curve;
    public BezierPoint p0;
    public BezierPoint p1;
    public BezierPoint p2;
    public BezierPoint p3;
    public BezierPoint p4;
    public BezierPoint p5;
    public BezierPoint p6;
    public float totalLength;
    public float currentLength;
    public float divisionDistance = 0.5f;
    public Vector3[] pointsPositions;
    public int pointArraySize;
    public float curvePercent;
    public float dis;


    public SpriteRenderer grappleSprite;
    public Sprite hookEnd;
    public Sprite hookEndGrab;
    public Sprite hookBall;
    public int grappleArraySize = 50;
    public SpriteRenderer[] grappleSegments;
    public int currentGrappleSegment;


    // Start is called before the first frame update
    void Start()
    {
        activePlayer = Player.activePlayer;

        grappleSegments = new SpriteRenderer[grappleArraySize];

        pointsPositions = new Vector3[pointArraySize];

        hitBuffer = new List<Collider2D>();
        hitFilter = new ContactFilter2D
        {
            useTriggers = false,
            layerMask = 1 << LayerMask.NameToLayer("Terrain"),
            useLayerMask = true
        };

        for (int i = 0; i < grappleSegments.Length; i++)
        {
            grappleSegments[i] = Instantiate(grappleSprite);
            grappleSegments[i].transform.parent = this.transform.parent;
            grappleSegments[i].gameObject.SetActive(false);

            if (i == 0)
            {
                grappleSegments[i].sprite = hookEnd;
            }
            else
            {
                grappleSegments[i].sprite = hookBall;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (!fireGrappling)
        {
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);

        if (grappleTargetPoint == Vector3.zero)
        {

            Vector3 dirToMouse = worldMouse - activePlayer.transform.position;
            Vector2 xy = new Vector2(dirToMouse.x, dirToMouse.y);
            xy = xy.normalized;
            mouseDirection = xy;

            this.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(xy.x, xy.y, 0f)).eulerAngles;

            grappleTargetPoint = pointSetter.transform.position;

            this.transform.position = Vector3.MoveTowards(this.transform.position, grappleTargetPoint, (grappleSpeed * Time.deltaTime));
            grappleDistance = grappleDistance + (grappleSpeed * Time.deltaTime);

            noGrappleBefore = noGrappleBefore - Time.deltaTime;
         
        }
        else
        {

            Vector3 dirToTargetPoint = grappleTargetPoint - activePlayer.transform.position;
            Vector2 xy = new Vector2(dirToTargetPoint.x, dirToTargetPoint.y);
            xy = xy.normalized;

            this.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(xy.x, xy.y, 0f)).eulerAngles;

            if (grappleDistance <= grappleMaxDistance)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, grappleTargetPoint, (grappleSpeed * Time.deltaTime));
                grappleDistance = grappleDistance + (grappleSpeed * Time.deltaTime);

                noGrappleBefore = noGrappleBefore - Time.deltaTime;

                if (noGrappleBefore <= 0f && !grappleStartSet)
                {
                    grappleStartSet = true;

                    Physics2D.OverlapCollider(grapplingCollider, hitFilter, hitBuffer);

                    if(hitBuffer.Count > 0)
                    {
                        overTerrain = true;
                    }
                    else
                    {
                        overTerrain = false;
                    }
                }

                if(noGrappleBefore <=0f && grappleStartSet)
                {
                    if (overTerrain)
                    {
                        Physics2D.OverlapCollider(grapplingCollider, hitFilter, hitBuffer);

                        if (hitBuffer.Count == 0)
                        {
                            grapplingPullPlayer = true;
                        }
                     
                    }

                    else
                    {
                        Physics2D.OverlapCollider(grapplingCollider, hitFilter, hitBuffer);

                        if (hitBuffer.Count > 0)
                        {
                            grapplingPullPlayer = true;
                        }

                    }
                }
            }
            else
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, activePlayer.transform.position, (grappleSpeed * Time.deltaTime));
                grappleDistance = grappleDistance - (grappleSpeed * Time.deltaTime);
            }
        }
        


        var thePoints = curve.GetAnchorPoints();

        for (int i = 0; i < pointsPositions.Length; i++)
        {
            pointsPositions[i] = thePoints[i].transform.position;
        }

        totalLength = 0f;

        for (int i = 1; i < pointsPositions.Length; i++)
        {
            totalLength = totalLength + BezierCurve.ApproximateLength(thePoints[i - 1], thePoints[i], 10);
        }

        curvePercent = 1 / (totalLength / divisionDistance);

        dis = 0f;


        for(int i = 0; i <grappleSegments.Length; i++)
        {
            grappleSegments[i].transform.position = p6.transform.position;
        }

        for (int i = 0; i < grappleSegments.Length; i++)
        {
            if(dis >= 1f)
            {
                break;
            }
            grappleSegments[i].transform.position = curve.GetPointAtDistance(dis);
            grappleSegments[i].gameObject.SetActive(true);
            dis = dis + curvePercent;
        }



        mousePos = new Vector3(worldMouse.x, worldMouse.y, 0f);
        Vector3 mouseToPlayer = activePlayer.transform.position - mousePos;

        for (int i = 0; i < grappleSegments.Length; i++)
        {

            Vector3 grappleToMouse = grappleSegments[i].transform.position - mousePos;
            Vector3 grappleToPlayer = grappleSegments[i].transform.position - activePlayer.transform.position;

            if (grappleToMouse.magnitude > mouseToPlayer.magnitude && grappleToMouse.magnitude > grappleToPlayer.magnitude)
            {
                grappleSegments[i].gameObject.SetActive(false);
            }

        }

    }

}