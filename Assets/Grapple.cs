using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{ 
    public Player activePlayer;
    public Vector2 mouseDirection;
    public BezierCurve curve;
    public BezierPoint p0;
    public BezierPoint p1;
    public BezierPoint p2;
    public BezierPoint p3;
    public BezierPoint p4;
    public BezierPoint p5;
    public BezierPoint p6;
    public float totalLength;
    public float oneTwoLength;
    public float ballDistance;
    public float curveDivisions;
    public Vector3[] points;
    public int pointArraySize;

    public GameObject grappleSegment;
    public SpriteRenderer grappleSprite;
    public Sprite hookEnd;
    public Sprite hookEndGrab;
    public Sprite hookBall;
    public int grappleArraySize =50;
    public SpriteRenderer[] grappleSegments;
    

    // Start is called before the first frame update
    void Start()
    {
        activePlayer = Player.activePlayer;

        grappleSegments = new SpriteRenderer[grappleArraySize];

        points = new Vector3[pointArraySize];

        for (int i = 0; i < grappleSegments.Length; i++)
        {
            grappleSegments[i] = Instantiate(grappleSprite);
            grappleSegments[i].transform.parent = this.transform.parent;
            grappleSegments[i].gameObject.SetActive(false);

            if(i == 0)
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
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);

        this.transform.position = worldMouse;

        Vector3 dirToMouse = worldMouse - activePlayer.transform.position;
        Vector2 xy = new Vector2(dirToMouse.x, dirToMouse.y);
        xy = xy.normalized;
        mouseDirection = xy;

        this.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(xy.x, xy.y, 0f)).eulerAngles;

        totalLength = 0f;

        totalLength = BezierCurve.ApproximateLength(p0, p1, 10);

        totalLength = totalLength + BezierCurve.ApproximateLength(p1, p2, 10);

        totalLength = totalLength + BezierCurve.ApproximateLength(p2, p3, 10);

        totalLength = totalLength + BezierCurve.ApproximateLength(p3, p4, 10);

        totalLength = totalLength + BezierCurve.ApproximateLength(p4, p5, 10);

        totalLength = totalLength + BezierCurve.ApproximateLength(p5, p6, 10);

        curveDivisions = totalLength / ballDistance;

        //BezierCurve.GetPoint()





    }

    void GrappleLogic()
    {

    }
}
