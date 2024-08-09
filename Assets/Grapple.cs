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
    public bool retracting;

    public CircleCollider2D grapplingCollider;
    public ContactFilter2D hitFilter;
    public List<Collider2D> hitBuffer;
    public int collisionRes;

    public bool fireGrappling;
    public bool grapplingPullPlayer;
    public bool grappleStartSet;
    public bool overTerrain;
    public float playerSlowDecay = 1.1f;
    public float playerSlowMult = 0.8f;
    public float playerSlow = 1f;
    public float playerSlowBase = 0.3f;
    public float grappleSpeedBase = 5f;
    public float grappleSpeedToPlayer;
    public float grappleSpeedMult = 1.05f;
    public float grappleSpeedDecay = 0.8f;
    public Vector3 grappleVectorToPlayer;
    public float grappleSpeedBreak = 1f;
    public Vector3 retractDistance;
    public Vector3 updatedPos;
    public Vector3 lastPos;
    public Vector3 detectSpot;
    public Vector3 pollingSpot;

    public BezierCurve curve;
    public BezierPoint p0;
    public BezierPoint p1;
    public BezierPoint p2;
    public BezierPoint p3;
    public BezierPoint p4;
    public BezierPoint p5;
    public BezierPoint p6;
    public Vector3 holdPoint;
    public float totalLength;
    public float currentLength;
    public float divisionDistance = 0.5f;
    public Vector3[] pointsPositions;
    public int pointArraySize;
    public float curvePercent;
    public float dis;

    public Animator animator;
    public bool animSet;
    public float lengthForAnim = 6f;

    public bool hideHook;
    public SpriteRenderer grappleSprite;
    public Sprite hookEnd;
    public Sprite hookEndGrab;
    public Sprite hookBall;
    public int grappleArraySize = 50;
    public SpriteRenderer[] grappleSegments;
    public int currentGrappleSegment;
    public GameObject grappleSegementsParent;


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
            grappleSegments[i].transform.parent = grappleSegementsParent.transform;
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

        if (World.activeWorld.paused)
        {
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);

        if (grapplingPullPlayer)
        {
            this.transform.position = holdPoint;
        }

        if (grappleTargetPoint == Vector3.zero && !grapplingPullPlayer)
        {
            hideHook = false;

            animator.Play("BezierAnim");
            animator.Update(Time.deltaTime);

            updatedPos = activePlayer.transform.position;
            this.transform.position = updatedPos;

            Vector3 dirToMouse = worldMouse - activePlayer.transform.position;
            Vector2 xyToMouse = new Vector2(dirToMouse.x, dirToMouse.y);
            xyToMouse = xyToMouse.normalized;

            this.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(xyToMouse.x, xyToMouse.y, 0f)).eulerAngles;

            grappleTargetPoint = pointSetter.transform.position;
         
        }
        if(!grapplingPullPlayer)
        {

            if (grappleDistance <= grappleMaxDistance && !retracting)
            {
                updatedPos = Vector3.MoveTowards(updatedPos, grappleTargetPoint, (grappleSpeed * Time.deltaTime));
                lastPos = this.transform.position; 
                this.transform.position = updatedPos;
                grappleDistance = grappleDistance + (grappleSpeed * Time.deltaTime);

                Vector3 hookDistance = this.transform.position - activePlayer.transform.position;

                //new implementation
                if (hookDistance.magnitude > noGrappleBefore && !grappleStartSet)
                {
                    Physics2D.OverlapCircle(updatedPos.xy(), 0.05f, hitFilter, hitBuffer);

                    grappleStartSet = true;

                    if (hitBuffer.Count != 0)
                    {
                        overTerrain = true;
                    }
                    else
                    {
                        overTerrain = false;
                    }
                }
                if (grappleStartSet)
                {
                    for (int i = 1; i <= collisionRes; i++)
                    {
                        float collisionSpacing = (1f / collisionRes) * i;
                        detectSpot = (updatedPos - lastPos);

                        Vector3 detectSpotReduced = detectSpot * (collisionSpacing);

                        pollingSpot = lastPos + detectSpotReduced;

                        Vector3 pollingSpotDistance = pollingSpot - activePlayer.transform.position;

                        if (pollingSpotDistance.magnitude > noGrappleBefore)
                        {

                            Physics2D.OverlapCircle(pollingSpot.xy(), 0.03f, hitFilter, hitBuffer);

                            if (overTerrain)
                            {

                                if (hitBuffer.Count == 0)
                                {
                                    grapplingPullPlayer = true;
                                    holdPoint = pollingSpot;
                                    this.transform.position = pollingSpot;
                                    overTerrain = false;
                                    break;
                                }
                            }

                            else
                            {
                                if (hitBuffer.Count != 0)
                                {
                                    grapplingPullPlayer = true;
                                    holdPoint = pollingSpot;
                                    this.transform.position = pollingSpot;
                                    overTerrain = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                //end new

                ////Old implementation

                //Physics2D.OverlapCircle(updatedPos.xy(), 0.05f, hitFilter, hitBuffer);

                //if (noGrappleBefore <= 0f && !grappleStartSet)
                //{
                //    grappleStartSet = true;

                //    if (hitBuffer.Count != 0)
                //    {
                //        overTerrain = true;
                //    }
                //    else
                //    {
                //        overTerrain = false;
                //    }
                //}

                //if (noGrappleBefore <= 0f && grappleStartSet)
                //{
                //    if (overTerrain)
                //    {

                //        if (hitBuffer.Count == 0)
                //        {

                //            grapplingPullPlayer = true;
                //            holdPoint = this.transform.position;
                //            overTerrain = false;
                //        }
                //    }

                //    else
                //    {
                //        if (hitBuffer.Count != 0)
                //        {
                //            grapplingPullPlayer = true;
                //            holdPoint = this.transform.position;
                //            overTerrain = true;
                //        }

                //    }

                //}
                ////end of old implementation

            }
            else
            {
                retracting = true;
            }
            if (retracting)
            {

                updatedPos = Vector3.MoveTowards(updatedPos, activePlayer.transform.position, (grappleSpeed * Time.deltaTime));
                this.transform.position = updatedPos;
                grappleDistance = grappleDistance - (grappleSpeed * Time.deltaTime);

                Vector3 retractDistance = this.transform.position - activePlayer.transform.position;

                if (retractDistance.magnitude <= 0.1f)
                {
                    hideHook = true;
                    ResetToBase();
                }
            }
        }

        if (grapplingPullPlayer)
        {
            Vector3 playerToSetter = grappleTargetPoint - activePlayer.transform.position;
            Vector3 pointToSetter = updatedPos - grappleTargetPoint;
            Vector3 playerToPoint = updatedPos - activePlayer.transform.position;

            if (!animSet)
            {
                if(playerToPoint.magnitude >= lengthForAnim)
                {
                    animator.Play("BezierGrapple");
                }
                animSet = true;
            }
            if(animSet)
            {
                animator.Update(Time.deltaTime);
            }


            if (playerToSetter.sqrMagnitude > pointToSetter.sqrMagnitude)
            {
                if (grappleSpeedToPlayer < grappleSpeedBase)
                {

                    grappleSpeedToPlayer = grappleSpeedBase;
                    playerSlow = playerSlowBase;

                }
                else
                {

                    grappleSpeedToPlayer = grappleSpeedToPlayer * grappleSpeedMult;
                    playerSlow = playerSlow * playerSlowMult;
                    
                }
                // could target p0 instead of setter  ie.  playerToPoint
                grappleVectorToPlayer = playerToSetter.normalized * grappleSpeedToPlayer * Time.deltaTime;

            }
            else
            {
                hideHook = true;

                grappleSpeedToPlayer = grappleSpeedToPlayer * grappleSpeedDecay;
                grappleVectorToPlayer = playerToSetter.normalized * grappleSpeedToPlayer * Time.deltaTime;

                playerSlow = Mathf.Min(playerSlow * playerSlowDecay, 1f);


                if (grappleSpeedToPlayer <= grappleSpeedBreak)
                {
                    ResetToBase();
                }
            }
        }


        Vector3 dirToTargetPoint = this.transform.position - activePlayer.transform.position;
        Vector2 xy = new Vector2(dirToTargetPoint.x, dirToTargetPoint.y);
        xy = xy.normalized;

        this.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(xy.x, xy.y, 0f)).eulerAngles;


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
            ////dont like the sprite i have for the end when pulling, might change it later
            //if (grapplingPullPlayer)
            //{
            //    grappleSegments[0].sprite = hookEndGrab;
            //}
            //else
            //{
            //    grappleSegments[0].sprite = hookEnd;
            //}

            grappleSegments[i].transform.position = curve.GetPointAtDistance(dis);
            grappleSegments[i].gameObject.SetActive(true);
            dis = dis + curvePercent;
        }

        Vector3 thisToPlayer = activePlayer.transform.position - this.transform.position;

        for (int i = 0; i < grappleSegments.Length; i++)
        {
            if (hideHook)
            {
                grappleSegments[i].gameObject.SetActive(false);               
            }
            else
            {
                Vector3 segmentToThis = grappleSegments[i].transform.position - this.transform.position;
                Vector3 segmentToPlayer = grappleSegments[i].transform.position - activePlayer.transform.position;

                if (segmentToThis.sqrMagnitude > thisToPlayer.sqrMagnitude && segmentToThis.sqrMagnitude > segmentToPlayer.sqrMagnitude)
                {
                    grappleSegments[i].gameObject.SetActive(false);
                }
            }
          
        }

    }

    public void ResetToBase()
    {
        this.transform.position = activePlayer.transform.position;
        grapplingPullPlayer = false;
        fireGrappling = false;
        grappleVectorToPlayer = Vector3.zero;
        grappleStartSet = false;
        grappleTargetPoint = Vector3.zero;
        animSet = false;
        playerSlow = 1f;
        retracting = false;
        grappleDistance = 0f;
        grappleSpeedToPlayer = 0f;
    }


}